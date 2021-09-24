namespace authservice.JWT
{
    public interface ITokenService
    {
        string CreateToken(Payload payload);
        Payload ValidateToken(string token);
    }
}
