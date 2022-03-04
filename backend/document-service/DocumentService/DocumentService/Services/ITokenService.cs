using DocumentService.Domain;

namespace DocumentService.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
        bool ValidateToken(string token);
        User DecodeToken(string token);
    }
}
