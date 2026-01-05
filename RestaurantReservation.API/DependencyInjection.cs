using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.Validations.Reservations;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Respositories;
namespace RestaurantReservation.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer()
                    .AddSwaggerGen()
                    .AddProblemDetails();


            services.AddValidatorsFromAssemblyContaining<CreateReservationRequestValidator>();


            services.AddDbContext<RestaurantReservationDbContext>(options =>
            {
                string connection = configuration.GetSection("Default").Value
                    ?? Environment.GetEnvironmentVariable("RestaurantReservationConStr")!;

                if(string.IsNullOrEmpty(connection))
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
    }
}
