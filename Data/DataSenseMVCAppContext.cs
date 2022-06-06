using DataSenseMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace DataSenseMVC.Data
{
    public class DataSenseMVCAppContext : DbContext
    {
        public DataSenseMVCAppContext(DbContextOptions<DataSenseMVCAppContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(config.GetValue<string>("ConnectionStrings:DefaultConnection").ToString());
            }

            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
    }
}
