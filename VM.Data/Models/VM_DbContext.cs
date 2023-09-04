using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
#nullable disable

namespace VM.Data.Models
{
    public partial class VM_DbContext : DbContext
    {
        private const string appSettingsJsonFileName = "appsettings.json";
        private readonly string connectionString = string.Empty;
        public VM_DbContext()
        {
            var builder = new ConfigurationBuilder();
            var config = builder.AddJsonFile(appSettingsJsonFileName, optional: false).Build();
            this.connectionString = config.GetConnectionString("WebApiDatabase").ToString();
        }

        public VM_DbContext(DbContextOptions<VM_DbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserPurchase> UserPurchases { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(this.connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserPurchaseConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
