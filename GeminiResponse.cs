using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document_Analyizer_AI
{
    public class GeminiResponse
    {
        public Candidate[] candidates { get; set; }

        public class Candidate
        {
            public Content content { get; set; }
            public string finishReason { get; set; }
            public double avgLogprobs { get; set; }
        }

        public class Content
        {
            public Part[] parts { get; set; }
            public string role { get; set; }
        }

        public class Part
        {
            public string text { get; set; }
        }
    }
}
