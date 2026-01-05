using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class CreateViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                CREATE VIEW vw_ReservationDetails AS
                SELECT 
                    r.ReservationId,
                    r.ReservationDate,
                    r.PartySize,
                    c.CustomerId,
                    c.FirstName AS CustomerFirstName,
                    c.LastName AS CustomerLastName,
                    c.Email AS CustomerEmail,
                    c.PhoneNumber AS CustomerPhoneNumber,
                    t.TableId,
                    t.Number AS TableNumber,
                    t.Capacity AS TableCapacity,
                    rest.RestaurantId,
                    rest.Name AS RestaurantName,
                    rest.Address AS RestaurantAddress,
                    rest.PhoneNumber AS RestaurantPhoneNumber,
                    rest.OpeningHours AS RestaurantOpeningHours
                FROM Reservations r
                INNER JOIN Customers c ON r.CustomerId = c.CustomerId
                INNER JOIN Tables t ON r.TableId = t.TableId
                INNER JOIN Restaurants rest ON t.RestaurantId = rest.RestaurantId
            ");


            migrationBuilder.Sql(@"
                CREATE VIEW vw_EmployeeDetails AS
                SELECT 
                    e.EmployeeId,
                    e.FirstName AS EmployeeFirstName,
                    e.LastName AS EmployeeLastName,
                    e.Position,
                    r.RestaurantId,
                    r.Name AS RestaurantName,
                    r.Address AS RestaurantAddress,
                    r.PhoneNumber AS RestaurantPhoneNumber,
                    r.OpeningHours AS RestaurantOpeningHours
                FROM Employees e
                INNER JOIN Restaurants r ON e.RestaurantId = r.RestaurantId
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_ReservationDetails");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_EmployeeDetails");
        }
    }
}