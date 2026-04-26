namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.valueObject;

// Value Object para la fecha en que una reserva ingresó a la lista de espera
public sealed record BookingWaitListRequestedAt
{
    // Fecha y hora exacta en que se solicitó el ingreso a la lista de espera
    public DateTime Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BookingWaitListRequestedAt(DateTime value) => Value = value;

    // Valida que la fecha sea real y no esté en el futuro
    public static BookingWaitListRequestedAt Create(DateTime value)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Requested date cannot be empty.", nameof(value));

        // No tiene sentido registrar una solicitud de espera que aún no ha ocurrido
        if (value > DateTime.Now)
            throw new ArgumentException("Requested date cannot be in the future.", nameof(value));

        return new BookingWaitListRequestedAt(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");
}
