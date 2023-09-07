using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MedicalCenter.Services
{
    public class GenerateToken
    {
        private IConfiguration _configuration;

        public GenerateToken(IConfiguration _configuration)
        {
            this._configuration = _configuration;
        }

        public string GenerateJwtToken(string phonenumber,string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //string key_gen = GenerateRandomKey(256);
            var key = Encoding.ASCII.GetBytes("abcdefjklmnopqrstuvwxyzsgfdgdgdhggfhfghfghgfjffgfgh");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.MobilePhone, phonenumber),
                    new Claim("role", role),
                }),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(this._configuration["Jwt:TokenExpirationDays"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = "blabla",
                Issuer = "blabla"
                
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //this for verification purpose , but its useless for now
        #region verificationtoken
        public string GenerateJwtVerificationToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string key_gen = GenerateRandomKey(256);
            var key = Encoding.ASCII.GetBytes(key_gen);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Email, email)
            }),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(this._configuration["Jwt:VerficationTokenExpirationDays"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion

        string GenerateRandomKey(int keySizeInBits)
        {
            byte[] keyBytes = new byte[keySizeInBits / 8];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(keyBytes);
            }
            return Convert.ToBase64String(keyBytes);
        }


        public bool IsTokenExpired(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = false,
                ValidateLifetime = true
            };

            try
            {
                // Parse and validate the JWT token
                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);

                // Extract the expiration claim from the token's claims
                var expirationClaim = claimsPrincipal.FindFirst("exp");

                // Check if the expiration claim exists and if the expiration time is in the past
                if (expirationClaim != null && DateTime.TryParse(expirationClaim.Value, out var expirationDate))
                {
                    return expirationDate < DateTime.UtcNow;
                }

                return false;

            }
            catch (Exception)
            {
                throw new Exception("Invalid token format or other error occurred");
            }
        }

        public string ExtractEmail(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            var payload = jwtToken.Payload;

            var phonenumber = payload["phonenumber"]?.ToString();

            return phonenumber;
        }

        public DateTime ExtractExpiredTime(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            var expiration = jwtToken.Payload.Exp;

            var expirationDateTime = DateTimeOffset.FromUnixTimeSeconds((Int64)expiration!).DateTime;

            return expirationDateTime;
        }
    }
}
