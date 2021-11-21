namespace Alpha.VKLib.Methods.Audio
{
    using System.Collections.Generic;
    using System.Text.Json;

    using Alpha.SharedLibrary.Extensions;
    using Alpha.SharedLibrary.Realisation;

    public class VKAudio : BaseMethod
    {
        public VKAudio(VK vkInstance) : base(vkInstance) { }

        /// <summary>
        /// Gets list of all user audios
        /// </summary>
        /// <param name="ownerId">Group or user id</param>
        /// <param name="albumId"></param>
        /// <param name="count">If negative - all audios will be returned</param>
        /// <param name="onlyAvailable">Only select audios with valid Url</param>
        /// <param name="captchaSid"></param>
        /// <param name="captchaKey"></param>
        /// <returns>List of <a href="">Models.VKAudio</a> audio items</returns>
        public async Task<SafeReturn<List<Models.Audio.VKAudio?>?>> Get(long ownerId, long albumId = 0, int count = 200, bool onlyAvailable = false, string? captchaSid = null, string? captchaKey = null)
        {
            if (!IsValid())
                return new SafeReturn<List<Models.Audio.VKAudio?>?>(null, 5);
            
            var parametrs = service.GetBaseParametrs();
            parametrs.Add("owner_id", $"{ownerId}");
            if (count > 0 && count < 9999) parametrs.Add("count", $"{count}");
            else parametrs.Add("count", "9999");
            if (albumId > 0) parametrs.Add("album_id", $"{albumId}");
            if (captchaSid != null && captchaKey != null)
            {
                parametrs.Add("captcha_sid", captchaSid);
                parametrs.Add("captcha_key", captchaKey);
            }
            var json = await GetAsync(RequestUrlType.MethodBase, "audio.get", parametrs);
            JsonElement? jsonError = null;
            if (json == null || ErrorProcessor.IsError(json, "error_code", out jsonError))
                return service.ProcessError<List<Models.Audio.VKAudio?>?>(jsonError);

            var arr = json.RootElement.SelectToken("response.items").ToEnumerable<Models.Audio.VKAudio>();
            if (onlyAvailable)
                arr = arr.Where(audio => !string.IsNullOrEmpty(audio.Url));
            return new SafeReturn<List<Models.Audio.VKAudio?>?>(new List<Models.Audio.VKAudio?>(arr), -1);
        }
    }
}
