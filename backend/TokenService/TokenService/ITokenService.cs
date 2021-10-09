using System;
using System.Collections.Generic;
using System.Text;
using TokenService.Models;

namespace TokenService
{
    public interface ITokenService
    {
        string CreateToken(Payload payload);
        Payload ValidateToken(string token);
        Payload DecodeToken(string token);
    }
}
