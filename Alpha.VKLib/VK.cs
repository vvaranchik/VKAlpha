namespace Alpha.VKLib
{
    using System;
    using System.Text.Json;

    using Alpha.SharedLibrary.Extensions;

    public partial class VK
    {
        public string ClientId { get; }

        public string ClientSercet { get; }

        public string ApiVersion { get; }

        public AccessToken Token { get; }

        public Methods.Auth.VKAuth Auth => new Methods.Auth.VKAuth(this);

        public Methods.Audio.VKAudio Audio => new Methods.Audio.VKAudio(this);

        public Methods.Users.VKUsers Users => new Methods.Users.VKUsers(this);

        internal Dictionary<string, string> GetBaseParametrs() => new Dictionary<string, string> { { "access_token", Token.Token }, { "v", ApiVersion } };

        internal SharedLibrary.Realisation.SafeReturn<T?> ProcessError<T>(JsonElement? json)
        {
            int code = SharedLibrary.Realisation.ErrorCodes.Common;
            if (json == null) return new SharedLibrary.Realisation.SafeReturn<T?>(default, code, "Json is null");
            object? content = null;
            if (json != null && json.Value.TryGetProperty("error_code", out JsonElement errorCodeElem))
            {
                code = errorCodeElem.GetInt32();
                switch (code)
                {
                    case 1:
                    case 10:
                        content = "Try again later";
                        break;
                    case 5: // auth fail
                        content = "Invalid client";
                        break;
                    case 9:
                        content = "Flood detected";
                        break;
                    case 14:
                    case 3300:
                        content = new[] { json.Value.As<string>("captcha_sid"), json.Value.As<string>("captcha_img") };
                        break;
                    case 34:
                        content = $"Api version ({ApiVersion}) deprecated";
                        break;
                    case 113: //  user not found
                        content = "Not found";
                        break;
                    case 15:  //  access denied
                    case 18:  //  access denied
                    case 37:  //  access denied (user banned)
                    case 39:  // unk user
                    case 40:  // unk group
                    case 200: //  access denied
                    case 201: //  access denied
                    case 203: //  access denied
                        content = "Access denied";
                        break;
                    default:
                        content = json.Value.ToString();
                        break;
                }
            }
            else
            {
                if (json.Value.GetString() == "need_captcha")
                {
                    code = 14;
                    content = new[] { json.Value.As<string>("captcha_sid"), json.Value.As<string>("captcha_img") };
                }
            }
            return new SharedLibrary.Realisation.SafeReturn<T?>(default, code, content);
        }

        public VK(string clientId, string clientSecret, string apiVersion = "5.68")
        {
            ClientId = clientId;
            ClientSercet = clientSecret;
            ApiVersion = "5.68";
            Token = new AccessToken(this);
        }

        public VK(string clientId, string clientSecret, string apiVersion, string accessToken, DateTime expireTime) : this(clientId, clientSecret, apiVersion)
        {
            Token = new AccessToken(this, accessToken, expireTime);
        }
    }
}
