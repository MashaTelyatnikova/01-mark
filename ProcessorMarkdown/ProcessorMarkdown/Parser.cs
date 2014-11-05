using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProcessorMarkdown
{
    public static class Parser
    {
        private static readonly Regex SectionsCode = new Regex("(`[^`]+`)");
        private static readonly Regex SreeningSections = new Regex(@"[\\].{1}");

        public static string ParseToHtml(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            text = ReplaceSpecialCharacters(text);
            var sectionsCode = GetWordsMatchesRegex(text, SectionsCode).ToList();
            text = ReplaceSectionsOnSpecialCharacter(text, sectionsCode, "©");
            var sectionsScreening = GetWordsMatchesRegex(text, SreeningSections).ToList();
            text = ReplaceSectionsOnSpecialCharacter(text, sectionsScreening, "®");

            var tree = new HtmlTree(text);
            var result = tree.ConvertTreeToText();

            sectionsCode = sectionsCode.Select(i => WrapLineInTags(i.Substring(1, i.Length - 2), "code")).ToList();
            sectionsScreening = sectionsScreening.Select(i => i.Substring(1)).ToList();
            result = ReplaceSpecialCharactersOnSections(result, sectionsCode, '©');
            result = ReplaceSpecialCharactersOnSections(result, sectionsScreening, '®');
            return result;
        }

        private static string WrapLineInTags(string line, string tag)
        {
            return string.Format("<{0}>{1}</{0}>", tag, line);
        }

        private static string ReplaceCodeAndScreeningOnSpecialChars(string text)
        {
            var sectionsCode = GetWordsMatchesRegex(text, SectionsCode).ToList();
            text = ReplaceSectionsOnSpecialCharacter(text, sectionsCode, "©");

            var sectionsScreening = GetWordsMatchesRegex(text, SreeningSections).ToList();
            text = ReplaceSectionsOnSpecialCharacter(text, sectionsScreening, "®");
            

            return text;
        }

        private static string ReplaceSpecialCharactersOnSectionsCodeAndScreening(string initialText, string resultText)
        {

            var sectionsScreening = GetWordsMatchesRegex(initialText, SreeningSections).Select(code => string.Format("<code>{0}</code>", code.Substring(1, code.Length - 2))).ToList();
            var sectionsCode = GetWordsMatchesRegex(initialText, SectionsCode).Select(scr => scr.Substring(1)).ToList();

            resultText = ReplaceSpecialCharactersOnSections(resultText, sectionsCode, '©');
            resultText = ReplaceSpecialCharactersOnSections(resultText, sectionsScreening, '®');

            return resultText;
        }

        private static string ReplaceSpecialCharacters(string text)
        {
            var htmlRepresentationOfSpecialCharacter = new Dictionary<string, string>()
            {
                    {"<", "&lt;"},
                    {">", "&gt;"},
                    {"\"", "&quot;"},
                    {"©", "&copy;"},
                    {"®", "&reg;"},
                    {"«", "&laquo;"},
                    {"§", "&sect;"},
                    {"¡", "&iexcl;"}
            };

            return htmlRepresentationOfSpecialCharacter
                    .Keys
                    .Aggregate(text, (current, v) => current.Replace(v, htmlRepresentationOfSpecialCharacter[v]));
        }

        private static IEnumerable<string> GetWordsMatchesRegex(string text, Regex regex)
        {
            var result = regex.Matches(text);

            return from object v in result select v.ToString();
        }

        private static string ReplaceSectionsOnSpecialCharacter(string text, IEnumerable<string> sections, string ch)
        {
            return sections.Aggregate(text, (current, c) => current.Replace(c, ch));
        }

        private static string ReplaceSpecialCharactersOnSections(string text, IReadOnlyList<string> sections, char specialCharacter)
        {
            var result = new StringBuilder();
            var index = 0;
            foreach (var t in text)
            {
                if (t == specialCharacter)
                {
                    result.Append(sections[index]);
                    index++;
                }
                else
                {
                    result.Append(t);
                }
            }

            return result.ToString();
        }
    }
}
