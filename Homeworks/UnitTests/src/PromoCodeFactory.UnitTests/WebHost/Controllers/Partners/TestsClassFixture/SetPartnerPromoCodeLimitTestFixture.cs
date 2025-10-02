using AutoFixture;
using AutoFixture.AutoMoq;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners.TestsClassFixture
{
    // Fixture класс для общих настроек
    public class SetPartnerPromoCodeLimitTestFixture
    {
        public Fixture Fixture { get; }

        public SetPartnerPromoCodeLimitTestFixture()
        {
            Fixture = new Fixture();
            Fixture.Customize(new AutoMoqCustomization());

            //Настройка AutoFixture для предотвращения циклических ссылок
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            //Partner
            Fixture.Customize<Partner>(composer => composer
                .With(x => x.IsActive, true)
                .With(x => x.NumberIssuedPromoCodes, 50)
                .With(x => x.PartnerLimits, new List<PartnerPromoCodeLimit>()));

            //PartnerPromoCodeLimit
            Fixture.Customize<PartnerPromoCodeLimit>(composer => composer
                .With(x => x.CreateDate, DateTime.Now.AddDays(-10))
                .With(x => x.EndDate, DateTime.Now.AddDays(30))
                .With(x => x.Limit, 100)
                .Without(x => x.CancelDate)
                .Without(x => x.Partner)); // Избегаем циклических ссылок

            //SetPartnerPromoCodeLimitRequest
            Fixture.Customize<SetPartnerPromoCodeLimitRequest>(composer => composer
                .With(x => x.Limit, 150)
                .With(x => x.EndDate, DateTime.Now.AddDays(60)));
        }
    }
}
