using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Models;
using RestaurantReservation.Db.Models.Identity;
using RestaurantReservation.Db.Seeding;
using RestaurantReservation.Db.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Db
{
    public class RestaurantReservationDbContext: DbContext
    {

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public RestaurantReservationDbContext(DbContextOptions<RestaurantReservationDbContext> options)
    : base(options)
        {
        }

        public RestaurantReservationDbContext()
    : base()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var cs = Environment.GetEnvironmentVariable("RestaurantReservationConStr");

                if (string.IsNullOrWhiteSpace(cs))
                    throw new InvalidOperationException(
                        "Missing connection string. Set env var: RestaurantReservationConStr"
                    );

                optionsBuilder.UseSqlServer(cs);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            AuthorizationSeed.Seed(modelBuilder);

            modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);


            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(mi => mi.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId);


            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Orders)
                .WithOne(Orders => Orders.Employee)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Table>()
                .HasMany(t => t.Reservations)
                .WithOne(r => r.Table)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuItem>()
                .HasMany(mi => mi.OrderItems)
                .WithOne(oi => oi.MenuItem)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ReservationDetailsView>(entity =>
            {
                entity.ToView("vw_ReservationDetails");
                entity.HasNoKey(); 
            });

            modelBuilder.Entity<EmployeeDetailsView>(entity =>
            {
                entity.ToView("vw_EmployeeDetails");
                entity.HasNoKey(); 
            });


            modelBuilder.Entity<Restaurant>().HasData(
        new Restaurant
        {
            RestaurantId = 1,
            Name = "Downtown Diner",
            Address = "123 Main St",
            PhoneNumber = "111-111-1111",
            OpeningHours = "10:00 - 22:00"
        },
        new Restaurant
        {
            RestaurantId = 2,
            Name = "Sea Breeze Grill",
            Address = "45 Beach Road",
            PhoneNumber = "222-222-2222",
            OpeningHours = "12:00 - 23:00"
        },
        new Restaurant
        {
            RestaurantId = 3,
            Name = "Mountain View Steakhouse",
            Address = "89 Hill Ave",
            PhoneNumber = "333-333-3333",
            OpeningHours = "17:00 - 23:30"
        },
        new Restaurant
        {
            RestaurantId = 4,
            Name = "City Lights Café",
            Address = "7 Downtown Plaza",
            PhoneNumber = "444-444-4444",
            OpeningHours = "08:00 - 20:00"
        },
        new Restaurant
        {
            RestaurantId = 5,
            Name = "Green Garden Vegan",
            Address = "12 Park Lane",
            PhoneNumber = "555-555-5555",
            OpeningHours = "11:00 - 21:00"
        }
    );

            // ----- Tables -----
            modelBuilder.Entity<Table>().HasData(
                new Table { TableId = 1, RestaurantId = 1, Capacity = 2, Number = "T1" },
                new Table { TableId = 2, RestaurantId = 1, Capacity = 4, Number = "T2" },
                new Table { TableId = 3, RestaurantId = 2, Capacity = 4, Number = "T3" },
                new Table { TableId = 4, RestaurantId = 3, Capacity = 6, Number = "T4" },
                new Table { TableId = 5, RestaurantId = 4, Capacity = 2, Number = "T5" }
            );

            // ----- Customers -----
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    CustomerId = 1,
                    FirstName = "Omar",
                    LastName = "Ali",
                    Email = "omar.ali@example.com",
                    PhoneNumber = "970-555-0001"
                },
                new Customer
                {
                    CustomerId = 2,
                    FirstName = "Sara",
                    LastName = "Hassan",
                    Email = "sara.hassan@example.com",
                    PhoneNumber = "970-555-0002"
                },
                new Customer
                {
                    CustomerId = 3,
                    FirstName = "Khaled",
                    LastName = "Yousef",
                    Email = "khaled.yousef@example.com",
                    PhoneNumber = "970-555-0003"
                },
                new Customer
                {
                    CustomerId = 4,
                    FirstName = "Lina",
                    LastName = "Saleh",
                    Email = "lina.saleh@example.com",
                    PhoneNumber = "970-555-0004"
                },
                new Customer
                {
                    CustomerId = 5,
                    FirstName = "Mahmoud",
                    LastName = "Nasser",
                    Email = "mahmoud.nasser@example.com",
                    PhoneNumber = "970-555-0005"
                }
            );

            // ----- Employees -----
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeId = 1,
                    RestaurantId = 1,
                    FirstName = "Ali",
                    LastName = "Hamad",
                    Position = "Waiter"
                },
                new Employee
                {
                    EmployeeId = 2,
                    RestaurantId = 1,
                    FirstName = "Nada",
                    LastName = "Samir",
                    Position = "Manager"
                },
                new Employee
                {
                    EmployeeId = 3,
                    RestaurantId = 2,
                    FirstName = "Yousef",
                    LastName = "Odeh",
                    Position = "Waiter"
                },
                new Employee
                {
                    EmployeeId = 4,
                    RestaurantId = 3,
                    FirstName = "Mona",
                    LastName = "Kamal",
                    Position = "Chef"
                },
                new Employee
                {
                    EmployeeId = 5,
                    RestaurantId = 4,
                    FirstName = "Hadi",
                    LastName = "Saeed",
                    Position = "Barista"
                }
            );

            // ----- MenuItems -----
            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem
                {
                    MenuItemId = 1,
                    RestaurantId = 1,
                    Name = "Margherita Pizza",
                    Description = "Classic cheese pizza with tomato sauce",
                    Price = 8.50m
                },
                new MenuItem
                {
                    MenuItemId = 2,
                    RestaurantId = 1,
                    Name = "Cheeseburger",
                    Description = "Grilled beef burger with cheese",
                    Price = 10.00m
                },
                new MenuItem
                {
                    MenuItemId = 3,
                    RestaurantId = 2,
                    Name = "Grilled Salmon",
                    Description = "Salmon fillet with lemon butter",
                    Price = 18.00m
                },
                new MenuItem
                {
                    MenuItemId = 4,
                    RestaurantId = 3,
                    Name = "Ribeye Steak",
                    Description = "Juicy ribeye steak",
                    Price = 25.00m
                },
                new MenuItem
                {
                    MenuItemId = 5,
                    RestaurantId = 4,
                    Name = "Cappuccino",
                    Description = "Espresso with steamed milk and foam",
                    Price = 4.00m
                }
            );

            // ----- Reservations -----
            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ReservationId = 1,
                    CustomerId = 1,
                    TableId = 1,
                    ReservationDate = new DateTime(2025, 1, 10, 19, 0, 0),
                    PartySize = 2
                },
                new Reservation
                {
                    ReservationId = 2,
                    CustomerId = 2,
                    TableId = 2,
                    ReservationDate = new DateTime(2025, 1, 11, 20, 0, 0),
                    PartySize = 4
                },
                new Reservation
                {
                    ReservationId = 3,
                    CustomerId = 3,
                    TableId = 3,
                    ReservationDate = new DateTime(2025, 1, 12, 18, 30, 0),
                    PartySize = 3
                },
                new Reservation
                {
                    ReservationId = 4,
                    CustomerId = 4,
                    TableId = 4,
                    ReservationDate = new DateTime(2025, 1, 13, 21, 0, 0),
                    PartySize = 5
                },
                new Reservation
                {
                    ReservationId = 5,
                    CustomerId = 5,
                    TableId = 5,
                    ReservationDate = new DateTime(2025, 1, 14, 19, 30, 0),
                    PartySize = 2
                }
            );

            // ----- Orders -----
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    OrderId = 1,
                    ReservationId = 1,
                    EmployeeId = 1,
                    OrderDate = new DateTime(2025, 1, 10, 19, 15, 0),
                    TotalAmount = 25.00m
                },
                new Order
                {
                    OrderId = 2,
                    ReservationId = 2,
                    EmployeeId = 2,
                    OrderDate = new DateTime(2025, 1, 11, 20, 15, 0),
                    TotalAmount = 40.00m
                },
                new Order
                {
                    OrderId = 3,
                    ReservationId = 3,
                    EmployeeId = 3,
                    OrderDate = new DateTime(2025, 1, 12, 18, 45, 0),
                    TotalAmount = 30.00m
                },
                new Order
                {
                    OrderId = 4,
                    ReservationId = 4,
                    EmployeeId = 4,
                    OrderDate = new DateTime(2025, 1, 13, 21, 15, 0),
                    TotalAmount = 50.00m
                },
                new Order
                {
                    OrderId = 5,
                    ReservationId = 5,
                    EmployeeId = 5,
                    OrderDate = new DateTime(2025, 1, 14, 19, 45, 0),
                    TotalAmount = 15.00m
                }
            );

            // ----- OrderItems -----
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem
                {
                    OrderItemId = 1,
                    OrderId = 1,
                    MenuItemId = 1,
                    Quantity = 1,
                    UnitPrice = 8.50m
                },
                new OrderItem
                {
                    OrderItemId = 2,
                    OrderId = 1,
                    MenuItemId = 2,
                    Quantity = 1,
                    UnitPrice = 10.00m
                },
                new OrderItem
                {
                    OrderItemId = 3,
                    OrderId = 3,
                    MenuItemId = 3,
                    Quantity = 2,
                    UnitPrice = 18.00m
                },
                new OrderItem
                {
                    OrderItemId = 4,
                    OrderId = 4,
                    MenuItemId = 4,
                    Quantity = 2,
                    UnitPrice = 25.00m
                },
                new OrderItem
                {
                    OrderItemId = 5,
                    OrderId = 5,
                    MenuItemId = 5,
                    Quantity = 1,
                    UnitPrice = 4.00m
                }
            );

        }
    }
}
