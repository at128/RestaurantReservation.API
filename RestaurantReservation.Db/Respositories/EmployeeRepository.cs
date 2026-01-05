using RestaurantReservation.Db.Models;

namespace RestaurantReservation.Db.Respositories
{
    public class EmployeeRepository : GenericRepository<Employee>
    {
        public EmployeeRepository(RestaurantReservationDbContext context)
            : base(context)
        {
        }
    }
}
