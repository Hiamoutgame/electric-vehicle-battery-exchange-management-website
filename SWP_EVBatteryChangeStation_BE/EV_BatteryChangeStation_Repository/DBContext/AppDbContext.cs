using EV_BatteryChangeStation_Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EV_BatteryChangeStation_Repository.DBContext;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<BatteryType> BatteryTypes => Set<BatteryType>();
    public DbSet<StationBatteryType> StationBatteryTypes => Set<StationBatteryType>();
    public DbSet<StationStaffAssignment> StationStaffAssignments => Set<StationStaffAssignment>();
    public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Battery> Batteries => Set<Battery>();
    public DbSet<BatteryHistory> BatteryHistories => Set<BatteryHistory>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<SwappingTransaction> SwappingTransactions => Set<SwappingTransaction>();
    public DbSet<BatteryReturnInspection> BatteryReturnInspections => Set<BatteryReturnInspection>();
    public DbSet<StationInventoryLog> StationInventoryLogs => Set<StationInventoryLog>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<SupportRequest> SupportRequests => Set<SupportRequest>();
    public DbSet<SupportRequestResponse> SupportRequestResponses => Set<SupportRequestResponse>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var postgresConnection =
            configuration.GetConnectionString("PostgresConnection") ??
            configuration.GetConnectionString("PostgresV2Connection");

        var sqlServerConnection =
            configuration.GetConnectionString("DefaultConnection") ??
            configuration.GetConnectionString("V2Connection");

        if (!string.IsNullOrWhiteSpace(postgresConnection))
        {
            optionsBuilder.UseNpgsql(postgresConnection);
            return;
        }

        if (!string.IsNullOrWhiteSpace(sqlServerConnection))
        {
            optionsBuilder.UseSqlServer(sqlServerConnection);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

