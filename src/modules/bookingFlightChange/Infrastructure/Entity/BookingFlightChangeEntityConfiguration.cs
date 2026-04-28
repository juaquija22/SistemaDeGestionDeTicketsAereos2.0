using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingFlightChange.Infrastructure.Entity;

// Configuración de EF Core para mapear BookingFlightChangeEntity a la tabla BookingFlightChange
public sealed class BookingFlightChangeEntityConfiguration : IEntityTypeConfiguration<BookingFlightChangeEntity>
{
    public void Configure(EntityTypeBuilder<BookingFlightChangeEntity> builder)
    {
        builder.ToTable("BookingFlightChange");

        // Clave primaria
        builder.HasKey(x => x.IdChange);

        // El ID se genera automáticamente
        builder.Property(x => x.IdChange)
            .HasColumnName("IdChange")
            .ValueGeneratedOnAdd();

        // FK a la reserva, obligatoria
        builder.Property(x => x.IdBooking)
            .HasColumnName("IdBooking")
            .IsRequired();

        // FK al vuelo anterior, obligatoria
        builder.Property(x => x.IdOldFlight)
            .HasColumnName("IdOldFlight")
            .IsRequired();

        // FK al vuelo nuevo, obligatoria
        builder.Property(x => x.IdNewFlight)
            .HasColumnName("IdNewFlight")
            .IsRequired();

        // Fecha del cambio, se registra con la hora actual
        builder.Property(x => x.ChangeDate)
            .HasColumnName("ChangeDate")
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        // Razón opcional del cambio, máximo 500 caracteres
        builder.Property(x => x.Reason)
            .HasColumnName("Reason")
            .HasColumnType("varchar(500)");

        // FK al usuario que ejecutó el cambio, obligatoria
        builder.Property(x => x.IdUser)
            .HasColumnName("IdUser")
            .IsRequired();

        // Relación: el registro pertenece a una reserva, una reserva puede tener varios cambios de vuelo
        builder.HasOne(x => x.Booking)
            .WithMany(x => x.BookingFlightChanges)
            .HasForeignKey(x => x.IdBooking)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con vuelo anterior — EF necesita WithMany explícito porque Flight tiene dos FKs aquí
        builder.HasOne(x => x.OldFlight)
            .WithMany(x => x.OldFlightChanges)
            .HasForeignKey(x => x.IdOldFlight)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con vuelo nuevo — colección separada para evitar ambigüedad con OldFlight
        builder.HasOne(x => x.NewFlight)
            .WithMany(x => x.NewFlightChanges)
            .HasForeignKey(x => x.IdNewFlight)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: cada cambio fue ejecutado por un usuario del sistema
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.IdUser)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
