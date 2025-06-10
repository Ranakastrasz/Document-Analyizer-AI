using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Document_Analyizer_AI
{
    public class Document
    {
        public string Title = "";
        public string Contents = "";

            // Figure out how to actually load a document here
             public static Document Load(string path)
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException($"Document file not found: {path}");

                Document document = new();
                document.Title = Path.GetFileNameWithoutExtension(path);
                document.Title = SanitizeForFilename(document.Title);
                document.Contents = File.ReadAllText(path);
                return document;
            }

        public static string SanitizeForFilename(string input)
        {
            // Replace invalid filename chars with underscore
            var invalidChars = Path.GetInvalidFileNameChars();
            string pattern = "[" + Regex.Escape(new string(invalidChars)) + "]";
            string sanitized = Regex.Replace(input, pattern, "_");

            // Optional: replace spaces with underscores
            sanitized = sanitized.Replace(' ', '_');

            // Optional: limit length
            int maxLength = 100;
            if (sanitized.Length > maxLength)
                sanitized = sanitized.Substring(0, maxLength);

            return sanitized;
        }
    }
}
