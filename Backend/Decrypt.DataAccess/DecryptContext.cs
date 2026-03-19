using Decrypt.DataAccess.Entities;
using Decrypt.DataAccess.Entities.References;
using Microsoft.EntityFrameworkCore;

namespace Decrypt.DataAccess
{
    public class DecryptContext(DbContextOptions<DecryptContext> options) : DbContext(options)
    {
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Currency> Currencies => Set<Currency>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureOrganization(modelBuilder);
            ConfigureUser(modelBuilder);
            ConfigureProject(modelBuilder);
            ConfigureTimeEntry(modelBuilder);
            ConfigureInvoice(modelBuilder);
            
            //Reference values 
            ConfigureCurrency(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureOrganization(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organization>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.HasIndex(x => x.LegacyId)
                    .IsUnique();

                builder.HasOne(x => x.Currency)
                    .WithMany()
                    .HasForeignKey(x => x.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.OwnsOne(x => x.Metadata, metadata =>
                {
                    metadata.Property(x => x.Source);
                    metadata.Property(x => x.LegacyNumericId);
                    metadata.Property(x => x.MigratedAt);
                });
            });
        }

        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.HasIndex(x => x.LegacyId)
                    .IsUnique();

                builder.HasOne(x => x.Organization)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureProject(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.HasIndex(x => x.LegacyId)
                    .IsUnique();

                builder.HasOne(x => x.Organization)
                    .WithMany(x => x.Projects)
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureTimeEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TimeEntry>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.HasIndex(x => x.LegacyId)
                    .IsUnique();

                builder.HasOne(x => x.User)
                    .WithMany(x => x.TimeEntries)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x => x.Project)
                    .WithMany(x => x.TimeEntries)
                    .HasForeignKey(x => x.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureInvoice(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.HasIndex(x => x.LegacyId)
                    .IsUnique();

                builder.HasOne(x => x.Organization)
                    .WithMany(x => x.Invoices)
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x => x.Project)
                    .WithMany(x => x.Invoices)
                    .HasForeignKey(x => x.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x=> x.Currency)
                    .WithMany()
                    .HasForeignKey(x => x.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureCurrency(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Code)
                    .IsRequired();

                builder.HasIndex(x => x.Code)
                    .IsUnique();
            });
        }

    }
}
