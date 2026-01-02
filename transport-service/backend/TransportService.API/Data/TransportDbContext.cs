using Microsoft.EntityFrameworkCore;
using TransportService.API.Models;

namespace TransportService.API.Data
{
    public class TransportDbContext : DbContext
    {
        public TransportDbContext(DbContextOptions<TransportDbContext> options) : base(options)
        {
        }

        public DbSet<Transport> Transports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Transport entity configuration
            modelBuilder.Entity<Transport>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                
                entity.Property(e => e.TransportFee)
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.Status)
                    .HasConversion<int>();
                
                entity.HasIndex(e => e.CarrierId);
                entity.HasIndex(e => e.PurchaseId);
                entity.HasIndex(e => e.OfferId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.AssignedAt);
            });
        }
    }
}