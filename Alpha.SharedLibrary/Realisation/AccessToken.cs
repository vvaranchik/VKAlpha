namespace Alpha.SharedLibrary.Realisation
{
    public partial class AccessToken
    {
        public string Token { get; protected set; } = "";

        public DateTime ExpiresIn { get; protected set; } = DateTime.MinValue;

        public bool IsExpired() => ExpiresIn < DateTime.Now;
    }
}
