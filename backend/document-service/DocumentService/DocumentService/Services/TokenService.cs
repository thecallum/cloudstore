using DocumentService.Domain;
using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Collections.Generic;

namespace DocumentService.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _secret;
        private readonly IJwtAlgorithm _algorithm = new HMACSHA256Algorithm();

        public TokenService(string secret = null)
        {
            _secret = string.IsNullOrEmpty(secret) ? 
                Environment.GetEnvironmentVariable("SECRET") : secret;
        }

        public string CreateToken(User user)
        {
            var token = JwtBuilder.Create()
                .WithAlgorithm(_algorithm)
                .WithSecret(_secret)
                .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(4).ToUnixTimeSeconds())
                .AddClaim("id", user.Id)
                .AddClaim("firstName", user.FirstName)
                .AddClaim("lastName", user.LastName)
                .AddClaim("email", user.Email)
                .AddClaim("storageCapacity", user.StorageCapacity)
                .Encode();

            return token;
        }

        public User DecodeToken(string token)
        {
            try
            {
                var payload = JwtBuilder.Create()
                .WithAlgorithm(_algorithm)
                .Decode<IDictionary<string, object>>(token);

                if (payload == null) return null;

                return new User
                {
                    Id = Guid.Parse((string)payload["id"]),
                    FirstName = (string)payload["firstName"],
                    LastName = (string)payload["lastName"],
                    Email = (string)payload["email"],
                    StorageCapacity = (long)payload["storageCapacity"]
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var payload = JwtBuilder.Create()
                    .WithAlgorithm(_algorithm)
                    .WithSecret(_secret)
                    .MustVerifySignature()
                    .Decode(token);

                if (payload == null) return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
