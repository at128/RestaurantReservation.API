using FluentValidation;
using RestaurantReservation.API.Contracts.Auth;

namespace RestaurantReservation.API.Validations.Auth
{
    public class LoginRequestValidator:AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(r => r.Username)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(200);

            RuleFor(r => r.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(50);
        }
    }
}
