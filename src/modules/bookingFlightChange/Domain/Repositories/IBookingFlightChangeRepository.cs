// Contrato del repositorio de cambios de vuelo: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.Repositories;

// Interfaz que define las operaciones del repositorio de historial de cambios de vuelo
public interface IBookingFlightChangeRepository
{
    // Busca un registro de cambio de vuelo por su ID
    Task<BookingFlightChange?> GetByIdAsync(BookingFlightChangeId id, CancellationToken ct = default);

    // Retorna todos los registros de cambios de vuelo del sistema
    Task<IReadOnlyList<BookingFlightChange>> ListAsync(CancellationToken ct = default);

    // Retorna el historial completo de cambios de vuelo de una reserva específica
    Task<IReadOnlyList<BookingFlightChange>> ListByBookingAsync(int idBooking, CancellationToken ct = default);

    // Agrega un nuevo registro de cambio de vuelo — el historial es de solo escritura
    Task AddAsync(BookingFlightChange change, CancellationToken ct = default);

    // Elimina un registro de cambio de vuelo por su ID
    Task DeleteAsync(BookingFlightChangeId id, CancellationToken ct = default);
}
