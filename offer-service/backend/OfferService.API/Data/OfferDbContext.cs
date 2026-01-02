using OfferService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace OfferService.API.Data
{
    public class OfferDbContext : DbContext
    {
        public OfferDbContext(DbContextOptions<OfferDbContext> options) : base(options)
        {
        }

        public DbSet<Offer> Offers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Offer entity
            modelBuilder.Entity<Offer>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.SellerId)
                    .IsRequired();

                entity.Property(e => e.VIN)
                    .IsRequired()
                    .HasMaxLength(17);

                entity.Property(e => e.Make)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Year)
                    .IsRequired();

                entity.Property(e => e.OfferAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Condition);

                entity.Property(e => e.Address)
                    .HasMaxLength(500);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(OfferStatus.Available);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                // Indexes
                entity.HasIndex(e => e.SellerId)
                    .HasDatabaseName("IX_Offers_SellerId");

                entity.HasIndex(e => e.VIN)
                    .HasDatabaseName("IX_Offers_VIN");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Offers_Status");

                entity.HasIndex(e => new { e.Make, e.Model })
                    .HasDatabaseName("IX_Offers_Make_Model");

                entity.HasIndex(e => e.Year)
                    .HasDatabaseName("IX_Offers_Year");

                entity.HasIndex(e => e.OfferAmount)
                    .HasDatabaseName("IX_Offers_OfferAmount");

                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_Offers_CreatedAt");
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var sampleOffers = new[]
            {
                new Offer
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    SellerId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // John Seller from Account Service
                    VIN = "1HGBH41JXMN109186",
                    Make = "Honda",
                    Model = "Civic",
                    Year = 2023,
                    OfferAmount = 25000.00m,
                    Condition = VehicleCondition.Excellent.ToString(),
                    Address = "123 Main Street, New York, NY 10001",
                    Status = OfferStatus.Available,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Offer
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    SellerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    VIN = "2T1BURHE5JC123456",
                    Make = "Toyota",
                    Model = "Camry",
                    Year = 2022,
                    OfferAmount = 28500.00m,
                    Condition = VehicleCondition.Good.ToString(),
                    Address = "456 Oak Avenue, New York, NY 10002",
                    Status = OfferStatus.Available,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10)
                }
            };

            modelBuilder.Entity<Offer>().HasData(sampleOffers);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entities = ChangeTracker
                .Entries()
                .Where(x => x.Entity is Offer && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((Offer)entity.Entity).CreatedAt = DateTime.UtcNow;
                }

                ((Offer)entity.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}