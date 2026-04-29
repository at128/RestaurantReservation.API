
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.Contracts.Auth;
using RestaurantReservation.API.Filters;
using RestaurantReservation.API.Identity;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Models.Identity;
using LoginRequest = RestaurantReservation.API.Contracts.Auth.LoginRequest;

namespace RestaurantReservation.API.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder api)
        {
            var auth = api.MapGroup("/auth").WithTags("Authentication");

            auth.MapPost("/login", Login)
                .AddEndpointFilter<ValidationFilter<LoginRequest>>();
            auth.MapPost("/refresh", Refresh)
                .AddEndpointFilter<ValidationFilter<RefreshTokenRequest>>();

            auth.MapPost("/logout", Logout);

            return auth;
        }

        private static async Task<IResult> Logout(
            RefreshTokenRequest req,
            RefreshTokenService refresh,
            CancellationToken ct)
        {
            await refresh.RevokeAsync(req.RefreshToken, ct);
            return Results.NoContent();
        }

        private static async Task<IResult> Login(
            Contracts.Auth.LoginRequest request,
            RestaurantReservationDbContext db,
            JwtTokenProvider jwt,
            RefreshTokenService refreshTokenService,
            CancellationToken ct
            )
        {
            var user = await db.AppUsers
                .FirstOrDefaultAsync(u => u.Email==request.Username, ct);

            if (user == null) return Results.Unauthorized();

            if (!PasswordHasher.Verify(request.Password,user.PasswordHash))
            {
                return Results.Unauthorized();
            }

            var accessToken = await jwt.GenerateTokenAccessAsync(user.Id, ct);
            var refreshToken = await refreshTokenService.IssueAsync(user.Id, ct);
            var response = new TokenResponse(
                AccessToken: accessToken.AccessToken,
                RefreshToken: refreshToken.RawToken,
                ExpiresUtc: refreshToken.ExpiresUtc
            );
            return Results.Ok(response);
        }

        private static async Task<IResult> Refresh(
        RefreshTokenRequest req,
        JwtTokenProvider jwt,
        RefreshTokenService refresh,
        CancellationToken ct)
        {
            var userId = await refresh.ValidateAndGetUserIdAsync(req.RefreshToken, ct);
            if (userId == null) return Results.Unauthorized();
            var accessToken = await jwt.GenerateTokenAccessAsync(userId, ct);
            var response = new TokenResponse(
                AccessToken: accessToken.AccessToken,
                RefreshToken: req.RefreshToken,
                ExpiresUtc: accessToken.ExpiresUtc
            );
            return Results.Ok(response);
        }
    }
}
