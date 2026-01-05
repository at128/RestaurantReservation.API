using RestaurantReservation.Db.Models;

namespace RestaurantReservation.Db.Respositories
{
    public class RestaurantRepository : GenericRepository<Restaurant>
    {
        public RestaurantRepository(RestaurantReservationDbContext context)
            : base(context)
        {
        }
    }
}
