// Caso de uso: obtener un registro de cambio de vuelo por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Application.UseCases;

public sealed class GetBookingFlightChangeByIdUseCase
{
    private readonly IBookingFlightChangeRepository _repo;
    public GetBookingFlightChangeByIdUseCase(IBookingFlightChangeRepository repo) => _repo = repo;

    public async Task<BookingFlightChange> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(BookingFlightChangeId.Create(id), ct);
        if (entity is null)
            throw new KeyNotFoundException($"BookingFlightChange with id '{id}' was not found.");
        return entity;
    }
}
