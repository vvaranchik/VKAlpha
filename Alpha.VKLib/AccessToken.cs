namespace Alpha.VKLib
{
    using System;

    public partial class VK
    {
        public class AccessToken : SharedLibrary.Realisation.AccessToken
        {
            private VK vkInstance;

            public long UserId { get; private set; }

            public AccessToken(VK vk) { vkInstance = vk; }

            public AccessToken(VK vk, string token, DateTime expireTime) : this(vk)
            {
                Token = token;
                ExpiresIn = expireTime;
            }

            public AccessToken(VK vk, string token, long unixMs) : this(vk, token, DateTimeOffset.FromUnixTimeMilliseconds(unixMs).LocalDateTime) { }

            public async Task<bool> RefreshToken(string newToken, DateTime expireTime)
            {
                this.Token = newToken;
                this.ExpiresIn = expireTime;
                if (await vkInstance.Users.GetBaseInfo(new[] { this.UserId.ToString() }, "first_name"))
                    return true;
                return false;
            }
        }
    }
}
