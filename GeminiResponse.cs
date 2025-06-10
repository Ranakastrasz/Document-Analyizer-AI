using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document_Analyizer_AI
{
    public class GeminiResponse
    {
        public Candidate[]? Candidates { get; set; }

        public class Candidate
        {
            public Content? Content { get; set; }
            public string? FinishReason { get; set; }
            public double? AvgLogprobs { get; set; }
        }

        public class Content
        {
            public Part[] Parts { get; set; } = [];
            public string Role { get; set; } = "";
        }

        public class Part
        {
            public string Text { get; set; } = "";
        }
    }
}
