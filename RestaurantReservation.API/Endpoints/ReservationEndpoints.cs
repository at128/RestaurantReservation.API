using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.Authorization;
using RestaurantReservation.API.Contracts.Reservations;
using RestaurantReservation.API.Filters;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Models;

namespace RestaurantReservation.API.Endpoints
{
    public static class ReservationEndpoints
    {
        public static IEndpointRouteBuilder MapReservationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/reservations")
                .WithTags("Reservations");

            group.MapGet("/", GetReservations)
                .RequireAuthorization(Permission.Reservations.Read)
                .WithName("GetReservations")
                .WithSummary("Retrieve all reservations")
                .WithDescription("Returns all reservations (latest first).")
                .Produces<List<ReservationResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .WithOpenApi();

            group.MapGet("/{id:int}", GetReservationById)
                .RequireAuthorization(Permission.Reservations.Read)
                .WithName("GetReservationById")
                .WithSummary("Retrieve a reservation by id")
                .WithDescription("Returns a single reservation if it exists.")
                .Produces<ReservationResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .WithOpenApi();

            group.MapGet("/customer/{customerId:int}", GetReservationsByCustomer)
                .RequireAuthorization(Permission.Reservations.Read)
                .WithName("GetReservationsByCustomer")
                .WithSummary("Retrieve reservations by customer id")
                .WithDescription("Returns all reservations for a specific customer.")
                .Produces<List<ReservationResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .WithOpenApi();

            group.MapGet("/{reservationId:int}/orders", GetReservationOrders)
                .RequireAuthorization(Permission.Reservations.Read)
                .WithName("GetReservationOrders")
                .WithSummary("Retrieve reservation orders")
                .WithDescription("Returns all orders for a reservation.")
                .Produces<List<ReservationOrderResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .WithOpenApi();

            group.MapGet("/{reservationId:int}/menu-items", GetReservationMenuItems)
                .RequireAuthorization(Permission.Reservations.Read)
                .WithName("GetReservationMenuItems")
                .WithSummary("Retrieve reservation ordered menu items")
                .WithDescription("Returns aggregated menu items ordered in a reservation.")
                .Produces<List<OrderedMenuItemResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .WithOpenApi();

            group.MapPost("/", CreateReservation)
                .AddEndpointFilter<ValidationFilter<CreateReservationRequest>>()
                .RequireAuthorization(Permission.Reservations.Create)
                .WithName("CreateReservation")
                .WithSummary("Create a reservation")
                .WithDescription("Creates a new reservation if customer/table exist and no conflict occurs.")
                .Accepts<CreateReservationRequest>("application/json")
                .Produces<ReservationResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .WithOpenApi();

            group.MapPut("/{id:int}", UpdateReservation)
                .AddEndpointFilter<ValidationFilter<UpdateReservationRequest>>()
                .RequireAuthorization(Permission.Reservations.Update)
                .WithName("UpdateReservation")
                .WithSummary("Update a reservation")
                .WithDescription("Updates an existing reservation if it exists and no conflict occurs.")
                .Accepts<UpdateReservationRequest>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .WithOpenApi();

            group.MapDelete("/{id:int}", DeleteReservation)
                .RequireAuthorization(Permission.Reservations.Delete)
                .WithName("DeleteReservation")
                .WithSummary("Delete a reservation")
                .WithDescription("Deletes a reservation by id if it exists.")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .WithOpenApi();

            return group;
        }


        private static async Task<IResult> GetReservationMenuItems(int reservationId, RestaurantReservationDbContext db, CancellationToken ct)
        {
            if (!await ReservationExists(reservationId, db, ct))
            {
                return Results.NotFound(new { message = "Reservation not found" });
            }
            var result = await db.Orders
                .AsNoTracking()
                .Where(o => o.ReservationId == reservationId)
                .SelectMany(o => o.OrderItems.Select(oi => new
                {
                    oi.MenuItemId,
                    oi.MenuItem.Name,
                    oi.MenuItem.Description,
                    oi.MenuItem.Price,
                    oi.Quantity
                }))
                .GroupBy(x => new
                {
                    x.MenuItemId,
                    x.Name,
                    x.Description,
                    x.Price
                })
                .Select(g => new OrderedMenuItemResponse(
                    g.Key.MenuItemId,
                    g.Key.Name,
                    g.Key.Description,
                    g.Key.Price,
                    g.Sum(x => x.Quantity)
                ))
                .ToListAsync(ct);

            return Results.Ok(result);
        }


        private static async Task<IResult> GetReservationOrders(int reservationId, RestaurantReservationDbContext db, CancellationToken ct)
        {
            if (!await ReservationExists(reservationId, db, ct))
            {
                return Results.NotFound(new { message = "Reservation not found" });
            }

            var orders = await db.Orders
                .AsNoTracking()
                .Where(o => o.ReservationId == reservationId)
                .OrderBy(o => o.OrderDate)
                .Select(o => new ReservationOrderResponse
                (
                    o.OrderId,
                    o.OrderDate,
                    o.TotalAmount,
                    o.EmployeeId,

                    o.OrderItems.Select(oi => new ReservationOrderItemResponse(
                        oi.OrderItemId,
                        oi.MenuItemId,
                        oi.MenuItem.Name,
                        oi.Quantity,
                        oi.UnitPrice
                        )).ToList()
                ))
                .ToListAsync(ct);

            return Results.Ok(orders);
        }

        private static async Task<IResult> UpdateReservation(int id, UpdateReservationRequest req, RestaurantReservationDbContext db, CancellationToken ct)
        {
            var existing = await db.Reservations.FirstOrDefaultAsync(r => r.ReservationId == id, ct);
            if (existing is null) return Results.NotFound();

            var customerExists = await db.Customers.AnyAsync(c => c.CustomerId == req.CustomerId, ct);
            if (!customerExists)
                return Results.NotFound(new { message = "Customer not found" });

            var table = await db.Tables.AsNoTracking().FirstOrDefaultAsync(t => t.TableId == req.TableId, ct);
            if (table is null)
                return Results.NotFound(new { message = "Table not found" });

            if (req.PartySize > table.Capacity)
                return Results.BadRequest(new { message = "PartySize exceeds table capacity" });

            var conflict = await db.Reservations.AnyAsync(r =>
                r.TableId == req.TableId &&
                r.ReservationDate == req.ReservationDate &&
                r.ReservationId != id, ct);

            if (conflict)
                return Results.Conflict(new { message = "Table already reserved at this time" });

            existing.CustomerId = req.CustomerId;
            existing.TableId = req.TableId;
            existing.PartySize = req.PartySize;
            existing.ReservationDate = req.ReservationDate;

            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        }

        private static async Task<IResult> GetReservationsByCustomer(int customerId, RestaurantReservationDbContext db, CancellationToken ct)
        {
            var list = await db.Reservations
            .AsNoTracking()
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.ReservationDate)
            .Select(r => new ReservationResponse(
                r.ReservationId,
                r.CustomerId,
                r.TableId,
                r.PartySize,
                r.ReservationDate
            ))
            .ToListAsync(ct);

            return Results.Ok(list);
        }

        private static async Task<IResult> GetReservations(RestaurantReservationDbContext db, CancellationToken ct)
        {
            var reservations = await db.Reservations
                .AsNoTracking()
                .OrderByDescending(r => r.ReservationDate)
                .Select(r => new ReservationResponse(
                    r.ReservationId,
                    r.CustomerId,
                    r.TableId,
                    r.PartySize,
                    r.ReservationDate
                    )
                )
                .ToListAsync(ct);

            return Results.Ok(reservations);

        }

        private static async Task<IResult> GetReservationById(int id, RestaurantReservationDbContext db, CancellationToken ct)
        {
            var r = await db.Reservations
                .AsNoTracking()
                .Where(x => x.ReservationId == id)
                .Select(x => new ReservationResponse(
                    x.ReservationId,
                    x.CustomerId,
                    x.TableId,
                    x.PartySize,
                    x.ReservationDate
                ))
                .FirstOrDefaultAsync(ct);

            return r is null ? Results.NotFound() : Results.Ok(r);
        }

        private static async Task<IResult> CreateReservation(CreateReservationRequest req, RestaurantReservationDbContext db, CancellationToken ct)
        {
            var customerExists = await db.Customers.AnyAsync(c => c.CustomerId == req.CustomerId, ct);
            if (!customerExists)
            {
                return Results.NotFound(new { message = "Customer not found" });
            }
            var table = await db.Tables.AsNoTracking().FirstOrDefaultAsync(t => t.TableId == req.TableId, ct);
            if (table is null)
                return Results.NotFound(new { message = "Table not found" });

            if (req.PartySize > table.Capacity)
                return Results.BadRequest(new { message = "PartySize exceeds table capacity" });

            var conflict = await db.Reservations.AnyAsync(r =>
            r.TableId == req.TableId && r.ReservationDate == req.ReservationDate, ct);

            if (conflict)
                return Results.Conflict(new { message = "Table already reserved at this time" });

            var entity = new Reservation
            {
                CustomerId = req.CustomerId,
                TableId = req.TableId,
                PartySize = req.PartySize,
                ReservationDate = req.ReservationDate
            };

            db.Reservations.Add(entity);
            await db.SaveChangesAsync(ct);

            var response = new ReservationResponse(
            entity.ReservationId,
            entity.CustomerId,
            entity.TableId,
            entity.PartySize,
            entity.ReservationDate
        );

            return Results.Created($"/api/reservations/{entity.ReservationId}", response);

        }


        private static async Task<IResult> DeleteReservation(int id, RestaurantReservationDbContext db, CancellationToken ct)
        {
            var existing = await db.Reservations.FirstOrDefaultAsync(r => r.ReservationId == id, ct);
            if (existing is null) return Results.NotFound();

            db.Reservations.Remove(existing);
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        }




        private static async Task<bool> ReservationExists(int reservationId, RestaurantReservationDbContext db, CancellationToken ct)
        {
            return await db.Reservations.AnyAsync(r => r.ReservationId == reservationId, ct);
        }
    }
}
