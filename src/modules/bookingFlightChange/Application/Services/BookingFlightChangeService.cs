// Implementación del servicio de historial de cambios de vuelo: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Application.Services;

// Servicio de aplicación — orquesta sin lógica de dominio propia
public sealed class BookingFlightChangeService : IBookingFlightChangeService
{
    private readonly IBookingFlightChangeRepository _bookingFlightChangeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookingFlightChangeService(IBookingFlightChangeRepository bookingFlightChangeRepository, IUnitOfWork unitOfWork)
    {
        _bookingFlightChangeRepository = bookingFlightChangeRepository;
        _unitOfWork = unitOfWork;
    }

    // Registra el cambio de vuelo y lo persiste inmediatamente
    public async Task<BookingFlightChange> CreateAsync(DateTime changeDate, string? reason, int idBooking, int idOldFlight, int idNewFlight, int idUser, CancellationToken cancellationToken = default)
    {
        var entity = BookingFlightChange.CreateNew(changeDate, reason, idBooking, idOldFlight, idNewFlight, idUser);
        await _bookingFlightChangeRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un registro por ID delegando directamente al repositorio
    public Task<BookingFlightChange?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _bookingFlightChangeRepository.GetByIdAsync(BookingFlightChangeId.Create(id), cancellationToken);

    // Retorna todos los cambios de vuelo sin filtro
    public async Task<IReadOnlyCollection<BookingFlightChange>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _bookingFlightChangeRepository.ListAsync(cancellationToken);

    // Retorna el historial de cambios de vuelo de una reserva específica
    public async Task<IReadOnlyCollection<BookingFlightChange>> GetByBookingAsync(int idBooking, CancellationToken cancellationToken = default)
        => await _bookingFlightChangeRepository.ListByBookingAsync(idBooking, cancellationToken);
}
