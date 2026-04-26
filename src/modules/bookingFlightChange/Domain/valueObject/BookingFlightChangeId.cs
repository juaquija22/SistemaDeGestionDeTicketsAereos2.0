namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.valueObject;

// Value Object para el ID del registro de cambio de vuelo de una reserva
public sealed record BookingFlightChangeId
{
    // El valor numérico del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BookingFlightChangeId(int value) => Value = value;

    // Valida que el ID no sea negativo (0 es válido para registros nuevos)
    public static BookingFlightChangeId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("BookingFlightChangeId must be greater than or equal to 0.", nameof(value));

        return new BookingFlightChangeId(value);
    }

    public override string ToString() => Value.ToString();
}
