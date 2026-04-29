using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.Authorization;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Respositories;

namespace RestaurantReservation.API.Endpoints
{
    public static class EmployeeEndpoints
    {
        public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/employees")
                .WithTags("Employees");

            group.MapGet("/managers", GetManagers)
                .RequireAuthorization(Permission.Employees.ViewManagers);
            group.MapGet("/{employeeId:int}/average=order-amount", GetAverageOrderAmount)
                .RequireAuthorization(Permission.Employees.ViewAverageOrderAmount);


            return group;
        }

        private static async Task<IResult> GetAverageOrderAmount(int employeeId, QueryRepository queryRepository,RestaurantReservationDbContext db,CancellationToken ct)
        {
            if(!await db.Employees.AnyAsync(e => e.EmployeeId == employeeId,ct))
            {
                return Results.NotFound($"Employee with ID {employeeId} not found.");
            }

            var result = await queryRepository.CalculateAverageOrderAmountAsync(employeeId);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetManagers(QueryRepository queryRepository)
        {
            var managers = await queryRepository.ListManagersAsync();

            return Results.Ok(managers);
        }
    } 

}
