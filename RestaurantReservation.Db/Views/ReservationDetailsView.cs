using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Db.Views
{
    public class ReservationDetailsView
    {

        public int ReservationId { get; set; }
        public DateTime ReservationDate { get; set; }
        public int PartySize { get; set; }


        public int CustomerId { get; set; }
        public string CustomerFirstName { get; set; } = string.Empty;
        public string CustomerLastName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;


        public int TableId { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public int TableCapacity { get; set; }


        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public string RestaurantAddress { get; set; } = string.Empty;
        public string RestaurantPhoneNumber { get; set; } = string.Empty;
        public string RestaurantOpeningHours { get; set; } = string.Empty;


    }
}
