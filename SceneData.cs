using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssertUtils;

namespace Document_Analyizer_AI
{
    internal class StoryData
    {
        public class Character
        {
            public string Name { get; set; } = "";
            public string Traits { get; set; } = "";
        }

        public class Theme
        {
            public string Name { get; set; }
        }
        // Strictly speaking, this isn't just for chapter analysis, but it is good enough for now.
        // Hence document being the actual input of the program.
        // May want to add different stuff, for songs, poems, essays, etc. later on.
        public class SceneData
        {
            public string summary { get; set; }
            public List<Character> new_characters { get; set; }
            public string setting { get; set; }
            public List<Theme> themes { get; set; }

            public SceneData() 
        {
            // Parameterless constructor for deserialization or default instantiation
            new_characters = new List<Character>();
            themes = new List<Theme>();
        }

        public SceneData(string? responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                // Initialize with defaults instead of throwing
                summary = "";
                new_characters = new List<Character>();
                setting = "";
                themes = new List<Theme>();
                return;
            }

            var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SceneData>(responseBody);
            if (jsonResponse != null)
            {
                summary = jsonResponse.summary;
                new_characters = jsonResponse.new_characters ?? new List<Character>();
                setting = jsonResponse.setting;
                themes = jsonResponse.themes ?? new List<Theme>();
            }
            else
            {
                // Fallback to defaults instead of throwing exception
                summary = "";
                new_characters = new List<Character>();
                setting = "";
                themes = new List<Theme>();
            }
        }

            public string ToJson()
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            }
        }
        public class ChapterData
        {
            public string Title { get; set; } = "";
            public string Summary { get; set; } = "";
            public List<Character> Characters { get; set; } = [];
            public string Setting { get; set; } = "";
            public List<Theme> Themes { get; set; } = [];
            
            public List<SceneData> Scenes = [];
            public ChapterData()
            {
                
            }
            //public ChapterData(string? responseBody)
            //{
            //    var finalSummary = string.Join(" ", sceneDataArray.Select(s => s.Summary));
            //
            //    var allCharacters = sceneDataArray
            //        .SelectMany(s => s.NewCharacters)
            //        .GroupBy(c => c.Name)
            //        .Select(g => g.First())
            //        .ToList();
            //
            //    var allThemes = sceneDataArray
            //        .SelectMany(s => s.Themes)
            //        .GroupBy(t => t.Name)
            //        .Select(g => g.First())
            //        .ToList();
            //
            //    var finalSetting = string.Join(" ", sceneDataArray.Select(s => s.Setting));
            //
            //    var chapterData = new
            //    {
            //        summary = finalSummary,
            //        characters = allCharacters,
            //        setting = finalSetting,
            //        themes = allThemes
            //    };
            //}
        }
    }
}
