using AssertUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Document_Analyizer_AI.Story;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Document_Analyizer_AI
{
    class PromptBuilder
    {
        public static string BuildPrompt(string? previousOutput, string currentChapter)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Analyze the chapter below. Provide:");
            sb.AppendLine("1. A concise summary of the chapter's plot and events.");
            sb.AppendLine("2. A list of new characters introduced in this chapter, including their names and any notable traits or events related to them.");
            sb.AppendLine("3. A description of the setting and atmosphere introduced in this chapter.");
            sb.AppendLine("4. Any significant themes or motifs that are present in this chapter.");
            sb.AppendLine("Return using the following JSON format:");
            sb.AppendLine("{");
            sb.AppendLine("  \"summary\": \"...\",");
            sb.AppendLine("  \"new_characters\": [");
            sb.AppendLine("    {");
            sb.AppendLine("      \"name\": \"...\",");
            sb.AppendLine("      \"traits\": \"...\"");
            sb.AppendLine("    }");
            sb.AppendLine("  ],");
            sb.AppendLine("  \"setting\": \"...\",");
            sb.AppendLine("  \"themes\":  [");
            sb.AppendLine("    {");
            sb.AppendLine("      \"name\": \"...\",");
            sb.AppendLine("    }");
            sb.AppendLine("  ],");
            sb.AppendLine("}");
            sb.AppendLine();

            if(!string.IsNullOrWhiteSpace(previousOutput))
            {
                sb.AppendLine("Previous Output:");
                sb.AppendLine(previousOutput);
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine("As this is the first chapter, the protagonist(s) has not been introduced before, include them explicitly in the new_characters list.");

            }

            sb.AppendLine("Current Chapter:");
            sb.AppendLine(currentChapter);

            return sb.ToString();
        }
    }

    class Program
    {
        static readonly HttpClient CLIENT = new();

        static async Task Main()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            string? apiKey = config["GeminiApiKey"];
            AssertUtil.AssertNotNull(apiKey );// Ensure that the API key is not null or empty
            //Console.WriteLine("Key: " + apiKey);

            string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
        
            string basePath = "Chapters/Chapter 1/";
            Story.SceneData? lastScene = null; // Initialize lastChapter to null for the first iteration

            for (int i = 1; i <= 4; i++)
            {
                    //Get the file, load it.
                string scenePath = Path.Combine(basePath, $"Scene {i}.txt");
                Document sceneDoc = Document.Load(scenePath);
                    // build the prompt and check it
                    // Not entirely sure if json added to prompt is a good format, but it should kinda sorta work.
                string prompt = PromptBuilder.BuildPrompt(lastScene?.ToJson(), sceneDoc.Title + "\r\n\r\n" + sceneDoc.Contents);
                AssertUtil.AssertNotNull(prompt, "Prompt should not be null or empty");

                // Print the prompt to the console for debugging
                Console.WriteLine("Prompt: " + prompt);
                // Ask the user to confirm before sending the request
                Console.WriteLine("Type 'start' and press Enter to send the request to Gemini API. Press Enter without typing 'start' to abort.");
                string? command = Console.ReadLine()?.Trim();

                if (!string.Equals(command, "start", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exiting program.");
                    return;
                }
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new
                                {
                                    text = @prompt
                                }
                            }
                        }
                    }
                };
            
                AssertUtil.AssertNotNull(requestBody); // Confirm a response was received before proceeding

                string json = JsonConvert.SerializeObject(requestBody); // Serialize the request body to JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // Create the HTTP content with the JSON body

                HttpResponseMessage response = await CLIENT.PostAsync(endpoint, content); // Send the POST request to the Gemini API
                string? responseBody = await response.Content.ReadAsStringAsync(); // Read the response body as a string

                // Deserialize the outer response into a JObject
                var outerJson = JObject.Parse(responseBody);

                // Extract the inner JSON string with code fences
                string rawInnerJson = (string)outerJson["candidates"]?[0]?["content"]?["parts"]?[0]?["text"];

                if (string.IsNullOrWhiteSpace(rawInnerJson))
                {
                    throw new Exception("Inner JSON text not found in response.");
                }

                // Remove ```json and ``` fences if present
                string cleanedJson = rawInnerJson.Trim();
                if (cleanedJson.StartsWith("```json"))
                {
                    cleanedJson = cleanedJson.Substring("```json".Length).TrimStart();
                }
                if (cleanedJson.EndsWith("```"))
                {
                    cleanedJson = cleanedJson.Substring(0, cleanedJson.Length - 3).TrimEnd();
                }
            
                Console.WriteLine(cleanedJson); // Print the response body to the console

                // Now deserialize cleanedJson into ChapterAnalysis
                Story.SceneData chapterAnalysis = new(cleanedJson);


            
                // Output the chapter analysis results to the console
                // Could be encapsulated in a method, but for now, let's keep it simple
                Console.WriteLine("Chapter Summary " + chapterAnalysis.Summary); 
                Console.WriteLine("New Characters: " + (chapterAnalysis.NewCharacters?.Count > 0 ? string.Join(", ", chapterAnalysis.NewCharacters.Select(c => c.Name)) : "None"));
                Console.WriteLine("Setting: " + chapterAnalysis.Setting);
                chapterAnalysis.Themes?.ForEach(t => Console.WriteLine("Theme: " + t.Name));

                string folderName = "ChapterResults"; // Folder to store the output files
                Directory.CreateDirectory(folderName); // Ensure folder exists

                string fileName = sceneDoc.Title + "_chapter_summary_output.txt";
                string fullPath = Path.Combine(folderName, fileName); // Combine folder and file name to get the full path

                AssertUtil.AssertNotNull(chapterAnalysis.ToJson()); // don't think this can fail, but let's be safe
                OutputWriter.WriteOutputToFile(chapterAnalysis.ToJson(), fullPath);

                lastScene = chapterAnalysis; // Update lastChapter for the next iteration
            }

            
        }
    }
}