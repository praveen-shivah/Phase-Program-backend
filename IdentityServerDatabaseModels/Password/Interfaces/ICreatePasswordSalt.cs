namespace IdentityServerDatabaseModels
{
    public interface ICreatePasswordSalt
    {
        string CreateSalt(int size);
    }
}
