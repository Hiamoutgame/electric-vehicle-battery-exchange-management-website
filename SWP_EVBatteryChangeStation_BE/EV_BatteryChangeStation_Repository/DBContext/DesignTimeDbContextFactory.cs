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

            var hasConnectionString = !string.IsNullOrWhiteSpace(DatabaseConnectionResolver.GetConnectionString(configuration));

            if (hasConnectionString)
            {
                break;
            }
        }

        configuration ??= new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .Build();

        var connectionString = DatabaseConnectionResolver.GetConnectionString(configuration);

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("No database connection string was found for design-time AppDbContext creation.");
        }

        DatabaseConnectionResolver.Configure(optionsBuilder, connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
