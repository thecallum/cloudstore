using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TokenService.Models;

namespace TokenService
{
    public class TokenService : ITokenService
    {
        private readonly string _secret = string.Empty;

        private readonly IJsonSerializer _serializer = new JsonNetSerializer();
        private readonly IJwtAlgorithm _algorithm = new HMACSHA256Algorithm();

        public TokenService(string secret)
        {
            _secret = secret;
        }

        public TokenService()
        {

        }

        public string CreateToken(Payload payload)
        {
            var encoder = CreateEncoder();

            var token = encoder.Encode(payload, _secret);

            return token;
        }

        public Payload DecodeToken(string token)
        {
            var decoder = CreateDecoder();

            try
            {
                var json = decoder.Decode(token);

                var payload = JsonConvert.DeserializeObject<Payload>(json);

                return payload;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public Payload ValidateToken(string token)
        {
            var decoder = CreateDecoder();

            try
            {
                var json = decoder.Decode(token, _secret, true);

                var payload = JsonConvert.DeserializeObject<Payload>(json);

                return payload;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private IJwtEncoder CreateEncoder()
        {
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(_algorithm, _serializer, urlEncoder);

            return encoder;
        }

        private IJwtDecoder CreateDecoder()
        {
            var provider = new UtcDateTimeProvider();

            IJwtValidator validator = new JwtValidator(_serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(_serializer, validator, urlEncoder, _algorithm);

            return decoder;
        }
    }

}
