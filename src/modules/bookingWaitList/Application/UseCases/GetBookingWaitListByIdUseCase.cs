// Caso de uso: obtener una entrada de lista de espera por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Application.UseCases;

public sealed class GetBookingWaitListByIdUseCase
{
    private readonly IBookingWaitListRepository _repo;
    public GetBookingWaitListByIdUseCase(IBookingWaitListRepository repo) => _repo = repo;

    public async Task<BookingWaitList> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(BookingWaitListId.Create(id), ct);
        if (entity is null)
            throw new KeyNotFoundException($"BookingWaitList with id '{id}' was not found.");
        return entity;
    }
}
