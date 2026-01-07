namespace RestaurantReservation.API.Contracts.Reservations
{
    public record ReservationOrderItemResponse(
    int OrderItemId,
    int MenuItemId,
    string MenuItemName,
    int Quantity,
    decimal UnitPrice
);
}
