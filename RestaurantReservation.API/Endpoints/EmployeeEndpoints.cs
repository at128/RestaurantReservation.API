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

            group.MapGet("/managers", );
            group.MapGet("/{employeeId:int}/average=order-amount", GetAverageOrderAmount);


            return group;
        }

        private static async Task<IResult> GetAverageOrderAmount(QueryRepository queryRepository)
        {
            var result = await queryRepository.CalculateAverageOrderAmountAsync(0);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetManagers(QueryRepository queryRepository)
        {
            var managers = await queryRepository.ListManagersAsync();

            return Results.Ok(managers);
        }
    } 

}
