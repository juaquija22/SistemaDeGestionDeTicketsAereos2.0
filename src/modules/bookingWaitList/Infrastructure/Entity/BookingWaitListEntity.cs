// Representa una reserva en espera para ser asignada a un vuelo sin cupos disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Infrastructure.Entity;

// Entidad que representa la tabla BookingWaitList (lista de espera de reservas para un vuelo)
public class BookingWaitListEntity
{
    // Clave primaria de la entrada en lista de espera
    public int IdWaitList { get; set; }

    // FK a la reserva que está en espera
    public int IdBooking { get; set; }

    // FK al vuelo deseado al que espera ser asignada la reserva
    public int IdFlight { get; set; }

    // Posición en la cola de espera del vuelo (empieza en 1)
    public int Position { get; set; }

    // Fecha y hora en que la reserva ingresó a la lista de espera
    public DateTime RequestedAt { get; set; }

    // Estado de la entrada: En Espera (21), Promovida (22) o Expirada (23)
    public int IdStatus { get; set; }

    // Navegación a la reserva en espera
    public BookingEntity Booking { get; set; } = null!;

    // Navegación al vuelo deseado
    public FlightEntity Flight { get; set; } = null!;

    // Navegación al estado actual de la entrada
    public SystemStatusEntity Status { get; set; } = null!;
}
