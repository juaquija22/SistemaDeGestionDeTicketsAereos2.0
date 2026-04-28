using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Infrastructure.Repositories;

public sealed class BookingFlightChangeRepository : IBookingFlightChangeRepository
{
    private readonly AppDbContext _dbContext;

    public BookingFlightChangeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingFlightChange?> GetByIdAsync(BookingFlightChangeId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingFlightChangeEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdChange == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<BookingFlightChange>> ListAsync(CancellationToken ct = default)
    {
        var entities = await _dbContext.Set<BookingFlightChangeEntity>()
            .AsNoTracking()
            .OrderBy(x => x.IdChange)
            .ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<BookingFlightChange>> ListByBookingAsync(int idBooking, CancellationToken ct = default)
    {
        var entities = await _dbContext.Set<BookingFlightChangeEntity>()
            .AsNoTracking()
            .Where(x => x.IdBooking == idBooking)
            .OrderBy(x => x.IdChange)
            .ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(BookingFlightChange change, CancellationToken ct = default)
    {
        var entity = ToEntity(change);
        await _dbContext.Set<BookingFlightChangeEntity>().AddAsync(entity, ct);
    }

    public async Task DeleteAsync(BookingFlightChangeId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingFlightChangeEntity>()
            .FirstOrDefaultAsync(x => x.IdChange == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<BookingFlightChangeEntity>().Remove(entity);
    }

    private static BookingFlightChange ToDomain(BookingFlightChangeEntity entity)
    {
        return BookingFlightChange.Create(
            entity.IdChange,
            entity.ChangeDate,
            entity.Reason,
            entity.IdBooking,
            entity.IdOldFlight,
            entity.IdNewFlight,
            entity.IdUser);
    }

    private static BookingFlightChangeEntity ToEntity(BookingFlightChange aggregate)
    {
        return new BookingFlightChangeEntity
        {
            IdChange = aggregate.Id.Value,
            IdBooking = aggregate.IdBooking,
            IdOldFlight = aggregate.IdOldFlight,
            IdNewFlight = aggregate.IdNewFlight,
            ChangeDate = aggregate.ChangeDate.Value,
            Reason = aggregate.Reason.Value,
            IdUser = aggregate.IdUser
        };
    }
}
