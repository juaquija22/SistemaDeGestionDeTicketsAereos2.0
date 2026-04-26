using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingFlightChangeAndWaitList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingFlightChange",
                columns: table => new
                {
                    IdChange = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdBooking = table.Column<int>(type: "int", nullable: false),
                    IdOldFlight = table.Column<int>(type: "int", nullable: false),
                    IdNewFlight = table.Column<int>(type: "int", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Reason = table.Column<string>(type: "varchar(500)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingFlightChange", x => x.IdChange);
                    table.ForeignKey(
                        name: "FK_BookingFlightChange_Booking_IdBooking",
                        column: x => x.IdBooking,
                        principalTable: "Booking",
                        principalColumn: "IdBooking",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingFlightChange_Flight_IdNewFlight",
                        column: x => x.IdNewFlight,
                        principalTable: "Flight",
                        principalColumn: "IdFlight",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingFlightChange_Flight_IdOldFlight",
                        column: x => x.IdOldFlight,
                        principalTable: "Flight",
                        principalColumn: "IdFlight",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingFlightChange_User_IdUser",
                        column: x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BookingWaitList",
                columns: table => new
                {
                    IdWaitList = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdBooking = table.Column<int>(type: "int", nullable: false),
                    IdFlight = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IdStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingWaitList", x => x.IdWaitList);
                    table.ForeignKey(
                        name: "FK_BookingWaitList_Booking_IdBooking",
                        column: x => x.IdBooking,
                        principalTable: "Booking",
                        principalColumn: "IdBooking",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingWaitList_Flight_IdFlight",
                        column: x => x.IdFlight,
                        principalTable: "Flight",
                        principalColumn: "IdFlight",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingWaitList_SystemStatus_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "SystemStatus",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "SystemStatus",
                columns: new[] { "IdStatus", "EntityType", "StatusName" },
                values: new object[,]
                {
                    { 21, "WaitList", "En Espera" },
                    { 22, "WaitList", "Promovida" },
                    { 23, "WaitList", "Expirada" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingFlightChange_IdBooking",
                table: "BookingFlightChange",
                column: "IdBooking");

            migrationBuilder.CreateIndex(
                name: "IX_BookingFlightChange_IdNewFlight",
                table: "BookingFlightChange",
                column: "IdNewFlight");

            migrationBuilder.CreateIndex(
                name: "IX_BookingFlightChange_IdOldFlight",
                table: "BookingFlightChange",
                column: "IdOldFlight");

            migrationBuilder.CreateIndex(
                name: "IX_BookingFlightChange_IdUser",
                table: "BookingFlightChange",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitList_IdBooking",
                table: "BookingWaitList",
                column: "IdBooking");

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitList_IdFlight",
                table: "BookingWaitList",
                column: "IdFlight");

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitList_IdStatus",
                table: "BookingWaitList",
                column: "IdStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingFlightChange");

            migrationBuilder.DropTable(
                name: "BookingWaitList");

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 23);
        }
    }
}
