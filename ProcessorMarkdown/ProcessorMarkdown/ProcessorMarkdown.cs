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
        private readonly Dictionary<string, string> spec = new Dictionary<string, string>()
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
            
            
            var paragraphs = Regex.Split(textMarkdown, @"\n\n").Select(ReplaceSpec).Select(ReplaceSelectionsParagraphOnTags);
            
            return paragraphs.Aggregate("", (current, paragraph) => current + string.Format("<p>{0}</p>", paragraph));
        }

        private string ReplaceSpec(string paragraph)
        {
            return spec.Keys.Aggregate(paragraph, (current, v) => current.Replace(v, spec[v]));
        }

        private static string ReplaceSelectionsParagraphOnTags(string paragraph)
        {
            var automate = new AutomateReplacement();

            return automate.GetResult(paragraph);
        }
    }
}
