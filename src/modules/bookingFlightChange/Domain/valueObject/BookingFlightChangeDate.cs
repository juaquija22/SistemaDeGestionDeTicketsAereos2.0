namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.valueObject;

// Value Object para la fecha en que se reprogramó el vuelo de una reserva
public sealed record BookingFlightChangeDate
{
    // Fecha y hora exacta en que se realizó el cambio de vuelo
    public DateTime Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BookingFlightChangeDate(DateTime value) => Value = value;

    // Valida que la fecha sea real y no esté en el futuro
    public static BookingFlightChangeDate Create(DateTime value)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Change date cannot be empty.", nameof(value));

        // No tiene sentido registrar un cambio de vuelo que aún no ha ocurrido
        if (value > DateTime.Now)
            throw new ArgumentException("Change date cannot be in the future.", nameof(value));

        return new BookingFlightChangeDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");
}
