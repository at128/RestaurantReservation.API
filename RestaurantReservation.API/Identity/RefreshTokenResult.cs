namespace RestaurantReservation.API.Identity
{
    public record RefreshTokenResult(string RawToken,DateTime ExpiresUtc);
}
