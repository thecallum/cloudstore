using System;
using System.Collections.Generic;
using System.Text;
using TokenService.Models;

namespace TokenService
{
    public interface ITokenService
    {
        string CreateToken(User user);
        bool ValidateToken(string token);
        User DecodeToken(string token);
    }
}
