namespace Alpha.VKLib.Methods.Auth
{
    using System.Text.Json;

    using Alpha.SharedLibrary.Extensions;
    using Alpha.SharedLibrary.Realisation;

    [Flags]
    public enum VKScope
    {

    }

    public class VKAuth : BaseMethod
    {
        public VKAuth(VK vkInstance) : base(vkInstance) { }

        /// <summary>
        /// Auth like official android VK app
        /// </summary>
        /// <param name="login">Email or phone number</param>
        /// <param name="password">Passowrd</param>
        /// <param name="flags">Scoped access flags</param>
        /// <param name="captchaSid"></param>
        /// <param name="captchaKey"></param>
        /// <returns>True, if auth successful, false otherwise</returns>
        public async Task<SafeReturn<bool>> Login(string login, string password, VKScope flags, string? captchaSid = null, string? captchaKey = null)
        {
            return SafeReturn<bool>.AsEmpty(true); //url.vkauth
        }

        public async Task<SafeReturn<AccessToken>> RefreshToken(string? captchaSid = null, string? captchaKey = null)
        {
            return SafeReturn<AccessToken>.AsEmpty(new AccessToken()); // url.method
        }

        // https://oauth.vk.com/token?grant_type=restore_code&client_id={clientId}&client_secret={clientSecret}&username={phoneNumber}&scope={VKScope}&sid={sid}&code={smsCode}
        public async Task<SafeReturn<bool>> Restore(string phoneNumber, string lastName)
        {
            return SafeReturn<bool>.AsEmpty(true); // url.vkauth/token?params
        }

    }
}
