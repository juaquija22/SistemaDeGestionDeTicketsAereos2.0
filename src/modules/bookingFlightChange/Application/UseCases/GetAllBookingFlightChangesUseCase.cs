// Caso de uso: obtener todos los registros de cambios de vuelo del sistema
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Application.UseCases;

public sealed class GetAllBookingFlightChangesUseCase
{
    private readonly IBookingFlightChangeRepository _repo;
    public GetAllBookingFlightChangesUseCase(IBookingFlightChangeRepository repo) => _repo = repo;

    public Task<IReadOnlyList<BookingFlightChange>> ExecuteAsync(CancellationToken ct = default)
        => _repo.ListAsync(ct);
}
