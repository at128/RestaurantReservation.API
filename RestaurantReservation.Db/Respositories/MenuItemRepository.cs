using RestaurantReservation.Db.Models;

namespace RestaurantReservation.Db.Respositories
{
    public class MenuItemRepository : GenericRepository<MenuItem>
    {
        public MenuItemRepository(RestaurantReservationDbContext context)
            : base(context)
        {
        }
    }
}
