using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.ExternalService.IService;
using EV_BatteryChangeStation_Service.ExternalService.Service;
using EV_BatteryChangeStation_Service.InternalService.IService;
using EV_BatteryChangeStation_Service.InternalService.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VNPAY.NET;

namespace EV_BatteryChangeStation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var postgresConnection = configuration.GetConnectionString("PostgresConnection")
                ?? configuration.GetConnectionString("PostgresV2Connection");

            if (!string.IsNullOrWhiteSpace(postgresConnection))
            {
                options.UseNpgsql(postgresConnection);
                return;
            }

            var sqlServerConnection = configuration.GetConnectionString("DefaultConnection")
                ?? configuration.GetConnectionString("V2Connection");

            options.UseSqlServer(sqlServerConnection);
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAuthenService, AuthenService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IStationService, StationService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IBatteryService, BatteryService>();
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ISupportRequestService, SupportRequestService>();
        services.AddScoped<ISwappingService, SwappingService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IRevenueService, RevenueService>();
        services.AddScoped<IFeedBackService, FeedBackService>();

        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<IVNPayService, VNPayService>();
        services.AddScoped<IVnpay, Vnpay>();
        services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();

        return services;
    }

    public static IServiceCollection AddFrontendCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()
                    ?? new[] { "http://localhost:3000" };

                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
