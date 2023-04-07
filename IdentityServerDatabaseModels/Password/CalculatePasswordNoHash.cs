namespace IdentityServerDatabaseModels
{
    public class CalculatePasswordNoHash : ICalculatePassword
    {
        string ICalculatePassword.calculatePassword(string password, string passwordSalt)
        {
            return password;
        }
    }
}
