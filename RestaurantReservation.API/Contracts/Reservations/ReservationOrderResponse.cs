namespace RestaurantReservation.API.Contracts.Reservations
{
    public record ReservationOrderResponse(
    int OrderId,
    DateTime OrderDate,
    decimal TotalAmount,
    int EmployeeId,
    List<ReservationOrderItemResponse> Items
);

}
