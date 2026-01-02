using Microsoft.EntityFrameworkCore;
using PurchaseService.API.Models;

namespace PurchaseService.API.Data
{
    public class PurchaseDbContext : DbContext
    {
        public PurchaseDbContext(DbContextOptions<PurchaseDbContext> options) : base(options)
        {
        }

        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.PurchaseAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.PurchasedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.BuyerId);
                entity.HasIndex(e => e.OfferId);
                entity.HasIndex(e => e.SellerId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.PurchasedAt);
            });
        }
    }
}