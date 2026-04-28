using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class SeedFlightWith5Seats : Migration
    {
        // IDs reservados para este seed (alejados de los IDs manuales bajos)
        private const int IdAircraft = 100;
        private const int IdFlight   = 100;

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Aeronave pequeña (5 asientos) reutilizando modelo y aerolínea demo
            migrationBuilder.Sql($"""
                INSERT IGNORE INTO `Aircraft` (`IdAircraft`, `Capacity`, `IdAirline`, `IdModel`)
                VALUES ({IdAircraft}, 5, 1, 1);
                """);

            // 5 asientos para la aeronave
            migrationBuilder.Sql($"""
                INSERT IGNORE INTO `Seat` (`IdSeat`, `IdAircraft`, `IdClase`, `Number`) VALUES
                (101, {IdAircraft}, 1, '1A'),
                (102, {IdAircraft}, 1, '2A'),
                (103, {IdAircraft}, 1, '3A'),
                (104, {IdAircraft}, 1, '4A'),
                (105, {IdAircraft}, 1, '5A');
                """);

            // Vuelo BOG→MIA, 20 dic 2026, con exactamente 5 asientos disponibles
            migrationBuilder.Sql($"""
                INSERT IGNORE INTO `Flight`
                    (`IdFlight`, `FlightNumber`, `Date`, `DepartureTime`, `ArrivalTime`,
                     `TotalCapacity`, `AvailableSeats`, `IdRoute`, `IdAircraft`, `IdStatus`, `IdCrew`, `IdFare`)
                VALUES
                    ({IdFlight}, 'DM202', '2026-12-20', '09:00:00', '15:15:00',
                     5, 5, 1, {IdAircraft}, 1, 1, 1);
                """);

            // Mapa de asientos: los 5 disponibles
            migrationBuilder.Sql($"""
                INSERT IGNORE INTO `SeatFlight` (`IdSeatFlight`, `Available`, `IdFlight`, `IdSeat`) VALUES
                (101, true, {IdFlight}, 101),
                (102, true, {IdFlight}, 102),
                (103, true, {IdFlight}, 103),
                (104, true, {IdFlight}, 104),
                (105, true, {IdFlight}, 105);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DELETE FROM `SeatFlight` WHERE `IdSeatFlight` IN (101,102,103,104,105);");
            migrationBuilder.Sql($"DELETE FROM `Flight`     WHERE `IdFlight`     = {IdFlight};");
            migrationBuilder.Sql($"DELETE FROM `Seat`       WHERE `IdSeat`       IN (101,102,103,104,105);");
            migrationBuilder.Sql($"DELETE FROM `Aircraft`   WHERE `IdAircraft`   = {IdAircraft};");
        }
    }
}
