// Caso de uso: promover automáticamente la primera reserva en espera cuando se libera un cupo
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;

public sealed class PromoteFromWaitListUseCase
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IFlightRepository _flightRepo;
    private readonly IBookingFlightChangeRepository _changeRepo;
    private readonly IBookingWaitListRepository _waitListRepo;
    private readonly IUnitOfWork _unitOfWork;

    public PromoteFromWaitListUseCase(
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

    // Se llama después de que una reserva es cancelada para reasignar el cupo liberado
    // idFlight: vuelo que liberó el cupo — se buscan entradas en espera para ese vuelo
    // Retorna true si se promovió alguna reserva, false si no había nadie en espera
    public async Task<bool> ExecuteAsync(
        int idFlight,
        int idStatusWaiting,
        int idStatusPromoted,
        int idStatusConfirmed,
        int idStatusPaid,
        int idUser,
        CancellationToken ct = default)
    {
        // 1. Obtener la primera entrada pendiente en la lista de espera para este vuelo
        var entry = await _waitListRepo.GetNextPendingByFlightAsync(idFlight, idStatusWaiting, ct);
        if (entry is null)
            return false;

        // 2. Cargar la reserva asociada a esa entrada
        var booking = await _bookingRepo.GetByIdAsync(BookingId.Create(entry.IdBooking), ct);
        if (booking is null)
            return false;

        // 3. Verificar que la reserva siga activa (no cancelada mientras esperaba)
        if (booking.IdStatus != idStatusConfirmed && booking.IdStatus != idStatusPaid)
            return false;

        // 4. Cargar el vuelo deseado y verificar que haya cupos suficientes
        var desiredFlight = await _flightRepo.GetByIdAsync(FlightId.Create(idFlight), ct);
        if (desiredFlight is null || desiredFlight.AvailableSeats.Value < booking.SeatCount.Value)
            return false;

        // 5. Cargar el vuelo actual de la reserva para liberar sus cupos
        var currentFlight = await _flightRepo.GetByIdAsync(FlightId.Create(booking.IdFlight), ct);

        // 6. Reprogramar la reserva al vuelo deseado
        var newFlightDate = desiredFlight.Date.Value.ToDateTime(desiredFlight.DepartureTime.Value);
        var updatedBooking = Booking.Create(
            booking.Id.Value, booking.Code.Value,
            newFlightDate, booking.CreationDate.Value,
            booking.SeatCount.Value, booking.Observations.Value,
            idFlight, booking.IdStatus);
        await _bookingRepo.UpdateAsync(updatedBooking, ct);

        // 7. Liberar cupos en el vuelo actual de la reserva (si es distinto al deseado)
        if (currentFlight is not null && currentFlight.Id.Value != idFlight)
        {
            var updatedCurrentFlight = Flight.Create(
                currentFlight.Id.Value, currentFlight.Number.Value,
                currentFlight.Date.Value, currentFlight.DepartureTime.Value, currentFlight.ArrivalTime.Value,
                currentFlight.TotalCapacity.Value, currentFlight.AvailableSeats.Value + booking.SeatCount.Value,
                currentFlight.IdRoute, currentFlight.IdAircraft, currentFlight.IdStatus,
                currentFlight.IdCrew, currentFlight.IdFare);
            await _flightRepo.UpdateAsync(updatedCurrentFlight, ct);
        }

        // 8. Ocupar cupos en el vuelo deseado
        var updatedDesiredFlight = Flight.Create(
            desiredFlight.Id.Value, desiredFlight.Number.Value,
            desiredFlight.Date.Value, desiredFlight.DepartureTime.Value, desiredFlight.ArrivalTime.Value,
            desiredFlight.TotalCapacity.Value, desiredFlight.AvailableSeats.Value - booking.SeatCount.Value,
            desiredFlight.IdRoute, desiredFlight.IdAircraft, desiredFlight.IdStatus,
            desiredFlight.IdCrew, desiredFlight.IdFare);
        await _flightRepo.UpdateAsync(updatedDesiredFlight, ct);

        // 9. Marcar la entrada de lista de espera como Promovida
        var promoted = BookingWaitList.Create(
            entry.Id.Value, entry.Position.Value,
            entry.RequestedAt.Value, entry.IdBooking,
            entry.IdFlight, idStatusPromoted);
        await _waitListRepo.UpdateAsync(promoted, ct);

        // 10. Registrar el cambio en el historial con motivo automático
        var change = BookingFlightChange.CreateNew(
            DateTime.Now, "Promoción automática desde lista de espera",
            entry.IdBooking, booking.IdFlight, idFlight, idUser);
        await _changeRepo.AddAsync(change, ct);

        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
