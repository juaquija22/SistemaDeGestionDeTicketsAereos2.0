// Caso de uso: obtener el historial de cambios de vuelo de una reserva específica
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Application.UseCases;

public sealed class GetBookingFlightChangesByBookingUseCase
{
    private readonly IBookingFlightChangeRepository _repo;
    public GetBookingFlightChangesByBookingUseCase(IBookingFlightChangeRepository repo) => _repo = repo;

    public Task<IReadOnlyList<BookingFlightChange>> ExecuteAsync(int idBooking, CancellationToken ct = default)
        => _repo.ListByBookingAsync(idBooking, ct);
}
