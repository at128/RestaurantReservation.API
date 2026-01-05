using RestaurantReservation.Db.Models;

namespace RestaurantReservation.Db.Respositories
{
    public class CustomerRepository : GenericRepository<Customer>
    {
        public CustomerRepository(RestaurantReservationDbContext context)
            : base(context)
        {
        }
    }
}
