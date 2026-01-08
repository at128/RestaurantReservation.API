namespace RestaurantReservation.API.Identity
{
    public record TokenGenerationResult(
    string AccessToken,
    DateTime ExpiresUtc
);
}
