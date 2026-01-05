using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Db.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int RestaurantId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Position { get; set; } = null!;

        public Restaurant Restaurant { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
