using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Middlewares;
using PromoCodeFactory.WebHost.Services;
using PromoCodeFactory.WebHost.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            if (Configuration.GetConnectionString("Default") == null)
            {
                services.AddSingleton(typeof(IRepository<Employee>), (x) =>
                    new InMemoryRepository<Employee>(FakeDataFactory.Employees));
                services.AddSingleton(typeof(IRepository<Role>), (x) =>
                    new InMemoryRepository<Role>(FakeDataFactory.Roles));
                services.AddSingleton(typeof(IRepository<Preference>), (x) =>
                    new InMemoryRepository<Preference>(FakeDataFactory.Preferences));
                services.AddSingleton(typeof(IRepository<Customer>), (x) =>
                    new InMemoryRepository<Customer>(FakeDataFactory.Customers));
            }
            else
            {
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("Default")));

                services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
                services.AddScoped<IDbInitializer, EfDbInitializer>();
            }

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IRoleService, RolesService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IPreferenceService, PreferenceService>();
            services.AddScoped<IPromocodeService, PromocodesService>();

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseOpenApi();
            app.UseSwaggerUi(x =>
            {
                x.DocExpansion = "list";
            });
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Попытка инициализации БД только если сервис зарегистрирован
            using var scope = app.ApplicationServices.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
            dbInitializer?.InitializeDb();
        }
    }
}