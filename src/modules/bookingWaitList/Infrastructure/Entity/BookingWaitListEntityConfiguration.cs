using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingWaitList.Infrastructure.Entity;

// Configuración de EF Core para mapear BookingWaitListEntity a la tabla BookingWaitList
public sealed class BookingWaitListEntityConfiguration : IEntityTypeConfiguration<BookingWaitListEntity>
{
    public void Configure(EntityTypeBuilder<BookingWaitListEntity> builder)
    {
        builder.ToTable("BookingWaitList");

        // Clave primaria
        builder.HasKey(x => x.IdWaitList);

        // El ID se genera automáticamente
        builder.Property(x => x.IdWaitList)
            .HasColumnName("IdWaitList")
            .ValueGeneratedOnAdd();

        // FK a la reserva en espera, obligatoria
        builder.Property(x => x.IdBooking)
            .HasColumnName("IdBooking")
            .IsRequired();

        // FK al vuelo deseado, obligatoria
        builder.Property(x => x.IdFlight)
            .HasColumnName("IdFlight")
            .IsRequired();

        // Posición en la cola de espera, obligatoria
        builder.Property(x => x.Position)
            .HasColumnName("Position")
            .IsRequired();

        // Fecha en que ingresó a la lista de espera, se registra con la hora actual
        builder.Property(x => x.RequestedAt)
            .HasColumnName("RequestedAt")
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        // FK al estado de la entrada (En Espera / Promovida / Expirada), obligatoria
        builder.Property(x => x.IdStatus)
            .HasColumnName("IdStatus")
            .IsRequired();

        // Relación: la entrada pertenece a una reserva, una reserva puede tener varias entradas en espera
        builder.HasOne(x => x.Booking)
            .WithMany(x => x.BookingWaitListEntries)
            .HasForeignKey(x => x.IdBooking)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: la entrada apunta a un vuelo deseado, un vuelo puede tener muchas entradas en espera
        builder.HasOne(x => x.Flight)
            .WithMany(x => x.WaitListEntries)
            .HasForeignKey(x => x.IdFlight)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: cada entrada tiene un estado, un estado puede aplicar a muchas entradas
        builder.HasOne(x => x.Status)
            .WithMany(x => x.BookingWaitListEntries)
            .HasForeignKey(x => x.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
