using RestaurantReservation.Db.Models;

namespace RestaurantReservation.Db.Respositories
{
    public class ReservationRepository : GenericRepository<Reservation>
    {
        public ReservationRepository(RestaurantReservationDbContext context)
            : base(context)
        {
        }
    }
}
