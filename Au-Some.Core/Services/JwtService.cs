using Au_Some.Core.DTO;
using Au_Some.Core.Identity;
using Au_Some.Core.ServiceContract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Au_Some.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _counfiguration;
        private readonly ILogger<JwtService> _logger;
        public JwtService(IConfiguration configuration,ILogger<JwtService>logger)
        {
            _counfiguration= configuration;
            _logger= logger;
        }
        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
        {
            DateTime AccessTokenExpirationDate = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_counfiguration["Jwt:expiration_minutes"]));
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.UserName),
                new Claim(ClaimTypes.Name,user.ChildName),
                new Claim(ClaimTypes.Email,user.Email)
            };
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_counfiguration["Jwt:Key"]));
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
             _counfiguration["Jwt:Issuer"],
             _counfiguration["Jwt:Audience"],
            claims,
            expires: AccessTokenExpirationDate,
            signingCredentials: signingCredentials);
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            string token = jwtHandler.WriteToken(tokenGenerator);
            return new AuthenticationResponse()
            {
                ChildName=user.ChildName,
                AccessTokenExpirationDateTime=AccessTokenExpirationDate,
                Email=user.Email,
                AccessToken=token
                ,
                RefreshToken=RefreshToken(),
                RefreshTokenExpirationDateTime=DateTime.Now.AddDays(30)
            };
        }

        public ClaimsPrincipal? GetClaimsPrincipal(string? token)
        {
            try { 
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = _counfiguration["Jwt:Issuer"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_counfiguration["Jwt:Key"])),

                ValidateLifetime = false,
                ValidateAudience=false
            };
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityTokenException("InValid Token");
            }
            return principal;
            }
            catch(JsonException ex)
            {
                _logger.LogDebug($"an error happed while validating JWT token {ex.Message}");
                return null;
            }
        }

        public string RefreshToken()
        {
            byte[] bytes= new byte[64];
            var randomNumberGenerator = RandomNumberGenerator.Create();
           randomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
