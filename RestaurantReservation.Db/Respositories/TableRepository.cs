using RestaurantReservation.Db.Models;

namespace RestaurantReservation.Db.Respositories
{
    public class TableRepository : GenericRepository<Table>
    {
        public TableRepository(RestaurantReservationDbContext context)
            : base(context)
        {
        }
    }
}
