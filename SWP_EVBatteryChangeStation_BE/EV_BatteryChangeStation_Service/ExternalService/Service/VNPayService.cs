using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.ExternalService.IService;
using EV_BatteryChangeStation_Repository.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;

namespace EV_BatteryChangeStation_Service.ExternalService.Service;

public sealed class VNPayService : IVNPayService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IVnpay _vnPay;

    public VNPayService(IVnpay vnPay, IConfiguration configuration, AppDbContext context)
    {
        _vnPay = vnPay;
        _configuration = configuration;
        _context = context;

        var tmnCode = _configuration["Vnpay:TmnCode"] ?? string.Empty;
        var hashSecret = _configuration["Vnpay:HashSecret"] ?? string.Empty;
        var baseUrl = _configuration["Vnpay:BaseUrl"] ?? string.Empty;
        var returnUrl = _configuration["Vnpay:ReturnUrl"] ?? string.Empty;

        _vnPay.Initialize(
            tmnCode,
            hashSecret,
            baseUrl,
            returnUrl);
    }

    public async Task<IServiceResult> CreatePaymentURL(Guid payment, string ipAddress)
    {
        var transaction = await _context.Payments.FirstOrDefaultAsync(x => x.PaymentId == payment);
        if (transaction is null)
        {
            return new ServiceResult(404, "Payment not found.");
        }

        if (transaction.Status == "PAID")
        {
            return new ServiceResult(400, "Payment has already been completed.");
        }

        if (!transaction.PaymentGatewayId.HasValue)
        {
            transaction.PaymentGatewayId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            transaction.TransactionReference = transaction.PaymentGatewayId.Value.ToString();
            await _context.SaveChangesAsync();
        }

        var request = new PaymentRequest
        {
            PaymentId = transaction.PaymentGatewayId.Value,
            Money = Convert.ToDouble(transaction.Amount),
            Description = $"Payment {transaction.PaymentId}",
            IpAddress = ipAddress,
            BankCode = BankCode.ANY,
            CreatedDate = DateTime.UtcNow,
            Currency = Currency.VND,
            Language = DisplayLanguage.Vietnamese
        };

        var paymentUrl = _vnPay.GetPaymentUrl(request);
        return new ServiceResult(200, "Create payment URL success", paymentUrl);
    }

    public async Task<IServiceResult> ValidateRespond(IQueryCollection queryParams)
    {
        if (queryParams is null || queryParams.Count == 0)
        {
            return new ServiceResult(400, "Invalid query parameter.");
        }

        var paymentResult = _vnPay.GetPaymentResult(queryParams);
        if (paymentResult is null)
        {
            return new ServiceResult(400, "Payment result is invalid.");
        }

        var payment = await _context.Payments.FirstOrDefaultAsync(x => x.PaymentGatewayId == paymentResult.PaymentId);
        if (payment is null)
        {
            return new ServiceResult(404, "Payment not found.");
        }

        if (paymentResult.IsSuccess)
        {
            payment.Status = "PAID";
            payment.PaidAt = DateTime.UtcNow;
            payment.TransactionReference ??= paymentResult.PaymentId.ToString();
            await _context.SaveChangesAsync();
            return new ServiceResult(200, "Payment success", paymentResult);
        }

        payment.Status = "FAILED";
        await _context.SaveChangesAsync();
        return new ServiceResult(400, $"Payment failed. Description: {paymentResult.Description}");
    }
}
