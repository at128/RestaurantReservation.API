namespace RestaurantReservation.API.Contracts.Reservations
{
    public record UpdateReservationRequest(
    int CustomerId,
    int TableId,
    int PartySize,
    DateTime ReservationDate
);
}
