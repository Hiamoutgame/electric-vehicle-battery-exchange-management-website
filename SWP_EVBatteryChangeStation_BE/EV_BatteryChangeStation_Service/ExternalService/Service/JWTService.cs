using EV_BatteryChangeStation_Common.DTOs.AuthencationDTO;
using EV_BatteryChangeStation_Common.Enum.ServiceResult;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.ExternalService.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Service.ExternalService.Service
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _config;
        public JWTService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Generate Token
        /// </summary>
        /// <param name="tokenDto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GenerateToken(LoginRespondDTO tokenDto)
        {
            try
            {
                var jwtKey = _config["JwtConfig:Key"] ?? throw new InvalidOperationException("JwtConfig:Key is missing.");
                var issuer = _config["JwtConfig:Issuer"] ?? "App";
                var audience = _config["JwtConfig:Audience"] ?? "App";
                var expiryRaw = _config["JwtConfig:ExpireMinutes"] ?? _config["JwtConfig:ExpiresInMinutes"];
                var expiryMinutes = double.TryParse(expiryRaw, out var parsedMinutes) ? parsedMinutes : 30d;

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, tokenDto.AccountId.ToString())
                };
                if (!string.IsNullOrEmpty(tokenDto.AccountName)) claims.Add(new Claim(JwtRegisteredClaimNames.Name, tokenDto.AccountName));
                if (!string.IsNullOrEmpty(tokenDto.Email)) claims.Add(new Claim(JwtRegisteredClaimNames.Email, tokenDto.Email));
                if (!string.IsNullOrEmpty(tokenDto.RoleName)) claims.Add(new Claim(ClaimTypes.Role, tokenDto.RoleName));
                var token = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = creds
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var createdToken = tokenHandler.CreateToken(token);

                return tokenHandler.WriteToken(createdToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"| {ex.Message} \n| InnerException: {ex.InnerException}");
                throw new InvalidOperationException("Fail to generate token");
            }
        }
        /// <summary>
        /// Validate Token mã token 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IServiceResult> ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new ServiceResult
                {
                    Status = Const.INVALID_TOKEN_CODE,
                    Message = "Token is null or empty",
                };
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _config["JwtConfig:Key"] ?? throw new InvalidOperationException("JwtConfig:Key is missing.");
            var issuer = _config["JwtConfig:Issuer"] ?? "App";
            var audience = _config["JwtConfig:Audience"] ?? "App";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var validation = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };
            try
            {
                var principal = tokenHandler.ValidateToken(token, validation, out _);
                return new ServiceResult
                {
                    Status = Const.SUCCESS_VALIDATE_TOKEN_CODE,
                    Message = Const.SUCCESS_VALIDATE_TOKEN_MSG,
                    Data = principal
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return new ServiceResult
                {
                    Status = Const.EXPIRED_TOKEN_CODE,
                    Message = Const.EXPIRED_TOKEN_MSG,
                };
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return new ServiceResult
                {
                    Status = Const.INVALID_TOKEN_SIGNATURE_CODE,
                    Message = Const.INVALID_TOKEN_SIGNATURE_MSG,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Status = Const.ERROR_EXCEPTION,
                    Message = $"|Exception: {ex.Message}\n|InnerException: {ex.InnerException?.Message}",
                };
            }
        }
    }
}
