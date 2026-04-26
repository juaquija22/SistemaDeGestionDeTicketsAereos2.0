// Contrato del servicio de lista de espera: define las operaciones para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Application.Interfaces;

// Interfaz del servicio de lista de espera — desacopla la UI del servicio concreto
public interface IBookingWaitListService
{
    // Agrega una reserva a la lista de espera de un vuelo, verificando duplicados y calculando posición
    Task<BookingWaitList> CreateAsync(int idBooking, int idFlight, int idStatusWaiting, CancellationToken cancellationToken = default);

    // Busca una entrada por su ID, retorna null si no existe
    Task<BookingWaitList?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todas las entradas de lista de espera del sistema
    Task<IReadOnlyCollection<BookingWaitList>> GetAllAsync(CancellationToken cancellationToken = default);

    // Retorna todas las entradas de lista de espera de un vuelo específico
    Task<IReadOnlyCollection<BookingWaitList>> GetByFlightAsync(int idFlight, CancellationToken cancellationToken = default);

    // Retorna todas las entradas de lista de espera de una reserva específica
    Task<IReadOnlyCollection<BookingWaitList>> GetByBookingAsync(int idBooking, CancellationToken cancellationToken = default);

    // Elimina una entrada por su ID, retorna false si no existe en lugar de lanzar excepción
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
