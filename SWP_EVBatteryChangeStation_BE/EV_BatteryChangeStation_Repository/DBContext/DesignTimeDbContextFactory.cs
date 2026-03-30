using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EV_BatteryChangeStation_Repository.DBContext;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var connectionString =
            configuration.GetConnectionString("PostgresConnection") ??
            configuration.GetConnectionString("DefaultConnection") ??
            configuration.GetConnectionString("PostgresV2Connection") ??
            configuration.GetConnectionString("V2Connection") ??
            throw new InvalidOperationException("No database connection string was found for design-time AppDbContext creation.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
