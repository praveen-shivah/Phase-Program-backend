namespace IdentityServerDatabaseModels
{
    using System.Security.Cryptography;
    using System.Text;

    using SharedUtilities;

    public class CalculatePassword : ICalculatePassword
    {
        string ICalculatePassword.calculatePassword(string password, string passwordSalt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combined = $"{password}{passwordSalt}";
                var hash = sha256.ComputeHash(Encoding.Unicode.GetBytes(combined));
                return hash.ByteArrayToHexString();
            }
        }
    }
}
