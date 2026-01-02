using AccountService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountService.API.Data
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.UserType)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(UserStatus.Active);

                entity.Property(e => e.Address)
                    .HasMaxLength(500);

                entity.Property(e => e.City)
                    .HasMaxLength(100);

                entity.Property(e => e.State)
                    .HasMaxLength(100);

                entity.Property(e => e.ZipCode)
                    .HasMaxLength(20);

                entity.Property(e => e.Country)
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                // Indexes
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");

                entity.HasIndex(e => e.UserType)
                    .HasDatabaseName("IX_Users_UserType");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Users_Status");

                entity.HasIndex(e => new { e.FirstName, e.LastName })
                    .HasDatabaseName("IX_Users_FullName");
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var users = new[]
            {
                new User
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    FirstName = "John",
                    LastName = "Seller",
                    Email = "john.seller@example.com",
                    PhoneNumber = "555-0101",
                    UserType = UserType.Seller,
                    Status = UserStatus.Active,
                    Address = "123 Seller Street",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001",
                    Country = "USA",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new User
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    FirstName = "Jane",
                    LastName = "Buyer",
                    Email = "jane.buyer@example.com",
                    PhoneNumber = "555-0102",
                    UserType = UserType.Buyer,
                    Status = UserStatus.Active,
                    Address = "456 Buyer Avenue",
                    City = "Los Angeles",
                    State = "CA",
                    ZipCode = "90001",
                    Country = "USA",
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    UpdatedAt = DateTime.UtcNow.AddDays(-25)
                },
                new User
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    FirstName = "Mike",
                    LastName = "Carrier",
                    Email = "mike.carrier@example.com",
                    PhoneNumber = "555-0103",
                    UserType = UserType.Carrier,
                    Status = UserStatus.Active,
                    Address = "789 Carrier Road",
                    City = "Chicago",
                    State = "IL",
                    ZipCode = "60001",
                    Country = "USA",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new User
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    FirstName = "Sarah",
                    LastName = "Agent",
                    Email = "sarah.agent@example.com",
                    PhoneNumber = "555-0104",
                    UserType = UserType.Agent,
                    Status = UserStatus.Active,
                    Address = "321 Agent Plaza",
                    City = "Miami",
                    State = "FL",
                    ZipCode = "33101",
                    Country = "USA",
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-15)
                }
            };

            modelBuilder.Entity<User>().HasData(users);
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
                .Where(x => x.Entity is User && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((User)entity.Entity).CreatedAt = DateTime.UtcNow;
                }

                ((User)entity.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}