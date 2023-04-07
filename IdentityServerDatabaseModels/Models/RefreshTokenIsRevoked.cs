namespace DatabaseContext
{
    public partial class RefreshToken
    {
        public bool IsRevoked
        {
            get
            {
                return Revoked != null;
            }
        }
    }
}
