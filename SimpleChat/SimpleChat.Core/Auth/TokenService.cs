using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sentry;

namespace SimpleChat.Core.Auth
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        bool IsTokenValid(string token);
    }

    public class TokenService : ITokenService
    {
        private IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var utcNow = DateTime.UtcNow;

            claims.Concat(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, utcNow.AddSeconds(_config.GetValue<int>("Jwt:ExpiryDuration")).ToString())
            }.AsEnumerable());

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<String>("Jwt:Key")));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                notBefore: utcNow,
                expires: utcNow.AddSeconds(_config.GetValue<int>("Jwt:ExpiryDuration")),
                audience: _config.GetValue<String>("Jwt:Audience"),
                issuer: _config.GetValue<String>("Jwt:Issuer")
            );

            return new JwtSecurityTokenHandler().WriteToken(tokeOptions); ;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<String>("Jwt:Key"))),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public bool IsTokenValid(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<String>("Jwt:Key"))),
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                    return false;
                else
                    return true;
            }
            catch (SecurityTokenExpiredException)
            {
                return false;
            }
            catch (SecurityTokenNotYetValidException)
            {
                return false;
            }
            catch (SecurityTokenNoExpirationException)
            {
                return false;
            }
            catch (SecurityTokenInvalidLifetimeException)
            {
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return false;
            }
            catch (SecurityTokenEncryptionKeyNotFoundException)
            {
                return false;
            }
            catch (SecurityTokenDecryptionFailedException)
            {
                return false;
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return false;
            }
        }
    }
}
