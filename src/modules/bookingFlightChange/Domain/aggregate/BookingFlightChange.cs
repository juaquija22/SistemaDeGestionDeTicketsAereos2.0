
// El historial de cambios de vuelo registra cada reprogramación de una reserva a otro vuelo
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.aggregate;

// Agregado BookingFlightChange: encapsula las reglas de un registro de reprogramación de vuelo
public class BookingFlightChange
{
    // ID del registro de cambio de vuelo (Value Object)
    public BookingFlightChangeId Id { get; private set; }

    // Fecha y hora exacta en que se realizó la reprogramación
    public BookingFlightChangeDate ChangeDate { get; private set; }

    // Razón opcional que explica el motivo del cambio de vuelo
    public BookingFlightChangeReason Reason { get; private set; }

    // FK a la reserva que fue reprogramada
    public int IdBooking { get; private set; }

    // FK al vuelo anterior (del que salió la reserva)
    public int IdOldFlight { get; private set; }

    // FK al vuelo nuevo (al que fue asignada la reserva)
    public int IdNewFlight { get; private set; }

    // FK al usuario del sistema que ejecutó el cambio
    public int IdUser { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private BookingFlightChange(
        BookingFlightChangeId id,
        BookingFlightChangeDate changeDate,
        BookingFlightChangeReason reason,
        int idBooking,
        int idOldFlight,
        int idNewFlight,
        int idUser)
    {
        Id = id;
        ChangeDate = changeDate;
        Reason = reason;
        IdBooking = idBooking;
        IdOldFlight = idOldFlight;
        IdNewFlight = idNewFlight;
        IdUser = idUser;
    }

    // Método de fábrica para crear o reconstruir un registro de cambio desde la base de datos
    public static BookingFlightChange Create(
        int id,
        DateTime changeDate,
        string? reason,
        int idBooking,
        int idOldFlight,
        int idNewFlight,
        int idUser)
    {
        // Regla: el registro debe estar asociado a una reserva válida
        if (idBooking <= 0)
            throw new ArgumentException("IdBooking must be greater than 0.", nameof(idBooking));

        // Regla: el vuelo anterior debe ser una referencia válida
        if (idOldFlight <= 0)
            throw new ArgumentException("IdOldFlight must be greater than 0.", nameof(idOldFlight));

        // Regla: el vuelo nuevo debe ser una referencia válida
        if (idNewFlight <= 0)
            throw new ArgumentException("IdNewFlight must be greater than 0.", nameof(idNewFlight));

        // Regla: no tiene sentido registrar un cambio al mismo vuelo
        if (idOldFlight == idNewFlight)
            throw new ArgumentException("Old and new flight must be different.", nameof(idNewFlight));

        // Regla: el cambio debe ser registrado por un usuario válido
        if (idUser <= 0)
            throw new ArgumentException("IdUser must be greater than 0.", nameof(idUser));

        return new BookingFlightChange(
            BookingFlightChangeId.Create(id),
            BookingFlightChangeDate.Create(changeDate),
            BookingFlightChangeReason.Create(reason),
            idBooking,
            idOldFlight,
            idNewFlight,
            idUser
        );
    }

    // Método de fábrica para crear un registro nuevo (ID = 0, la BD lo asigna después)
    public static BookingFlightChange CreateNew(
        DateTime changeDate,
        string? reason,
        int idBooking,
        int idOldFlight,
        int idNewFlight,
        int idUser)
        => Create(0, changeDate, reason, idBooking, idOldFlight, idNewFlight, idUser);
}
