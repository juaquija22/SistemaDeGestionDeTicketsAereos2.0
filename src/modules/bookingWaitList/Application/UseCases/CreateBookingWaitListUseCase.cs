// Caso de uso: agregar una reserva a la lista de espera de un vuelo sin cupos
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Application.UseCases;

public sealed class CreateBookingWaitListUseCase
{
    private readonly IBookingWaitListRepository _repo;
    public CreateBookingWaitListUseCase(IBookingWaitListRepository repo) => _repo = repo;

    // Verifica duplicado y calcula posición antes de agregar
    public async Task<BookingWaitList> ExecuteAsync(
        int idBooking, int idFlight, int idStatusWaiting,
        CancellationToken ct = default)
    {
        var alreadyWaiting = await _repo.ExistsAsync(idBooking, idFlight, idStatusWaiting, ct);
        if (alreadyWaiting)
            throw new InvalidOperationException("This booking is already in the wait list for this flight.");

        var position = await _repo.CountPendingByFlightAsync(idFlight, idStatusWaiting, ct) + 1;
        var entry = BookingWaitList.CreateNew(position, DateTime.Now, idBooking, idFlight, idStatusWaiting);
        await _repo.AddAsync(entry, ct);
        return entry;
    }
}
