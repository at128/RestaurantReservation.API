namespace RestaurantReservation.Db.Models.Identity
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
