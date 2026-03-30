using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EV_BatteryChangeStation_Repository.DBContext;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var candidateBasePaths = new[]
        {
            currentDirectory,
            Path.GetFullPath(Path.Combine(currentDirectory, "..", "EV_BatteryChangeStation"))
        };

        IConfigurationRoot? configuration = null;
        foreach (var basePath in candidateBasePaths.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!Directory.Exists(basePath))
            {
                continue;
            }

            configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var hasConnectionString =
                configuration.GetConnectionString("PostgresConnection") is not null ||
                configuration.GetConnectionString("PostgresV2Connection") is not null ||
                configuration.GetConnectionString("DefaultConnection") is not null ||
                configuration.GetConnectionString("V2Connection") is not null;

            if (hasConnectionString)
            {
                break;
            }
        }

        configuration ??= new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .Build();

        var postgresConnection =
            configuration.GetConnectionString("PostgresConnection") ??
            configuration.GetConnectionString("PostgresV2Connection");

        var sqlServerConnection =
            configuration.GetConnectionString("DefaultConnection") ??
            configuration.GetConnectionString("V2Connection");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        if (!string.IsNullOrWhiteSpace(postgresConnection))
        {
            optionsBuilder.UseNpgsql(postgresConnection);
        }
        else if (!string.IsNullOrWhiteSpace(sqlServerConnection))
        {
            optionsBuilder.UseSqlServer(sqlServerConnection);
        }
        else
        {
            throw new InvalidOperationException("No database connection string was found for design-time AppDbContext creation.");
        }

        return new AppDbContext(optionsBuilder.Options);
    }
}
