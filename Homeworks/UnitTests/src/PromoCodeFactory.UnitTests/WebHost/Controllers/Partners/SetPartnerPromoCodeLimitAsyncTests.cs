using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.WebHost.Controllers.Partners.TestsClassFixture;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests : IClassFixture<SetPartnerPromoCodeLimitTestFixture>
    {
        private readonly SetPartnerPromoCodeLimitTestFixture _testFixture;
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests(SetPartnerPromoCodeLimitTestFixture testFixture)
        {
            _testFixture = testFixture;
            _partnersRepositoryMock = _testFixture.Fixture.Create<Mock<IRepository<Partner>>>();
            _partnersController = new PartnersController(_partnersRepositoryMock.Object);
        }

        #region Заполнение данных через AutoFixture

        private Partner CreateActivePartnerWithoutLimits()
        {
            return _testFixture.Fixture.Build<Partner>()
                .With(x => x.IsActive, true)
                .With(x => x.NumberIssuedPromoCodes, 50)
                .With(x => x.PartnerLimits, new List<PartnerPromoCodeLimit>())
                .Create();
        }

        private Partner CreateActivePartnerWithActiveLimit()
        {
            var partner = CreateActivePartnerWithoutLimits();
            var activeLimit = _testFixture.Fixture.Build<PartnerPromoCodeLimit>()
                .With(x => x.PartnerId, partner.Id)
                .With(x => x.Partner, partner)
                .With(x => x.CreateDate, DateTime.Now.AddDays(-10))
                .With(x => x.EndDate, DateTime.Now.AddDays(30))
                .With(x => x.Limit, 100)
                .Without(x => x.CancelDate)
                .Create();
                
            partner.PartnerLimits = new List<PartnerPromoCodeLimit> { activeLimit };
            return partner;
        }

        private Partner CreateActivePartnerWithCancelledLimit()
        {
            var partner = CreateActivePartnerWithoutLimits();
            var cancelledLimit = _testFixture.Fixture.Build<PartnerPromoCodeLimit>()
                .With(x => x.PartnerId, partner.Id)
                .With(x => x.Partner, partner)
                .With(x => x.CreateDate, DateTime.Now.AddDays(-20))
                .With(x => x.EndDate, DateTime.Now.AddDays(-10))
                .With(x => x.Limit, 100)
                .With(x => x.CancelDate, DateTime.Now.AddDays(-5))
                .Create();
                
            partner.PartnerLimits = new List<PartnerPromoCodeLimit> { cancelledLimit };
            return partner;
        }

        private Partner CreateInactivePartner()
        {
            return _testFixture.Fixture.Build<Partner>()
                .With(x => x.IsActive, false)
                .With(x => x.NumberIssuedPromoCodes, 50)
                .With(x => x.PartnerLimits, new List<PartnerPromoCodeLimit>())
                .Create();
        }

        private SetPartnerPromoCodeLimitRequest CreateValidRequest()
        {
            return _testFixture.Fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(x => x.Limit, 150)
                .With(x => x.EndDate, DateTime.Now.AddDays(60))
                .Create();
        }

        private SetPartnerPromoCodeLimitRequest CreateRequestWithLimit(int limit)
        {
            return _testFixture.Fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(x => x.Limit, limit)
                .With(x => x.EndDate, DateTime.Now.AddDays(60))
                .Create();
        }

        private Guid CreatePartnerId()
        {
            return _testFixture.Fixture.Create<Guid>();
        }

        #endregion

        /// <summary>
        /// Тест проверяет, что при попытке установить лимит для несуществующего партнера
        /// метод возвращает статус 404 NotFound и не выполняет обновление в базе данных
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = CreatePartnerId();
            var request = CreateValidRequest();
            Partner partner = null;
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
            _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Partner>()), Times.Never);
        }

        /// <summary>
        /// Тест проверяет, что при попытке установить лимит для заблокированного партнера (IsActive=false)
        /// метод возвращает статус 400 BadRequest с соответствующим сообщением об ошибке
        /// и не выполняет обновление в базе данных
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsInactive_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = CreatePartnerId();
            var request = CreateValidRequest();
            var partner = CreateInactivePartner();
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Данный партнер не активен");
            _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Partner>()), Times.Never);
        }

        /// <summary>
        /// Параметризованный тест проверяет, что при установке лимита со значением меньше или равным 0
        /// метод возвращает статус 400 BadRequest с сообщением "Лимит должен быть больше 0"
        /// и не выполняет обновление в базе данных
        /// </summary>
        /// <param name="invalidLimit">Невалидное значение лимита (0, отрицательное число)</param>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-100)]
        public async Task SetPartnerPromoCodeLimitAsync_InvalidLimit_ReturnsBadRequest(int invalidLimit)
        {
            // Arrange
            var partnerId = CreatePartnerId();
            var request = CreateRequestWithLimit(invalidLimit);
            var partner = CreateActivePartnerWithoutLimits();
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Лимит должен быть больше 0");
            _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Partner>()), Times.Never);
        }

        /// <summary>
        /// Тест проверяет бизнес-логику при установке нового лимита партнеру, у которого уже есть активный лимит:
        /// 1. Обнуляется количество выданных промокодов (NumberIssuedPromoCodes = 0)
        /// 2. Отключается предыдущий активный лимит (устанавливается CancelDate)
        /// 3. Метод возвращает статус 201 Created
        /// 4. Выполняется сохранение изменений в базе данных
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerHasActiveLimit_ResetsNumberIssuedPromoCodesAndCancelsActiveLimit()
        {
            // Arrange
            var partnerId = CreatePartnerId();
            var request = CreateValidRequest();
            var partner = CreateActivePartnerWithActiveLimit();
            var originalActiveLimit = partner.PartnerLimits.First(x => !x.CancelDate.HasValue);
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            partner.NumberIssuedPromoCodes.Should().Be(0);
            originalActiveLimit.CancelDate.Should().NotBeNull();
            originalActiveLimit.CancelDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
            _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(partner), Times.Once);
        }

        /// <summary>
        /// Тест проверяет, что при установке лимита партнеру, у которого нет активных лимитов
        /// (есть только отмененные лимиты), количество выданных промокодов НЕ обнуляется,
        /// так как согласно бизнес-логике обнуление происходит только при наличии активного лимита
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerHasNoCancelledLimitsOnly_DoesNotResetNumberIssuedPromoCodes()
        {
            // Arrange
            var partnerId = CreatePartnerId();
            var request = CreateValidRequest();
            var partner = CreateActivePartnerWithCancelledLimit();
            var originalNumberIssuedPromoCodes = partner.NumberIssuedPromoCodes;
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            partner.NumberIssuedPromoCodes.Should().Be(originalNumberIssuedPromoCodes);
            _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(partner), Times.Once);
        }

        /// <summary>
        /// Тест проверяет корректность создания и сохранения нового лимита в базе данных:
        /// 1. Создается новый объект PartnerPromoCodeLimit с правильными параметрами
        /// 2. Лимит добавляется к коллекции PartnerLimits партнера
        /// 3. Устанавливаются корректные связи (PartnerId, Partner)
        /// 4. Выполняется сохранение в базе данных через репозиторий
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_CreatesNewLimitInDatabase()
        {
            // Arrange
            var partnerId = CreatePartnerId();
            var request = CreateValidRequest();
            var partner = CreateActivePartnerWithoutLimits();
            var originalLimitsCount = partner.PartnerLimits.Count;
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            
            partner.PartnerLimits.Should().HaveCount(originalLimitsCount + 1);
            
            var newLimit = partner.PartnerLimits.LastOrDefault();
            newLimit.Should().NotBeNull();
            newLimit.Limit.Should().Be(request.Limit);
            newLimit.EndDate.Should().Be(request.EndDate);
            newLimit.PartnerId.Should().Be(partner.Id);
            newLimit.Partner.Should().Be(partner);
            newLimit.CreateDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
            newLimit.CancelDate.Should().BeNull();
            
            _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(partner), Times.Once);
        }

        /// <summary>
        /// Тест проверяет корректность HTTP-ответа при успешном создании лимита:
        /// 1. Возвращается статус 201 Created
        /// 2. В заголовке Location указывается правильный action (GetPartnerLimitAsync)
        /// 3. Передаются корректные параметры маршрута (id партнера и limitId нового лимита)
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_ReturnsCreatedAtActionWithCorrectParameters()
        {
            // Arrange
            var partnerId = CreatePartnerId();
            var request = CreateValidRequest();
            var partner = CreateActivePartnerWithoutLimits();
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            
            var createdResult = result as CreatedAtActionResult;
            createdResult.ActionName.Should().Be("GetPartnerLimitAsync");
            createdResult.RouteValues.Should().ContainKey("id");
            createdResult.RouteValues["id"].Should().Be(partner.Id);
            createdResult.RouteValues.Should().ContainKey("limitId");
            
            var newLimit = partner.PartnerLimits.LastOrDefault();
            createdResult.RouteValues["limitId"].Should().Be(newLimit.Id);
        }

        /// <summary>
        /// Тест проверяет, что при установке лимита партнеру, у которого нет лимитов вообще,
        /// количество выданных промокодов НЕ изменяется, так как обнуление происходит
        /// только при наличии активного лимита, который нужно отменить
        /// </summary>
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerWithoutLimits_DoesNotResetNumberIssuedPromoCodes()
        {
            // Arrange
            var partnerId = CreatePartnerId();
            var request = CreateValidRequest();
            var partner = CreateActivePartnerWithoutLimits();
            var originalNumberIssuedPromoCodes = partner.NumberIssuedPromoCodes;
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            partner.NumberIssuedPromoCodes.Should().Be(originalNumberIssuedPromoCodes);
            _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(partner), Times.Once);
        }
    }
}