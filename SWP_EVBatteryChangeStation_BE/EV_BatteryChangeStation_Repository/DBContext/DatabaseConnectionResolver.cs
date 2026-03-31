using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EV_BatteryChangeStation_Repository.DBContext;

public static class DatabaseConnectionResolver
{
    public static string? GetConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString("PostgresConnection")
            ?? configuration.GetConnectionString("PostgresV2Connection")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("V2Connection");
    }

    public static bool IsPostgres(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        return connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase)
            || connectionString.Contains("Username=", StringComparison.OrdinalIgnoreCase)
            || connectionString.Contains("Port=", StringComparison.OrdinalIgnoreCase);
    }

    public static void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        if (IsPostgres(connectionString))
        {
            optionsBuilder.UseNpgsql(connectionString);
            return;
        }

        optionsBuilder.UseSqlServer(connectionString);
    }
}
