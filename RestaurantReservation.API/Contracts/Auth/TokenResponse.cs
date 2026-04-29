namespace RestaurantReservation.API.Contracts.Auth
{
    public record TokenResponse(string AccessToken,string RefreshToken,DateTime ExpiresUtc);
}
