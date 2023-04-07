namespace IdentityServerDatabaseModels
{
    using System.Security.Cryptography;

    public class CreatePasswordSalt : ICreatePasswordSalt
    {
        string ICreatePasswordSalt.CreateSalt(int size)
        {
            using (var generator = RandomNumberGenerator.Create())
            {
                var salt = new byte[size];
                generator.GetBytes(salt);
                return Convert.ToBase64String(salt).Substring(0, size); ;
            }
        }
    }
}
