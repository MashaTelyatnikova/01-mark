using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ProcessorMarkdown
{
    public class ProcessorMarkdown
    {
        private readonly string text;
        private static readonly Dictionary<string, string> HtmlRepresentationOfSpecialCharacter = new Dictionary<string, string>()
        {
            {"<", "&lt;"},
            {">", "&gt;"},
            {"\"", "&quot;"},
            {"©", "&copy;"},
            {"®", "&reg;"},
            {"«", "&laquo;"}
        };

        private static readonly Dictionary<string, string> HideSection = new Dictionary<string, string>()
        {
            {"code", "©"},
            {"escape", "®"},
            {"underlines", "«"},
            {"doubleunderlines", "\""}
        };


        public ProcessorMarkdown(string text)
        {
            this.text = text;
        }


        public string GetHtmlResultOfProcessing()
        {
            return string.IsNullOrEmpty(text) ? "" : string.Join("", GetParagraphs());
        }

        private IEnumerable<string> GetParagraphs()
        {
            return Regex.Split(text, @"\n\s*\n")
                        .Select(ParseParagraph)
                        .Select(paragraph => string.Format("<p>{0}</p>", paragraph));
        }

        private static string ParseParagraph(string paragraph)
        {
            paragraph = ReplaceSpecialCharacters(paragraph);
            var sectionsCode = GetSectionsCode(paragraph).ToList();
            paragraph = HideSectionsCode(paragraph, sectionsCode);
            var sectionsEscape = GetSectionsEscape(paragraph).ToList();
            paragraph = HideSectionsEscape(paragraph, sectionsEscape);
            paragraph = HideUnderlinesBetweenLettersAndDigits(paragraph);
            paragraph = ReplaceSectionsStrong(paragraph);
            paragraph = HideDoubleUnderlines(paragraph);
            paragraph = ReplaceUnderlinesOnTagEm(paragraph);

            paragraph = OpenSectionsCodeWithTag(paragraph, sectionsCode);
            paragraph = OpenSectionsEscape(paragraph, sectionsEscape);
            paragraph = OpenUnderlinesBetweenLettersAndDigits(paragraph);
            paragraph = OpenDoubleUnderlines(paragraph);
            return paragraph;
        }

        private static string ReplaceUnderlinesOnTagEm(string paragraph)
        {
            var regex = new Regex(@"_[^_]+_");

            foreach (var section in regex.Matches(paragraph))
            {
                var sectionEm = section.ToString();
                sectionEm = string.Format("<em>{0}</em>", sectionEm.Substring(1, sectionEm.Length - 2));
                paragraph = paragraph.Replace(section.ToString(), sectionEm);
            }
            return paragraph;
        }

        private static string OpenUnderlinesBetweenLettersAndDigits(string paragraph)
        {
            return paragraph.Replace(HideSection["underlines"], "_");
        }

        private static string OpenDoubleUnderlines(string paragraph)
        {
            return paragraph.Replace(HideSection["doubleunderlines"], "__");
        }

        private static string HideDoubleUnderlines(string paragraph)
        {
            return paragraph.Replace("__", HideSection["doubleunderlines"]);
        }
        private static string ReplaceSectionsStrong(string paragraph)
        {

            var regex = new Regex(@"__[^_]+__");

            foreach (var section in regex.Matches(paragraph))
            {
                var sectionStrong = section.ToString();
                sectionStrong = string.Format("<strong>{0}</strong>", sectionStrong.Substring(2, sectionStrong.Length - 4));
                paragraph = paragraph.Replace(section.ToString(), sectionStrong);
            }
            return paragraph;
        }

        private static string HideUnderlinesBetweenLettersAndDigits(string paragraph)
        {
            var regex = new Regex(@"[a-zA-ZА-Яа-я0-9]+[_]+[a-zA-ZА-Яа-я0-9]");

            foreach (var v in regex.Matches(paragraph))
            {
                var t = v.ToString().Replace("_", HideSection["underlines"]);
                paragraph = paragraph.Replace(v.ToString(), t);
            }

            return paragraph;
        }

        private static IEnumerable<string> GetSectionsCode(string paragraph)
        {
            var regex = new Regex("(`[^`]+`)");
            var result = regex.Matches(paragraph);

            return from object v in result select v.ToString();
        }

        private static string HideSectionsCode(string paragraph, IEnumerable<string> sectionsCode)
        {
            return sectionsCode.Aggregate(paragraph, (current, p) => current.Replace(p, HideSection["code"]));
        }

        private static string HideSectionsEscape(string paragraph, IEnumerable<string> sectionsEscape)
        {
            return sectionsEscape.Aggregate(paragraph, (current, p) => current.Replace(p, HideSection["escape"]));
        }

        private static IEnumerable<string> GetSectionsEscape(string paragraph)
        {
            var regex = new Regex(@"[\\].{1}");
            var result = regex.Matches(paragraph);

            return from object v in result select v.ToString();
        }

        private static string ReplaceSpecialCharacters(string paragraph)
        {
            return HtmlRepresentationOfSpecialCharacter
                    .Keys
                    .Aggregate(paragraph, (current, v) => current.Replace(v, HtmlRepresentationOfSpecialCharacter[v]));
        }

        private static string OpenSectionsCodeWithTag(string paragraph, List<string> sectionsCode)
        {
            var result = new StringBuilder();
            var index = 0;
            foreach (var ch in paragraph)
            {
                if (ch + "" != HideSection["code"])
                    result.Append(ch);
                else
                {
                    result.Append(string.Format("<code>{0}</code>", sectionsCode[index].Substring(1, sectionsCode[index].Length - 2)));
                    index++;
                }
            }

            return result.ToString();
        }

        private static string OpenSectionsEscape(string paragraph, List<string> sectionsEscape)
        {
            var result = new StringBuilder();
            var index = 0;
            foreach (var ch in paragraph)
            {
                if (ch + "" != HideSection["escape"])
                    result.Append(ch);
                else
                {
                    result.Append(sectionsEscape[index][1]);
                    index++;
                }
            }

            return result.ToString();
        }
    }
}
