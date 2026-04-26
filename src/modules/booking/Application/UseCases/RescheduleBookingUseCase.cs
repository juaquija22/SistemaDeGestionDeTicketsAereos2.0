// Caso de uso central: reprogramar una reserva a otro vuelo compatible o ingresarla a lista de espera
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;

public sealed class RescheduleBookingUseCase
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IFlightRepository _flightRepo;
    private readonly IBookingFlightChangeRepository _changeRepo;
    private readonly IBookingWaitListRepository _waitListRepo;
    private readonly IUnitOfWork _unitOfWork;

    public RescheduleBookingUseCase(
        IBookingRepository bookingRepo,
        IFlightRepository flightRepo,
        IBookingFlightChangeRepository changeRepo,
        IBookingWaitListRepository waitListRepo,
        IUnitOfWork unitOfWork)
    {
        _bookingRepo = bookingRepo;
        _flightRepo = flightRepo;
        _changeRepo = changeRepo;
        _waitListRepo = waitListRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<RescheduleResult> ExecuteAsync(
        int idBooking,
        int idNewFlight,
        int idStatusConfirmed,
        int idStatusPaid,
        int idStatusWaiting,
        int idUser,
        string? reason,
        CancellationToken ct = default)
    {
        // 1. Cargar la reserva
        var booking = await _bookingRepo.GetByIdAsync(BookingId.Create(idBooking), ct)
            ?? throw new KeyNotFoundException($"Booking with id '{idBooking}' was not found.");

        // 2. Solo se pueden reprogramar reservas confirmadas o pagadas
        if (booking.IdStatus != idStatusConfirmed && booking.IdStatus != idStatusPaid)
            throw new InvalidOperationException("Only confirmed or paid bookings can be rescheduled.");

        // 3. Validar que el vuelo nuevo sea diferente al actual
        if (booking.IdFlight == idNewFlight)
            throw new InvalidOperationException("The new flight must be different from the current one.");

        // 4. Cargar vuelo actual y vuelo nuevo
        var oldFlight = await _flightRepo.GetByIdAsync(FlightId.Create(booking.IdFlight), ct)
            ?? throw new KeyNotFoundException($"Current flight with id '{booking.IdFlight}' was not found.");

        var newFlight = await _flightRepo.GetByIdAsync(FlightId.Create(idNewFlight), ct)
            ?? throw new KeyNotFoundException($"New flight with id '{idNewFlight}' was not found.");

        // 5. Validar compatibilidad de ruta: mismo origen y destino
        if (oldFlight.IdRoute != newFlight.IdRoute)
            throw new InvalidOperationException("Flights are incompatible: the new flight must have the same origin and destination.");

        // 6. Evaluar disponibilidad de cupos en el vuelo nuevo
        if (newFlight.AvailableSeats.Value >= booking.SeatCount.Value)
        {
            // 7a. Hay cupo: reprogramar la reserva al vuelo nuevo
            var newFlightDate = newFlight.Date.Value.ToDateTime(newFlight.DepartureTime.Value);
            var updatedBooking = Booking.Create(
                booking.Id.Value, booking.Code.Value,
                newFlightDate, booking.CreationDate.Value,
                booking.SeatCount.Value, booking.Observations.Value,
                idNewFlight, booking.IdStatus);
            await _bookingRepo.UpdateAsync(updatedBooking, ct);

            // 7b. Liberar cupos en el vuelo anterior
            var updatedOldFlight = Flight.Create(
                oldFlight.Id.Value, oldFlight.Number.Value,
                oldFlight.Date.Value, oldFlight.DepartureTime.Value, oldFlight.ArrivalTime.Value,
                oldFlight.TotalCapacity.Value, oldFlight.AvailableSeats.Value + booking.SeatCount.Value,
                oldFlight.IdRoute, oldFlight.IdAircraft, oldFlight.IdStatus, oldFlight.IdCrew, oldFlight.IdFare);
            await _flightRepo.UpdateAsync(updatedOldFlight, ct);

            // 7c. Ocupar cupos en el vuelo nuevo
            var updatedNewFlight = Flight.Create(
                newFlight.Id.Value, newFlight.Number.Value,
                newFlight.Date.Value, newFlight.DepartureTime.Value, newFlight.ArrivalTime.Value,
                newFlight.TotalCapacity.Value, newFlight.AvailableSeats.Value - booking.SeatCount.Value,
                newFlight.IdRoute, newFlight.IdAircraft, newFlight.IdStatus, newFlight.IdCrew, newFlight.IdFare);
            await _flightRepo.UpdateAsync(updatedNewFlight, ct);

            // 7d. Registrar el cambio en el historial
            var change = BookingFlightChange.CreateNew(
                DateTime.Now, reason, idBooking,
                booking.IdFlight, idNewFlight, idUser);
            await _changeRepo.AddAsync(change, ct);

            await _unitOfWork.SaveChangesAsync(ct);
            return RescheduleResult.Rescheduled;
        }
        else
        {
            // 8a. Sin cupo: verificar que no esté ya en lista de espera para ese vuelo
            var alreadyWaiting = await _waitListRepo.ExistsAsync(idBooking, idNewFlight, idStatusWaiting, ct);
            if (alreadyWaiting)
                throw new InvalidOperationException("This booking is already in the wait list for this flight.");

            // 8b. Calcular posición y agregar a lista de espera
            var position = await _waitListRepo.CountPendingByFlightAsync(idNewFlight, idStatusWaiting, ct) + 1;
            var entry = BookingWaitList.CreateNew(position, DateTime.Now, idBooking, idNewFlight, idStatusWaiting);
            await _waitListRepo.AddAsync(entry, ct);

            await _unitOfWork.SaveChangesAsync(ct);
            return RescheduleResult.WaitListed(position);
        }
    }
}

// Resultado tipado que evita el uso de bool ambiguo para comunicar el outcome al UI
public sealed class RescheduleResult
{
    public bool IsRescheduled { get; }
    public int? WaitListPosition { get; }

    private RescheduleResult(bool isRescheduled, int? position)
    {
        IsRescheduled = isRescheduled;
        WaitListPosition = position;
    }

    public static RescheduleResult Rescheduled => new(true, null);
    public static RescheduleResult WaitListed(int position) => new(false, position);
}
