namespace Alpha.VKLib.Methods.Users
{
    using System.Text.Json;

    using Alpha.SharedLibrary.Extensions;
    using Alpha.SharedLibrary.Realisation;

    public enum SortingType
    {
        Name,
        Popularity,
        Random
    }

    public class VKUsers : BaseMethod
    {
        public VKUsers(VK vkInstance) : base(vkInstance) { }

        public Task<SafeReturn<object?>>? GetBaseInfo(IEnumerable<string> userIds, string nameFields, string? captchaSid = null, string? captchaKey = null)
        {
            return null;
        }

        public Task<SafeReturn<object?>>? GetFriends(long userId, string? nameFields, SortingType sort, string? captchaSid = null, string? captchaKey = null)
        {
            return null;
        }

    }
}
