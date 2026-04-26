// Caso de uso: obtener todas las entradas de lista de espera de un vuelo específico
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Application.UseCases;

public sealed class GetBookingWaitListsByFlightUseCase
{
    private readonly IBookingWaitListRepository _repo;
    public GetBookingWaitListsByFlightUseCase(IBookingWaitListRepository repo) => _repo = repo;

    public Task<IReadOnlyList<BookingWaitList>> ExecuteAsync(int idFlight, CancellationToken ct = default)
        => _repo.ListByFlightAsync(idFlight, ct);
}
