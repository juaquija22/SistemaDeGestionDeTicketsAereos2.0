using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Infrastructure.Repositories;

public sealed class BookingWaitListRepository : IBookingWaitListRepository
{
    private readonly AppDbContext _dbContext;

    public BookingWaitListRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingWaitList?> GetByIdAsync(BookingWaitListId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingWaitListEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdWaitList == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<BookingWaitList>> ListAsync(CancellationToken ct = default)
    {
        var entities = await _dbContext.Set<BookingWaitListEntity>()
            .AsNoTracking()
            .OrderBy(x => x.IdWaitList)
            .ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<BookingWaitList>> ListByFlightAsync(int idFlight, CancellationToken ct = default)
    {
        var entities = await _dbContext.Set<BookingWaitListEntity>()
            .AsNoTracking()
            .Where(x => x.IdFlight == idFlight)
            .OrderBy(x => x.Position)
            .ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<BookingWaitList>> ListByBookingAsync(int idBooking, CancellationToken ct = default)
    {
        var entities = await _dbContext.Set<BookingWaitListEntity>()
            .AsNoTracking()
            .Where(x => x.IdBooking == idBooking)
            .OrderBy(x => x.IdWaitList)
            .ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<BookingWaitList?> GetNextPendingByFlightAsync(int idFlight, int idStatusWaiting, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingWaitListEntity>()
            .AsNoTracking()
            .Where(x => x.IdFlight == idFlight && x.IdStatus == idStatusWaiting)
            .OrderBy(x => x.Position)
            .FirstOrDefaultAsync(ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<int> CountPendingByFlightAsync(int idFlight, int idStatusWaiting, CancellationToken ct = default)
    {
        return await _dbContext.Set<BookingWaitListEntity>()
            .CountAsync(x => x.IdFlight == idFlight && x.IdStatus == idStatusWaiting, ct);
    }

    public async Task<bool> ExistsAsync(int idBooking, int idFlight, int idStatusWaiting, CancellationToken ct = default)
    {
        return await _dbContext.Set<BookingWaitListEntity>()
            .AnyAsync(x => x.IdBooking == idBooking && x.IdFlight == idFlight && x.IdStatus == idStatusWaiting, ct);
    }

    public async Task AddAsync(BookingWaitList entry, CancellationToken ct = default)
    {
        var entity = ToEntity(entry);
        await _dbContext.Set<BookingWaitListEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(BookingWaitList entry, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingWaitListEntity>()
            .FirstOrDefaultAsync(x => x.IdWaitList == entry.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException($"BookingWaitList with id '{entry.Id.Value}' was not found.");
        }

        entity.Position = entry.Position.Value;
        entity.IdStatus = entry.IdStatus;
    }

    public async Task DeleteAsync(BookingWaitListId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingWaitListEntity>()
            .FirstOrDefaultAsync(x => x.IdWaitList == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<BookingWaitListEntity>().Remove(entity);
    }

    private static BookingWaitList ToDomain(BookingWaitListEntity entity)
    {
        return BookingWaitList.Create(
            entity.IdWaitList,
            entity.Position,
            entity.RequestedAt,
            entity.IdBooking,
            entity.IdFlight,
            entity.IdStatus);
    }

    private static BookingWaitListEntity ToEntity(BookingWaitList aggregate)
    {
        return new BookingWaitListEntity
        {
            IdWaitList = aggregate.Id.Value,
            IdBooking = aggregate.IdBooking,
            IdFlight = aggregate.IdFlight,
            Position = aggregate.Position.Value,
            RequestedAt = aggregate.RequestedAt.Value,
            IdStatus = aggregate.IdStatus
        };
    }
}
