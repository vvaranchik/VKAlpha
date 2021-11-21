namespace Alpha.VKLib.Methods
{
    using System.Text.Json;

    using Alpha.SharedLibrary.Interfaces;

    public enum RequestUrlType
    {
        ApiBase,
        MethodBase,
        Auth
    }

    public class BaseMethod : IMethod<VK>
    {
        public VK service { get; set; }
        public HttpClient client { get; set; }

        public BaseMethod(VK vkInstance)
        {
            service = vkInstance;
            client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate });
            client.DefaultRequestHeaders.Add("User-Agent", "VKAndroidApp/4.0.1-816 (Android 9.0; SDK 28; ARM64; Huawei Note 20; ru)");
        }

        public bool IsValid() => !string.IsNullOrEmpty(service.Token.Token) && !service.Token.IsExpired();

        static string GetUrl(RequestUrlType type, string method)
        {
            var url = "https://api.vk.com/";
            switch (type)
            {
                case RequestUrlType.ApiBase: break;
                case RequestUrlType.MethodBase: url += "method/"; break;
                case RequestUrlType.Auth: url += "oauth/token"; break;
            }
            return url += method;
        }

        protected async Task<JsonDocument?> GetAsync(RequestUrlType type, string method, Dictionary<string, string> @params)
        {
            var response = await client.GetAsync(new Uri(string.Concat(GetUrl(type, method), "?", string.Join("&", @params.Select(kvp => $"{kvp.Key}={kvp.Value}")))));
            using (var stream = response.Content.ReadAsStream())
            {
                if (stream.Length > 0)
                    return await JsonDocument.ParseAsync(stream) ?? null;
            }
            return null;
        }

        protected async Task<JsonDocument?> PostAsync(RequestUrlType type, string method, Dictionary<string, string> @params)
        {
            var postContent = new FormUrlEncodedContent(@params);
            postContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await client.PostAsync(new Uri(GetUrl(type, method)), postContent);
            using (var stream = response.Content.ReadAsStream())
            {
                if (stream.Length > 0)
                    return await JsonDocument.ParseAsync(stream) ?? null;
            }
            return null;
        }
    }
}
