namespace RestaurantReservation.API.Contracts.Reservations
{
    public record OrderedMenuItemResponse(
    int MenuItemId,
    string Name,
    string Description,
    decimal Price,
    int TotalQuantity
);
}
