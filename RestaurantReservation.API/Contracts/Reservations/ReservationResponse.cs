namespace RestaurantReservation.API.Contracts.Reservations
{
    public record ReservationResponse(
    int ReservationId,
    int CustomerId,
    int TableId,
    int PartySize,
    DateTime ReservationDate
);
}
