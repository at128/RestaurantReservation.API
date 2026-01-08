namespace RestaurantReservation.Db.Models.Identity
{
    public class UserRole
    {
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
