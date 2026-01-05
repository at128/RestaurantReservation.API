using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Models;
using RestaurantReservation.Db.Respositories;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var connectionString = Environment.GetEnvironmentVariable("RestaurantReservationConStr");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("⚠️ Environment variable 'RestaurantReservationConStr' is not set.");
    Console.WriteLine("Please set it to your SQL Server connection string.");
    return;
}

// لأن DbContext عندك مكوّن بـ OnConfiguring نقدر ننشئه مباشرة
using var context = new RestaurantReservationDbContext();

// Repositories
var restaurantRepo = new RestaurantRepository(context);
var tableRepo = new TableRepository(context);
var customerRepo = new CustomerRepository(context);
var employeeRepo = new EmployeeRepository(context);
var reservationRepo = new ReservationRepository(context);
var orderRepo = new OrderRepository(context);
var orderItemRepo = new OrderItemRepository(context);
var menuItemRepo = new MenuItemRepository(context);

// Query repository (للميّزات المعقدة والـ Views / Function / SP)
var queryRepo = new QueryRepository(context);

Console.WriteLine("=== Restaurant Reservation Demo ===\n");

// نستخدم await في top-level statements
await DemoCrudForCustomerAsync(customerRepo);
await DemoQueryMethodsAsync(queryRepo);
await DemoViewsAsync(queryRepo);
await DemoDatabaseFunctionAsync(queryRepo);
await DemoStoredProcedureAsync(queryRepo);

Console.WriteLine("\nDone. Press any key to exit...");
Console.ReadKey();

// ===== Local functions =====

static async Task DemoCrudForCustomerAsync(CustomerRepository customerRepo)
{
    Console.WriteLine("---- CRUD Demo (Customer) ----");

    // Create
    var newCustomer = new Customer
    {
        FirstName = "Test",
        LastName = "Customer",
        Email = "test.customer@example.com",
        PhoneNumber = "000-000-0000"
    };

    await customerRepo.AddSync(newCustomer);
    await customerRepo.SaveChangesAsync();
    Console.WriteLine($"Created customer with ID: {newCustomer.CustomerId}");

    // Read
    var loaded = await customerRepo.GetByIdAsync(newCustomer.CustomerId);
    Console.WriteLine($"Loaded: {loaded?.FirstName} {loaded?.LastName}");

    // Update
    if (loaded != null)
    {
        loaded.PhoneNumber = "111-111-1111";
        await customerRepo.UpdateAsync(loaded);
        await customerRepo.SaveChangesAsync();
        Console.WriteLine("Updated customer phone number.");
    }

    // Delete
    if (loaded != null)
    {
        await customerRepo.DeleteAsync(loaded);
        await customerRepo.SaveChangesAsync();
        Console.WriteLine("Deleted test customer.");
    }

    Console.WriteLine();
}

static async Task DemoQueryMethodsAsync(QueryRepository queryRepo)
{
    Console.WriteLine("---- QueryRepository Demo (Tasks 10) ----");

    // 1) ListManagers
    var managers = await queryRepo.ListManagersAsync();
    Console.WriteLine("Managers:");
    foreach (var m in managers)
    {
        Console.WriteLine($"- {m.FirstName} {m.LastName} ({m.Position})");
    }

    // 2) GetReservationsByCustomer
    var customerId = 1;
    var reservations = await queryRepo.GetReservationsByCustomerAsync(customerId);
    Console.WriteLine($"\nReservations for customer {customerId}:");
    foreach (var r in reservations)
    {
        Console.WriteLine($"- Reservation {r.ReservationId} on {r.ReservationDate}, party size {r.PartySize}");
    }

    // 3) ListOrdersAndMenuItems
    var reservationId = 1;
    var orders = await queryRepo.ListOrdersAndMenuItemsAsync(reservationId);
    Console.WriteLine($"\nOrders for reservation {reservationId}:");
    foreach (var o in orders)
    {
        Console.WriteLine($"Order {o.OrderId}, total {o.TotalAmount}");
        foreach (var item in o.OrderItems)
        {
            Console.WriteLine($"   - {item.MenuItem?.Name} x{item.Quantity} @ {item.UnitPrice}");
        }
    }

    // 4) ListOrderedMenuItems
    var orderedMenuItems = await queryRepo.ListOrderedMenuItemsAsync(reservationId);
    Console.WriteLine($"\nDistinct menu items for reservation {reservationId}:");
    foreach (var mi in orderedMenuItems)
    {
        Console.WriteLine($"- {mi.Name} ({mi.Price})");
    }

    // 5) CalculateAverageOrderAmount
    var employeeId = 1;
    var avgAmount = await queryRepo.CalculateAverageOrderAmountAsync(employeeId);
    Console.WriteLine($"\nAverage order amount for employee {employeeId}: {avgAmount}");

    Console.WriteLine();
}

static async Task DemoViewsAsync(QueryRepository queryRepo)
{
    Console.WriteLine("---- Views Demo (Task 11) ----");

    // ReservationDetailsView
    var reservationDetails = await queryRepo.GetReservationDetailsAsync();
    Console.WriteLine("Reservation Details (first 3):");
    foreach (var d in reservationDetails.Take(3))
    {
        Console.WriteLine(
            $"- Reservation {d.ReservationId} for {d.CustomerFirstName} {d.CustomerLastName} " +
            $"at {d.RestaurantName} (Table {d.TableNumber})");
    }

    // EmployeeDetailsView
    var employeeDetails = await queryRepo.GetEmployeeDetailsAsync();
    Console.WriteLine("\nEmployee Details (first 3):");
    foreach (var e in employeeDetails.Take(3))
    {
        Console.WriteLine(
            $"- {e.EmployeeFirstName} {e.EmployeeLastName} ({e.Position}) at {e.RestaurantName}");
    }

    Console.WriteLine();
}

static async Task DemoDatabaseFunctionAsync(QueryRepository queryRepo)
{
    Console.WriteLine("---- Database Function Demo (Task 12) ----");

    var restaurantId = 1;
    var totalRevenue = await queryRepo.GetRestaurantTotalRevenueAsync(restaurantId);

    Console.WriteLine($"Total revenue for restaurant {restaurantId}: {totalRevenue}");

    Console.WriteLine();
}

static async Task DemoStoredProcedureAsync(QueryRepository queryRepo)
{
    Console.WriteLine("---- Stored Procedure Demo (Task 13) ----");

    var minPartySize = 3;
    var customers = await queryRepo.GetCustomersWithPartySizeGreaterThanAsync(minPartySize);

    Console.WriteLine($"Customers with reservations party size > {minPartySize}:");
    foreach (var c in customers)
    {
        Console.WriteLine($"- {c.FirstName} {c.LastName} ({c.Email})");
    }

    Console.WriteLine();
}
