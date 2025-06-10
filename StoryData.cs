using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssertUtils;

namespace Document_Analyizer_AI.Story
{
    public class StoryData
    {
        public List<ChapterData> Chapters { get; set; } = [];
        public string Title { get; set; } = ""; // Quite useful to have, for context and identification.
        public string Author { get; set; } = ""; // And no reason not to have this, if it is available.
        public string Genre { get; set; } = ""; // Well, dunno if i'm going to use this, but it is a good idea to have it.
        public string Description { get; set; } = ""; // This at least seems manditory, as it often has useful context about the story.
        public StoryData() { }


        // will need a constructor that takes a book file or a directory of chapters/scenes, and loads them into the StoryData object.
        // Gonna hardcode a lot of the pieces for now.

        public StoryData(string title, string author, string genre, string description)
        {
            Title = title;
            Author = author;
            Genre = genre;
            Description = description;
        }
    }

    public class Character
    {
        public string Name { get; set; } = "";
        public string Traits { get; set; } = "";
    }

    public class Theme
    {
        public string Name { get; set; } = "";
    }
    // Strictly speaking, this isn't just for chapter analysis, but it is good enough for now.
    // Hence document being the actual input of the program.
    // May want to add different stuff, for songs, poems, essays, etc. later on.
    public class SceneData
    {
        public string RawText { get; set; } = ""; // The raw text of the scene, for reference.
        public string Summary { get; set; } = ""; // A summary of the scene, once generated.
        public List<Character> NewCharacters { get; set; } = []; // A list of new characters introduced in the scene.
        public List<Character> ExistingCharacters { get; set; } = []; // A list of characters have changed in some way, or have been mentioned in the scene.
        public string Setting { get; set; } = "";
        public List<Theme> Themes { get; set; } = [];

        public SceneData() 
    {
        // Parameterless constructor for deserialization or default instantiation
    }

    public SceneData(string? responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            // Initialize with defaults instead of throwing
            Summary = "";
            NewCharacters = [];
            Setting = "";
            Themes = [];
            return;
        }

        var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SceneData>(responseBody);
        if (jsonResponse != null)
        {
            Summary = jsonResponse.Summary;
            NewCharacters = jsonResponse.NewCharacters ?? [];
            Setting = jsonResponse.Setting;
            Themes = jsonResponse.Themes ?? new List<Theme>();
        }
        else
        {
            // Fallback to defaults instead of throwing exception
            Summary = "";
            NewCharacters = new List<Character>();
            Setting = "";
            Themes = [];
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
