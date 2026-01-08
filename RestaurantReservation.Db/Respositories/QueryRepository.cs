using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Models;
using RestaurantReservation.Db.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Db.Respositories
{
    public class QueryRepository
    {
        private readonly RestaurantReservationDbContext _context;

        public QueryRepository(RestaurantReservationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<List<Employee>> ListManagersAsync()
        {
            return await _context.Employees
                .Include(e => e.Restaurant)
                .Where(e => e.Position == "Manager")
                .ToListAsync();
        }


        public async Task<List<Reservation>> GetReservationsByCustomerAsync(int customerId)
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                    .ThenInclude(t => t.Restaurant)
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }



        public async Task<List<Order>> ListOrdersAndMenuItemsAsync(int reservationId)
        {
            return await _context.Orders
                .Where(o => o.ReservationId == reservationId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Employee)
                .Include(o => o.Reservation)
                .ToListAsync();
        }



        public async Task<List<MenuItem>> ListOrderedMenuItemsAsync(int reservationId)
        {
            return await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Where(oi => oi.Order.ReservationId == reservationId)
                .Select(oi => oi.MenuItem)
                .Distinct()
                .ToListAsync();
        }



        public async Task<decimal> CalculateAverageOrderAmountAsync(int employeeId)
        {
            var averageAmount = await _context.Orders
                .Where(o => o.EmployeeId == employeeId)
                .AverageAsync(o => (decimal?)o.TotalAmount);

            return averageAmount ?? 0;
        }


        public async Task<List<ReservationDetailsView>> GetReservationDetailsAsync()
        {
            return await _context.Set<ReservationDetailsView>()
                                 .AsNoTracking()
                                 .ToListAsync();
        }


        public async Task<List<EmployeeDetailsView>> GetEmployeeDetailsAsync()
        {
            return await _context.Set<EmployeeDetailsView>()
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<decimal> GetRestaurantTotalRevenueAsync(int restaurantId)
        {
            await using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT dbo.fn_GetRestaurantTotalRevenue(@RestaurantId)";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@RestaurantId";
            parameter.Value = restaurantId;
            command.Parameters.Add(parameter);

            var result = await command.ExecuteScalarAsync();

            return result != null && result != DBNull.Value
                ? Convert.ToDecimal(result)
                : 0m;
        }


        public async Task<List<Customer>> GetCustomersWithPartySizeGreaterThanAsync(int minPartySize)
        {
            return await _context.Customers
                .FromSqlInterpolated($"EXEC sp_GetCustomersWithMinPartySize {minPartySize}")
                .AsNoTracking()
                .ToListAsync();
        }

    }

}
