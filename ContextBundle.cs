using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Document_Analyizer_AI.Story;

namespace Document_Analyizer_AI
{
    internal class ContextBundle
    {
        public StoryData StoryData { get; set; } = new();
        public ChapterData ChapterData { get; set; } = new();
        public SceneData SceneData { get; set; } = new();
    }
}
