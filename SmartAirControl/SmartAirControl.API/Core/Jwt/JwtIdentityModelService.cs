using Microsoft.IdentityModel.Tokens;
using SmartAirControl.API.Core.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SmartAirControl.API.Core.Jwt
{
    public class JwtIdentityModelService : IJwtService
    {
        private readonly AppSettingsData _appSettings;

        public JwtIdentityModelService(AppSettingsData appSettings) => _appSettings = appSettings;

        /// <inheritdoc cref="IJwtService.GenerateJwtToken(IReadOnlyDictionary{string, string})"/>
        public TokenInfo GenerateJwtToken(IReadOnlyDictionary<string, string> payload)
        {
            byte[] key = Encoding.ASCII.GetBytes(_appSettings.Jwt.Secret);

            var claims = payload.Select(p => new Claim(p.Key, p.Value));
            var tokenExpiration = DateTime.UtcNow.AddSeconds(_appSettings.Jwt.Expiration);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpiration,
                Issuer = _appSettings.Jwt.Issuer,
                Audience = _appSettings.Jwt.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            string jwtToken = tokenHandler.WriteToken(securityToken);

            var result = new TokenInfo("Bearer", jwtToken, tokenExpiration);

            return result;
        }
    }
}
