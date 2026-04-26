// Registra cada reprogramación de una reserva de un vuelo a otro
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Infrastructure.Entity;

// Entidad que representa la tabla BookingFlightChange (historial de cambios de vuelo de una reserva)
public class BookingFlightChangeEntity
{
    // Clave primaria del registro de cambio de vuelo
    public int IdChange { get; set; }

    // FK a la reserva que fue reprogramada
    public int IdBooking { get; set; }

    // FK al vuelo anterior del que salió la reserva
    public int IdOldFlight { get; set; }

    // FK al vuelo nuevo al que fue asignada la reserva
    public int IdNewFlight { get; set; }

    // Fecha y hora en que se realizó el cambio de vuelo
    public DateTime ChangeDate { get; set; }

    // Razón opcional del cambio (ej: "Solicitud del cliente", "Promoción automática")
    public string? Reason { get; set; }

    // FK al usuario que ejecutó el cambio
    public int IdUser { get; set; }

    // Navegación a la reserva reprogramada
    public BookingEntity Booking { get; set; } = null!;

    // Navegación al vuelo anterior
    public FlightEntity OldFlight { get; set; } = null!;

    // Navegación al vuelo nuevo
    public FlightEntity NewFlight { get; set; } = null!;

    // Navegación al usuario que realizó el cambio
    public UserEntity User { get; set; } = null!;
}
