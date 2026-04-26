// Caso de uso: obtener todas las entradas de lista de espera del sistema
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Application.UseCases;

public sealed class GetAllBookingWaitListsUseCase
{
    private readonly IBookingWaitListRepository _repo;
    public GetAllBookingWaitListsUseCase(IBookingWaitListRepository repo) => _repo = repo;

    public Task<IReadOnlyList<BookingWaitList>> ExecuteAsync(CancellationToken ct = default)
        => _repo.ListAsync(ct);
}
