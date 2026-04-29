using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Db.Models.Identity
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }

        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;

        public string TokenHash { get; set; } = null!;
        public DateTime CreatedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        public DateTime? RevokedUtc { get; set; }

        public bool IsActive => RevokedUtc is null && DateTime.UtcNow < ExpiresUtc;
    }
}
