using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReservation.Db.Migrations
{
    public partial class AddFunctionAndStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Database Function: total revenue by restaurant
            migrationBuilder.Sql(@"
                CREATE FUNCTION dbo.fn_GetRestaurantTotalRevenue
                (
                    @RestaurantId INT
                )
                RETURNS DECIMAL(18, 2)
                AS
                BEGIN
                    DECLARE @Total DECIMAL(18, 2);

                    SELECT @Total = SUM(o.TotalAmount)
                    FROM Orders o
                    INNER JOIN Reservations r ON o.ReservationId = r.ReservationId
                    INNER JOIN Tables t ON r.TableId = t.TableId
                    WHERE t.RestaurantId = @RestaurantId;

                    RETURN ISNULL(@Total, 0);
                END
            ");

            // 2) Stored Procedure: customers with party size > value
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetCustomersWithMinPartySize
                    @MinPartySize INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT DISTINCT c.*
                    FROM Customers c
                    INNER JOIN Reservations r ON c.CustomerId = r.CustomerId
                    WHERE r.PartySize > @MinPartySize;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetCustomersWithMinPartySize");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.fn_GetRestaurantTotalRevenue");
        }
    }
}
