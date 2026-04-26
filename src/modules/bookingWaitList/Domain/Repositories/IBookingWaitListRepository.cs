// Contrato del repositorio de lista de espera: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.Repositories;

// Interfaz que define las operaciones del repositorio de lista de espera de vuelos
public interface IBookingWaitListRepository
{
    // Busca una entrada de lista de espera por su ID
    Task<BookingWaitList?> GetByIdAsync(BookingWaitListId id, CancellationToken ct = default);

    // Retorna todas las entradas de lista de espera del sistema
    Task<IReadOnlyList<BookingWaitList>> ListAsync(CancellationToken ct = default);

    // Retorna todas las entradas de lista de espera para un vuelo específico
    Task<IReadOnlyList<BookingWaitList>> ListByFlightAsync(int idFlight, CancellationToken ct = default);

    // Retorna todas las entradas de lista de espera de una reserva específica
    Task<IReadOnlyList<BookingWaitList>> ListByBookingAsync(int idBooking, CancellationToken ct = default);

    // Retorna la primera entrada pendiente de un vuelo ordenada por posición — para promoción automática
    Task<BookingWaitList?> GetNextPendingByFlightAsync(int idFlight, int idStatusWaiting, CancellationToken ct = default);

    // Cuenta las entradas pendientes de un vuelo — para calcular la posición de la siguiente entrada
    Task<int> CountPendingByFlightAsync(int idFlight, int idStatusWaiting, CancellationToken ct = default);

    // Verifica si una reserva ya tiene una entrada activa en espera para un vuelo — evita duplicados
    Task<bool> ExistsAsync(int idBooking, int idFlight, int idStatusWaiting, CancellationToken ct = default);

    // Agrega una nueva entrada a la lista de espera
    Task AddAsync(BookingWaitList entry, CancellationToken ct = default);

    // Actualiza una entrada existente — necesario para marcar como Promovida o Expirada
    Task UpdateAsync(BookingWaitList entry, CancellationToken ct = default);

    // Elimina una entrada de la lista de espera por su ID
    Task DeleteAsync(BookingWaitListId id, CancellationToken ct = default);
}
