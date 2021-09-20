namespace authservice.JWT
{
    public interface IJWTService
    {
        string CreateToken(Payload payload);
        Payload ValidateToken(string token);
    }
}
