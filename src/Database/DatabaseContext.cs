using Microsoft.EntityFrameworkCore;
using Vault.Core;
using Vault.Database.Tables;

namespace Vault.Database
{
    public class DatabaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                $"Data Source={Config.Get("database", "hostname")};" +
                $"Initial Catalog={Config.Get("database", "database")};" +
                $"User ID={Config.Get("database", "username")};" +
                $"Password={Config.Get("database", "password")};");
        }

        #region DbSets

        public DbSet<AccessLogDbEntry> AccessLogs { get; set; }

        public DbSet<AppDbEntry> Apps { get; set; }

        public DbSet<PropertyDbEntry> Properties { get; set; }

        #endregion
    }
}