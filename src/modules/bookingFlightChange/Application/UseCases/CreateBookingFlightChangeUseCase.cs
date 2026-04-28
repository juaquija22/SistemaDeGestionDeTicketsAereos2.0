// Caso de uso: registrar una nueva reprogramación de vuelo en el historial de una reserva
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Application.UseCases;

public sealed class CreateBookingFlightChangeUseCase
{
    private readonly IBookingFlightChangeRepository _repo;
    public CreateBookingFlightChangeUseCase(IBookingFlightChangeRepository repo) => _repo = repo;

    // Las validaciones (IDs > 0, vuelos distintos, fecha no futura) las hace el agregado
    public async Task<BookingFlightChange> ExecuteAsync(
        DateTime changeDate, string? reason,
        int idBooking, int idOldFlight, int idNewFlight, int idUser,
        CancellationToken ct = default)
    {
        var entity = BookingFlightChange.CreateNew(changeDate, reason, idBooking, idOldFlight, idNewFlight, idUser);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
