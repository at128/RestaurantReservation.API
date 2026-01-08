using FluentValidation;
using RestaurantReservation.API.Contracts.Reservations;

namespace RestaurantReservation.API.Validations.Reservations
{
    public class UpdateReservationRequestValidator : AbstractValidator<UpdateReservationRequest>
    {
        public UpdateReservationRequestValidator() {
            RuleFor(r => r.CustomerId)
                .NotEmpty();
            RuleFor(r => r.TableId)
                    .NotEmpty();
            RuleFor(r => r.PartySize)
                    .NotEmpty()
                    .GreaterThan(0);

            RuleFor(r => r.ReservationDate)
                    .GreaterThan(DateTime.UtcNow);
        }
    }
}
