using Newtonsoft.Json;

namespace Document_Analyizer_AI
{

    public static class OutputWriter
    {
        public static GeminiResponse? ParseResponse(string jsonResponse)
        {
            // Deprecated?
            return JsonConvert.DeserializeObject<GeminiResponse>(jsonResponse);
        }

        public static void WriteOutputToFile(string text, string path)
        {
            File.WriteAllText(path, text);
        }
    }
}
