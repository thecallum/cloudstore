namespace DocumentService.Middleware
{
    public interface ITokenService
    {
        string CreateToken(Payload payload);
        Payload ValidateToken(string token);
        Payload DecodeToken(string token);
    }
}