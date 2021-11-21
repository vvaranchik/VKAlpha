namespace Alpha.SharedLibrary.Realisation
{
    using System.Text.Json;

    public static class ErrorProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="json_error_code_field">Token name in json</param>
        /// <param name="errorData">Json of error</param>
        /// <returns>If error found, return true, false otherwise</returns>
        public static bool IsError(JsonDocument? json, string json_error_code_field, out JsonElement? errorData)
        {
            errorData = null;
            if (json.RootElement.TryGetProperty("error", out JsonElement jsonElem)) {
                if (jsonElem.TryGetProperty(json_error_code_field, out _))
                    errorData = jsonElem;
                return true;
            }
            return false;
        }
    }
}
