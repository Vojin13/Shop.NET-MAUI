using Newtonsoft.Json.Linq;

namespace Android_Ispit.DTO
{
    public static class ApiErrorHelper
    {
        // NestJS returns "message" sometimes as a string (e.g. Unauthorized), sometimes as an array
        // of strings (class-validator errors) - that's why it's parsed at runtime instead of a fixed DTO.
        public static string Parse(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return "Error communicating with the server.";

            try
            {
                var json = JObject.Parse(content);
                var message = json["message"];
                if (message == null)
                    return "Error communicating with the server.";

                if (message.Type == JTokenType.Array)
                    return string.Join(", ", message.ToObject<List<string>>() ?? new List<string>());

                return message.ToString();
            }
            catch
            {
                return "Error communicating with the server.";
            }
        }
    }
}
