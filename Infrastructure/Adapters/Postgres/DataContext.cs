using Domain.BookingAggregate;
using Domain.CustomerAggregate;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Adapters.Postgres;

public class DataContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<OutboxEvent> Outbox { get; set; }
    public DbSet<InboxEvent> Inbox { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new LevelEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VehicleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VehicleModelEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BookingEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StatusEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxEventTypeConfiguration());
        modelBuilder.ApplyConfiguration(new InboxEventTypeConfiguration());
    }
}

internal class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customer");
        
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Level).WithMany().HasForeignKey("level_id").HasConstraintName("FK_level_id").IsRequired();

        builder.Property(x => x.Id).ValueGeneratedNever().HasColumnName("id");
        builder.Property(x => x.Trips).HasColumnName("trips");
        builder.Property(x => x.CanceledBookings).HasColumnName("canceled_bookings");
        builder.Property(x => x.IsCanBooking).HasColumnName("is_can_booking");
        builder.Property(x => x.Categories)
            .HasField("_categories")
            .HasConversion<char[]>(x => x.Select(to => to.Character).ToArray(),
                from => from.Select(Category.Create).ToArray())
            .HasColumnName("categories");
        
        builder.Ignore(x => x.Points);
    }
}

internal class LevelEntityTypeConfiguration : IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> builder)
    {
        builder.ToTable("level");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).ValueGeneratedNever().HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").IsRequired();

        // Todo: временное решение, OwnsOne с HasData не дружат https://github.com/dotnet/efcore/issues/31373 ничего лучше пока не придумал
        builder.Property<LoyaltyPoints>(x => x.NeededPoints)
            .HasConversion<int>(to => to.Value, from => LoyaltyPoints.Create(from)).HasColumnName("needed_points");
        
        builder.HasData(Level.All());
    }
}

internal class VehicleEntityTypeConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicle");
        
        builder.HasKey(x => x.Id);

        builder.HasOne<VehicleModel>()
            .WithMany()
            .HasForeignKey(x => x.VehicleModelId)
            .HasConstraintName("FK_vehicle_model_id");
        
        builder.Property(x => x.Id).ValueGeneratedNever().HasColumnName("id");
        builder.Property(x => x.VehicleModelId).ValueGeneratedNever().HasColumnName("vehicle_model_id").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
    }
}

internal class VehicleModelEntityTypeConfiguration : IEntityTypeConfiguration<VehicleModel>
{
    public void Configure(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.ToTable("vehicle_model");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).ValueGeneratedNever().HasColumnName("id");
        builder.OwnsOne(x => x.Category,
            cfg => cfg.Property(x => x.Character).HasColumnName("category").HasConversion<char>().IsRequired());
    }
}

internal class BookingEntityTypeConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("booking");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).ValueGeneratedNever().HasColumnName("id");
        
        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(x => x.VehicleId)
            .HasConstraintName("FK_vehicle_id")
            .IsRequired();

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey("status_id")
            .HasConstraintName("FK_status_id")
            .IsRequired();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .HasConstraintName("FK_customer_id")
            .IsRequired();

        builder.Property(x => x.CustomerId).ValueGeneratedNever().HasColumnName("customer_id").IsRequired();
        builder.Property(x => x.VehicleId).ValueGeneratedNever().HasColumnName("vehicle_id").IsRequired();
        builder.Property(x => x.Start).HasColumnName("start").IsRequired(false);
        builder.Property(x => x.End).HasColumnName("end").IsRequired(false);
        
        builder.OwnsOne(x => x.FreeWait, cfg =>
        {
            cfg.Property(x => x.Duration).HasColumnName("free_wait_duration");
        });
        
        builder.Ignore(x => x.DomainEvents);
    }
}

internal class StatusEntityTypeConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.ToTable("status");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).ValueGeneratedNever().HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").IsRequired();
        
        builder.HasData(Status.All());
    }
}

internal class OutboxEventTypeConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        builder.ToTable("outbox");

        builder.HasKey(x => x.EventId);

        builder.HasIndex(x => new { x.OccurredOnUtc, x.ProcessedOnUtc }, "IX_outbox_messages_unprocessed")
            .IncludeProperties(x => new { x.EventId, x.Type })
            .IsDescending(false, false)
            .HasFilter("processed_on_utc IS NULL");

        builder.Property(x => x.EventId).ValueGeneratedNever().HasColumnName("event_id").IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").IsRequired();
        builder.Property(x => x.Content).HasColumnName("content").IsRequired();
        builder.Property(x => x.OccurredOnUtc).HasColumnName("occurred_on_utc").IsRequired();
        builder.Property(x => x.ProcessedOnUtc).HasColumnName("processed_on_utc").IsRequired(false);
    }
}

internal class InboxEventTypeConfiguration : IEntityTypeConfiguration<InboxEvent>
{
    public void Configure(EntityTypeBuilder<InboxEvent> builder)
    {
        builder.ToTable("inbox");

        builder.HasKey(x => x.EventId);

        builder.HasIndex(x => new { x.OccurredOnUtc, x.ProcessedOnUtc }, "IX_inbox_messages_unprocessed")
            .IncludeProperties(x => new { x.EventId, x.Type })
            .IsDescending(false, false)
            .HasFilter("processed_on_utc IS NULL");

        builder.Property(x => x.EventId).ValueGeneratedNever().HasColumnName("event_id").IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").IsRequired();
        builder.Property(x => x.Content).HasColumnName("content").IsRequired();
        builder.Property(x => x.OccurredOnUtc).HasColumnName("occurred_on_utc").IsRequired();
        builder.Property(x => x.ProcessedOnUtc).HasColumnName("processed_on_utc").IsRequired(false);
    }
}