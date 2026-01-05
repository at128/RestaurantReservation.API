using RestaurantReservation.Db.Models;

namespace RestaurantReservation.Db.Respositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>
    {
        public OrderItemRepository(RestaurantReservationDbContext context)
            : base(context)
        {
        }
    }
}
