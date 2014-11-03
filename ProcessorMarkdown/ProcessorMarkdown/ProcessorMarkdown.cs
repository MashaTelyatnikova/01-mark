using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProcessorMarkdown
{
    public class ProcessorMarkdown
    {
        private readonly string textMarkdown;
        private static readonly Dictionary<string, string> HtmlRepresentationOfSpecialCharacter = new Dictionary<string, string>()
        {
            {"<", "&lt;"},
            {">", "&gt;"},
            {"\"", "&quot;"},
            {"©", "&copy;"}
        };

        public ProcessorMarkdown(string textMarkdown)
        {
            this.textMarkdown = textMarkdown;
        }

        public string GetResultOfProcessing()
        {
            if (string.IsNullOrEmpty(textMarkdown)) return string.Empty;

            var paragraphs = GetParagraphs(textMarkdown);

            return string.Join("", paragraphs);
        }

        private static IEnumerable<string> GetParagraphs(string textMarkdown)
        {
            return Regex.Split(textMarkdown, @"\n\s*\n")
                        .Select(ReplaceSpecialCharacters)
                        .Select(ReplaceSelectionsParagraphOnTags)
                        .Select(paragraph => string.Format("<p>{0}</p>", paragraph));
        }

        private static string ReplaceSpecialCharacters(string paragraph)
        {
            return HtmlRepresentationOfSpecialCharacter
                    .Keys
                    .Aggregate(paragraph, (current, v) => current.Replace(v, HtmlRepresentationOfSpecialCharacter[v]));
        }

        private static string ReplaceSelectionsParagraphOnTags(string paragraph)
        {
            var automate = new AutomateReplacement();

            return automate.GetResult(paragraph);
        }
    }
}
