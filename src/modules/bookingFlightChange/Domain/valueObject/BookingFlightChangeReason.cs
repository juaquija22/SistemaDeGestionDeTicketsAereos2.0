
namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.valueObject;

// Value Object para la razón del cambio de vuelo de una reserva — es opcional
public sealed record BookingFlightChangeReason
{
    // El texto de la razón (puede ser null si el cliente no especifica motivo)
    public string? Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BookingFlightChangeReason(string? value) => Value = value;

    // Valida que si existe razón, no exceda la longitud máxima permitida
    public static BookingFlightChangeReason Create(string? value)
    {
        // Si hay texto, se limita a 500 caracteres para no saturar el almacenamiento
        if (value != null && value.Trim().Length > 500)
            throw new ArgumentException("Reason cannot exceed 500 characters.", nameof(value));

        return new BookingFlightChangeReason(value?.Trim());
    }

    public override string ToString() => Value ?? string.Empty;
}
