using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using EcoAssistant.Infrastructure.Data;

namespace EcoAssistant.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // PostgreSQL connection string from your API appsettings.json
            var connectionString = "Host=localhost;Port=5433;Database=ecoassistant;Username=postgres;Password=1234";
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
