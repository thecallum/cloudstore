namespace authservice.Encryption
{
    public interface IPasswordHasher
    {
        string Hash(string password);

        bool Check(string hash, string password);
    }
}
