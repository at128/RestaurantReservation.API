using Microsoft.EntityFrameworkCore;
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

            group.MapGet("/" , GetReservations);
            group.MapGet("/{id:int}", GetReservationById);
            group.MapGet("/customer/{customerId:int}", GetReservationsByCustomer);

            group.MapPost("/", Createreservation)
                .AddEndpointFilter<ValidationFilter<CreateReservationRequest>>();


            group.MapPut("/{id:int}", UpdateReservation)
                .AddEndpointFilter<ValidationFilter<UpdateReservationRequest>>();

            group.MapDelete("/{id:int}", DeleteReservation);

            return app;
        }

       

        private static async Task<IResult> UpdateReservation(int id,UpdateReservationRequest req,RestaurantReservationDbContext db ,CancellationToken ct)
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

        private static async Task<IResult> GetReservations(RestaurantReservationDbContext db,CancellationToken ct)
        {
            var reservations = await db.Reservations
                .AsNoTracking()
                .OrderByDescending(r => r.ReservationDate)
                .Select(r => new
                {
                    r.ReservationId,
                    r.CustomerId,
                    r.TableId,
                    r.PartySize,
                    r.ReservationDate
                })
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

        private static async Task<IResult> Createreservation(CreateReservationRequest req,RestaurantReservationDbContext db,CancellationToken ct)
        {
            var customerExists = await db.Customers.AnyAsync(c => c.CustomerId == req.CustomerId,ct);
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

    }
}
