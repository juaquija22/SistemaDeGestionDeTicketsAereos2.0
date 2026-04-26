// La lista de espera registra reservas que desean cambiar a un vuelo sin cupos disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.aggregate;

// Agregado BookingWaitList: encapsula las reglas de una entrada en la lista de espera de un vuelo
public class BookingWaitList
{
    // ID de la entrada en lista de espera (Value Object)
    public BookingWaitListId Id { get; private set; }

    // Posición de la reserva en la cola de espera del vuelo (empieza en 1)
    public BookingWaitListPosition Position { get; private set; }

    // Fecha y hora en que la reserva ingresó a la lista de espera
    public BookingWaitListRequestedAt RequestedAt { get; private set; }

    // FK a la reserva que está en espera
    public int IdBooking { get; private set; }

    // FK al vuelo deseado al que espera ser asignada la reserva
    public int IdFlight { get; private set; }

    // Estado de la entrada: En Espera (21), Promovida (22) o Expirada (23)
    public int IdStatus { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private BookingWaitList(
        BookingWaitListId id,
        BookingWaitListPosition position,
        BookingWaitListRequestedAt requestedAt,
        int idBooking,
        int idFlight,
        int idStatus)
    {
        Id = id;
        Position = position;
        RequestedAt = requestedAt;
        IdBooking = idBooking;
        IdFlight = idFlight;
        IdStatus = idStatus;
    }

    // Método de fábrica para crear o reconstruir una entrada desde la base de datos
    public static BookingWaitList Create(
        int id,
        int position,
        DateTime requestedAt,
        int idBooking,
        int idFlight,
        int idStatus)
    {
        // Regla: la entrada debe estar asociada a una reserva válida
        if (idBooking <= 0)
            throw new ArgumentException("IdBooking must be greater than 0.", nameof(idBooking));

        // Regla: la entrada debe estar asociada a un vuelo válido
        if (idFlight <= 0)
            throw new ArgumentException("IdFlight must be greater than 0.", nameof(idFlight));

        // Regla: el estado debe ser una referencia válida
        if (idStatus <= 0)
            throw new ArgumentException("IdStatus must be greater than 0.", nameof(idStatus));

        return new BookingWaitList(
            BookingWaitListId.Create(id),
            BookingWaitListPosition.Create(position),
            BookingWaitListRequestedAt.Create(requestedAt),
            idBooking,
            idFlight,
            idStatus
        );
    }

    // Método de fábrica para crear una entrada nueva (ID = 0, la BD lo asigna después)
    public static BookingWaitList CreateNew(
        int position,
        DateTime requestedAt,
        int idBooking,
        int idFlight,
        int idStatus)
        => Create(0, position, requestedAt, idBooking, idFlight, idStatus);
}
