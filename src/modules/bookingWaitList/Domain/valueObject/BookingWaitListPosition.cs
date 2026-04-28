namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.valueObject;

// Value Object para la posición de una reserva dentro de la lista de espera de un vuelo
public sealed record BookingWaitListPosition
{
    // La posición en la cola (empieza en 1, no en 0)
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BookingWaitListPosition(int value) => Value = value;

    // Valida que la posición sea al menos 1 — no existe la posición 0 en una lista de espera
    public static BookingWaitListPosition Create(int value)
    {
        if (value < 1)
            throw new ArgumentException("Position must be at least 1.", nameof(value));

        return new BookingWaitListPosition(value);
    }

    public override string ToString() => Value.ToString();
}
