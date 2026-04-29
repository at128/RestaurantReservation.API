using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Db.Seeding
{
    public class AuthorizationSeed
    {
        private const int AdminRoleId = 1;
        private const int EmployeeRoleId = 2;

        private const string AdminUserId = "11111111111111111111111111111111";

        private const int P_ResCreate = 1;
        private const int P_ResRead = 2;
        private const int P_ResUpdate = 3;
        private const int P_ResDelete = 4;
        private const int P_ResOrdersRead = 5;
        private const int P_ResMenuItemsRead = 6;

        private const int P_EmpViewManagers = 7;
        private const int P_EmpViewAvgOrderAmount = 8;

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = AdminRoleId, Name = "Admin" },
                new Role { RoleId = EmployeeRoleId, Name = "User" }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionId = P_ResCreate, Name = "Reservations.Create" },
                new Permission { PermissionId = P_ResRead, Name = "Reservations.Read" },
                new Permission { PermissionId = P_ResUpdate, Name = "Reservations.Update" },
                new Permission { PermissionId = P_ResDelete, Name = "Reservations.Delete" },
                new Permission { PermissionId = P_ResOrdersRead, Name = "Reservations.Orders.Read" },
                new Permission { PermissionId = P_ResMenuItemsRead, Name = "Reservations.MenuItems.Read" },
                new Permission { PermissionId = P_EmpViewManagers, Name = "Employees.ViewManagers" },
                new Permission { PermissionId = P_EmpViewAvgOrderAmount, Name = "Employees.ViewAvgOrderAmount" }
            );

            modelBuilder.Entity<RolePermission>().HasData(
            new { RoleId = AdminRoleId, PermissionId = P_ResCreate },
            new { RoleId = AdminRoleId, PermissionId = P_ResRead },
            new { RoleId = AdminRoleId, PermissionId = P_ResUpdate },
            new { RoleId = AdminRoleId, PermissionId = P_ResDelete },
            new { RoleId = AdminRoleId, PermissionId = P_ResOrdersRead },
            new { RoleId = AdminRoleId, PermissionId = P_ResMenuItemsRead },
            new { RoleId = AdminRoleId, PermissionId = P_EmpViewManagers },
            new { RoleId = AdminRoleId, PermissionId = P_EmpViewAvgOrderAmount }
        );

            modelBuilder.Entity<RolePermission>().HasData(
            new { RoleId = EmployeeRoleId, PermissionId = P_ResRead },
            new { RoleId = EmployeeRoleId, PermissionId = P_ResOrdersRead },
            new { RoleId = EmployeeRoleId, PermissionId = P_ResMenuItemsRead }
        );

            modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = AdminUserId,
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@local.test",
                PasswordHash = PasswordHasher.Hash("Admin@12345")
            }
        );

            modelBuilder.Entity<UserRole>().HasData(
            new { UserId = AdminUserId, RoleId = AdminRoleId }
        );


        }



    }
}
