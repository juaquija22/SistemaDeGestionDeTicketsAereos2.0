// Caso de uso: eliminar una entrada de lista de espera por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Application.UseCases;

public sealed class DeleteBookingWaitListUseCase
{
    private readonly IBookingWaitListRepository _repo;
    public DeleteBookingWaitListUseCase(IBookingWaitListRepository repo) => _repo = repo;

    // Retorna false si no existe en lugar de lanzar excepción — la UI decide cómo informarlo
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entry = await _repo.GetByIdAsync(BookingWaitListId.Create(id), ct);
        if (entry is null)
            return false;

        await _repo.DeleteAsync(BookingWaitListId.Create(id), ct);
        return true;
    }
}
