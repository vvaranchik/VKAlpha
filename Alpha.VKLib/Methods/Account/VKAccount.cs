namespace Alpha.VKLib.Methods.Account
{

    using Alpha.SharedLibrary.Realisation;
    using Alpha.SharedLibrary.Extensions;

    public class VKAccount : BaseMethod
    {
        public VKAccount(VK vk) : base(vk) { }

        public async Task<SafeReturn<bool>> Ban()
        {
            return SafeReturn<bool>.AsEmpty(true);
        }

        public async Task<SafeReturn<bool>> ChangePassword(string restoreSid, string hash, string oldPassword, string newPassword)
        {
            return SafeReturn<bool>.AsEmpty(true);
        }

    }
}
