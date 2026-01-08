using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Models.Identity;

namespace RestaurantReservation.API.Identity
{
    public class RefreshTokenService(IConfiguration configuration,RestaurantReservationDbContext db)
    {
        public async Task<RefreshTokenResult> IssueAsync(string userId,CancellationToken ct)
        {
            var days = int.Parse(configuration["JwtSettings:RefreshTokenExpirationInDays"]!);
            var rawToken = JwtTokenProvider.GenerateRefreshTokenRaw();
            var hash = JwtTokenProvider.HashRefreshToken(rawToken);

            var entity = new RefreshToken
            {
                UserId = userId,
                TokenHash = hash,
                ExpiresUtc = DateTime.UtcNow.AddDays(days),
                CreatedUtc = DateTime.UtcNow
            };

            db.RefreshTokens.Add(entity);
            await db.SaveChangesAsync(ct);

            return new RefreshTokenResult(
                RawToken : rawToken,
                ExpiresUtc: entity.ExpiresUtc
            );
        }


        public async Task<string?> ValidateAndGetUserIdAsync(string rawToken, CancellationToken ct)
        {
            var hash = JwtTokenProvider.HashRefreshToken(rawToken);

            var token = await db.RefreshTokens
                .AsTracking()
                .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

            if (token is null || !token.IsActive) return null;
            return token.UserId;
        }

        public async Task RevokeAsync(string rawToken, CancellationToken ct)
        {
            var hash = JwtTokenProvider.HashRefreshToken(rawToken);
            var token = await db.RefreshTokens.AsTracking().FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
            if (token is null) return;

            token.RevokedUtc = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }
}
