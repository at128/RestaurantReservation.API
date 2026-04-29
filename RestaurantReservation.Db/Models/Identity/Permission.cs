namespace RestaurantReservation.Db.Models.Identity
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string Name { get; set; } = null!; 
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
