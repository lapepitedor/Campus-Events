using Campus_Events.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Campus_Events.Persistence
{
    public class ApplicationDbContext :DbContext
    {
        private DbConnectionFactory dbConnectionFactory;
        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; } // Table de liaison pour les inscriptions aux événements

        public ApplicationDbContext(DbConnectionFactory connectionFactory)
        {
            this.dbConnectionFactory = connectionFactory;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new DateTimeToUtcConverter());
                    }
                }
            }



            // Configuration de la clé composite pour UserEvent
            modelBuilder.Entity<UserEvent>()
                .HasKey(ue => new { ue.UserId, ue.EventId });

            // Configuration des relations entre UserEvent, User, et Event
            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.UserEvents)
                .HasForeignKey(ue => ue.UserId);

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(ue => ue.EventId);
           
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connection = dbConnectionFactory.GetConnection();
                optionsBuilder.UseNpgsql(connection);
                optionsBuilder.UseLowerCaseNamingConvention();
            }

            base.OnConfiguring(optionsBuilder);
        }

        public void Migrate()
        {
            if (Database.GetPendingMigrations().Any())
                Database.Migrate();
        }

        // Convertisseur de DateTime vers UTC
        public class DateTimeToUtcConverter : ValueConverter<DateTime, DateTime>
        {
            public DateTimeToUtcConverter() : base(
                v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
            {
            }
        }
    }


}
