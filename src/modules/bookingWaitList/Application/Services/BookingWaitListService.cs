// Implementación del servicio de lista de espera: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Application.Services;

// Servicio de aplicación — orquesta sin lógica de dominio propia
public sealed class BookingWaitListService : IBookingWaitListService
{
    private readonly IBookingWaitListRepository _bookingWaitListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookingWaitListService(IBookingWaitListRepository bookingWaitListRepository, IUnitOfWork unitOfWork)
    {
        _bookingWaitListRepository = bookingWaitListRepository;
        _unitOfWork = unitOfWork;
    }

    // Verifica duplicado, calcula posición y persiste la entrada en lista de espera
    public async Task<BookingWaitList> CreateAsync(int idBooking, int idFlight, int idStatusWaiting, CancellationToken cancellationToken = default)
    {
        var useCase = new CreateBookingWaitListUseCase(_bookingWaitListRepository);
        var entry = await useCase.ExecuteAsync(idBooking, idFlight, idStatusWaiting, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entry;
    }

    // Busca una entrada por ID delegando directamente al repositorio
    public Task<BookingWaitList?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _bookingWaitListRepository.GetByIdAsync(BookingWaitListId.Create(id), cancellationToken);

    // Retorna todas las entradas sin filtro
    public async Task<IReadOnlyCollection<BookingWaitList>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _bookingWaitListRepository.ListAsync(cancellationToken);

    // Retorna entradas de lista de espera de un vuelo específico
    public async Task<IReadOnlyCollection<BookingWaitList>> GetByFlightAsync(int idFlight, CancellationToken cancellationToken = default)
        => await _bookingWaitListRepository.ListByFlightAsync(idFlight, cancellationToken);

    // Retorna entradas de lista de espera de una reserva específica
    public async Task<IReadOnlyCollection<BookingWaitList>> GetByBookingAsync(int idBooking, CancellationToken cancellationToken = default)
        => await _bookingWaitListRepository.ListByBookingAsync(idBooking, cancellationToken);

    // Elimina una entrada por ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entry = await _bookingWaitListRepository.GetByIdAsync(BookingWaitListId.Create(id), cancellationToken);
        if (entry is null)
            return false;

        await _bookingWaitListRepository.DeleteAsync(BookingWaitListId.Create(id), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
