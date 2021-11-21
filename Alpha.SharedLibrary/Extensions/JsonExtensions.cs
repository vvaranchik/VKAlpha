namespace Alpha.SharedLibrary.Extensions
{
    using System.Text;
    using System.Linq;
    using System.Text.Json;

    public static class JsonExtensions
    {
        public static T? As<T>(this JsonElement json) => json.Deserialize<T>();

        public static T? As<T>(this JsonElement json, string token) => json.SelectToken(token).As<T>();

        public static IEnumerable<T?> ToEnumerable<T>(this JsonElement json) => json.EnumerateArray().Select(jElem => jElem.As<T>());
        
        public static IEnumerable<T?> ToUtf8Enumerable<T>(this JsonElement json) => json.EnumerateArray().Select(jElem => JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(jElem.ToString()))));

        public static JsonElement SelectToken(this JsonElement json, string separator, string tokens)
        {
            // { "response": { "items": [ {...} ] } }
            // response.items.etc
            JsonElement ret = json;
            foreach (var token in tokens.Split(separator, StringSplitOptions.TrimEntries))
                if (!ret.TryGetProperty(token, out ret))
                    return ret; // return last found token
            return ret;
        }

        public static JsonElement SelectToken(this JsonElement json, string token) => SelectToken(json, ".", token);
    }
}
