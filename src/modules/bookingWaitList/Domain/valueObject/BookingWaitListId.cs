namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.valueObject;

// Value Object para el ID del registro de lista de espera de una reserva
public sealed record BookingWaitListId
{
    // El valor numérico del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BookingWaitListId(int value) => Value = value;

    // Valida que el ID no sea negativo (0 es válido para registros nuevos)
    public static BookingWaitListId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("BookingWaitListId must be greater than or equal to 0.", nameof(value));

        return new BookingWaitListId(value);
    }

    public override string ToString() => Value.ToString();
}
