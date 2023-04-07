namespace IdentityServerDatabaseModels
{
    public interface ICalculatePassword
    {
        string calculatePassword(string password, string passwordSalt);
    }
}
