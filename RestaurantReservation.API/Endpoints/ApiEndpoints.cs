namespace RestaurantReservation.API.Endpoints
{
    public static class ApiEndpoints
    {
        public static IEndpointRouteBuilder MapApi(this IEndpointRouteBuilder app)
        {
            var api = app.MapGroup("api");

            api.MapReservationEndpoints();

            return app;
        }

    }
}
