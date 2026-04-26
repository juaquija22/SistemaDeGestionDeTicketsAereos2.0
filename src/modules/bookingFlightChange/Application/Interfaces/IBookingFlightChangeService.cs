// Contrato del servicio de historial de cambios de vuelo: define las operaciones para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Application.Interfaces;

// Interfaz del servicio de historial de cambios de vuelo — desacopla la UI del servicio concreto
public interface IBookingFlightChangeService
{
    // Registra una nueva reprogramación de vuelo en el historial de una reserva
    Task<BookingFlightChange> CreateAsync(DateTime changeDate, string? reason, int idBooking, int idOldFlight, int idNewFlight, int idUser, CancellationToken cancellationToken = default);

    // Busca un registro por su ID, retorna null si no existe
    Task<BookingFlightChange?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los registros de cambios de vuelo del sistema
    Task<IReadOnlyCollection<BookingFlightChange>> GetAllAsync(CancellationToken cancellationToken = default);

    // Retorna el historial completo de cambios de vuelo de una reserva específica
    Task<IReadOnlyCollection<BookingFlightChange>> GetByBookingAsync(int idBooking, CancellationToken cancellationToken = default);
}
