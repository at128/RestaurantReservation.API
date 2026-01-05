using RestaurantReservation.Db.Models;

namespace RestaurantReservation.Db.Respositories
{
    public class OrderRepository : GenericRepository<Order>
    {
        public OrderRepository(RestaurantReservationDbContext context)
            : base(context)
        {
        }
    }
}
