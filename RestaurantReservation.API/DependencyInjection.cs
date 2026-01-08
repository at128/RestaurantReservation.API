using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantReservation.API.Authorization;
using RestaurantReservation.API.Identity;
using RestaurantReservation.API.Validations.Reservations;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Respositories;
using System.Text;
namespace RestaurantReservation.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer()
                    .AddSwaggerGen()
                    .AddProblemDetails()
                    .AddDbContexts(configuration)
                    .AddJwtAuthentication(configuration)
                    .AddAuthorizationPolicies()
                    .AddValidation()
                    .AddBusinessServices();

            return services;
        }

        private static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RestaurantReservationDbContext>(options =>
            {
                string connection = configuration.GetConnectionString("DefaultConnection")
                    ?? Environment.GetEnvironmentVariable("RestaurantReservationConStr")!;

                if (string.IsNullOrEmpty(connection))
                {
                    throw new InvalidOperationException("Database connection string is not configured.");
                }

                options.UseSqlServer(connection);

            });

            services.AddScoped<QueryRepository>();
            services.AddScoped<RestaurantRepository>();
            services.AddScoped<TableRepository>();
            services.AddScoped<CustomerRepository>();
            services.AddScoped<EmployeeRepository>();
            services.AddScoped<ReservationRepository>();
            services.AddScoped<OrderRepository>();
            services.AddScoped<OrderItemRepository>();
            services.AddScoped<MenuItemRepository>();



            return services;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            var jwtSettings = configuration.GetSection("JwtSettings");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
                };
            });
            return services;
        }

        private static IServiceCollection AddValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateReservationRequestValidator>();
            return services;
        }

        private static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                AddReservationPolicies(options);
                AddEmployeePolicies(options);
            });

            return services;
        }

        private static void AddReservationPolicies(AuthorizationOptions options)
        {
            options.AddPolicy(Permission.Reservations.Read,
                policy => policy.RequireClaim("permission", Permission.Reservations.Read));

            options.AddPolicy(Permission.Reservations.Create,
                policy => policy.RequireClaim("permission", Permission.Reservations.Create));

            options.AddPolicy(Permission.Reservations.Update,
                policy => policy.RequireClaim("permission", Permission.Reservations.Update));

            options.AddPolicy(Permission.Reservations.Delete,
                policy => policy.RequireClaim("permission", Permission.Reservations.Delete));

            options.AddPolicy(Permission.Reservations.ViewOrders,
                policy => policy.RequireClaim("permission", Permission.Reservations.ViewOrders));

            options.AddPolicy(Permission.Reservations.ViewMenuItems,
                policy => policy.RequireClaim("permission", Permission.Reservations.ViewMenuItems));
        }

        private static void AddEmployeePolicies(AuthorizationOptions options)
        {
            options.AddPolicy(Permission.Employees.ViewManagers,
                policy => policy.RequireClaim("permission", Permission.Employees.ViewManagers));

            options.AddPolicy(Permission.Employees.ViewAverageOrderAmount,
                policy => policy.RequireClaim("permission", Permission.Employees.ViewAverageOrderAmount));
        }

        private static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<JwtTokenProvider>();
            services.AddScoped<RefreshTokenService>(); // إذا مستخدمه عندك

            // Later: services.AddScoped<IReservationService, ReservationService>();

            return services;
        }
    }
}
