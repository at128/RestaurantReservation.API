using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Db.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int CustomerId { get; set; }
        public int TableId { get; set; }
        public DateTime ReservationDate { get; set; }
        public int PartySize { get; set; }


        public Customer Customer { get; set; } = null!;
        public Table Table { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
