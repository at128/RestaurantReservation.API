namespace RestaurantReservation.API.Contracts.Reservations
{
    public record CreateReservationRequest(
    int CustomerId,
    int TableId,
    int PartySize,
    DateTime ReservationDate
    );
}
