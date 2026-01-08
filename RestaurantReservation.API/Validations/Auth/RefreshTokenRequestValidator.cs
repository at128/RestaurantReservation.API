using FluentValidation;
using RestaurantReservation.API.Contracts.Auth;

namespace RestaurantReservation.API.Validations.Auth
{
    public class RefreshTokenRequestValidator:AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(r => r.RefreshToken)
                .NotEmpty()
                .Length(64);

        }
    }
}
