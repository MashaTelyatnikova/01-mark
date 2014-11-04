using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ProcessorMarkdown
{
    public static class Processor
    {
        private static readonly Dictionary<string, string> HtmlRepresentationOfSpecialCharacter = new Dictionary<string, string>()
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

        private static readonly Dictionary<string, string> SpecialSymbolsReplacement = new Dictionary<string, string>()
        {
            {"code", "©"},
            {"escape", "®"},
            {"em", "«"},
            {"strong", "\""},
            {"underlines", "§"},
            {"unpaired", "¡"}
        };

        public static string GerHtmlResultProcess(string text)
        {
            //Ужасный код. Думаю, как сделать лучше
            if (string.IsNullOrEmpty(text))
                return "";

            text = ReplaceSpecialCharacters(text);
            var sectionsCode = MarkdownParser.GetSectionsCode(text).ToList();
            var screeningSections = MarkdownParser.GetScreeningSections(text).ToList();
            
            var sectionsUnderlinesBetweenLettersAndDigits =
                MarkdownParser.GetSectionsUnderlinesBetweenLettersAndDigits(text).ToList();
            text = ReplaceSectionsOnSpecialCharacter(text, sectionsUnderlinesBetweenLettersAndDigits,
              SpecialSymbolsReplacement["underlines"]);
           
            var sectionsStrong = MarkdownParser.GetSectionsStrong(text).ToList();
            var sectionsUnpairedDoubleUnderlines = MarkdownParser.GetSectionsUnpairedDoubleUnderlines(text).ToList();

            text = ReplaceSectionsOnSpecialCharacter(text, sectionsCode, SpecialSymbolsReplacement["code"]);
            text = ReplaceSectionsOnSpecialCharacter(text, screeningSections, SpecialSymbolsReplacement["escape"]);
            text = ReplaceSectionsOnSpecialCharacter(text, sectionsStrong, SpecialSymbolsReplacement["strong"]);
            text = ReplaceSectionsOnSpecialCharacter(text, sectionsUnpairedDoubleUnderlines,
                SpecialSymbolsReplacement["unpaired"]);

            var sectionsEm = MarkdownParser.GetSectionsEm(text).ToList();
            text = ReplaceSectionsOnSpecialCharacter(text, sectionsEm, SpecialSymbolsReplacement["em"]);

            var paragraphs = MarkdownParser.GetParagraphs(text)
                                    .Select(paragraph => WrapLineInTags(paragraph, "p"));

            var htmlRepresentationSectionsCode = sectionsCode.Select(ConvertSectionCodeToHtml).ToList();
            var htmlRepresentationScreeningSections = screeningSections.Select(ConvertSectionScreeningToHtml).ToList();
            var htmlRepresentationSectionsStrong = sectionsStrong.Select(ConvertSectionStrongToHtml).ToList();
            
            var htmlRepresentationSectionsEm = sectionsEm.Select(ConvertSectionEmToHtml).ToList();

            var resultHtml = string.Join("", paragraphs);
            resultHtml = ReplaceSpecialCharactersOnSections(resultHtml, htmlRepresentationSectionsCode, SpecialSymbolsReplacement["code"][0]);
            resultHtml = ReplaceSpecialCharactersOnSections(resultHtml, htmlRepresentationScreeningSections,
                SpecialSymbolsReplacement["escape"][0]);
            resultHtml = ReplaceSpecialCharactersOnSections(resultHtml, htmlRepresentationSectionsEm,
                SpecialSymbolsReplacement["em"][0]);
            resultHtml = ReplaceSpecialCharactersOnSections(resultHtml, htmlRepresentationSectionsStrong,
                SpecialSymbolsReplacement["strong"][0]);

            resultHtml = ReplaceSpecialCharactersOnSections(resultHtml, sectionsUnderlinesBetweenLettersAndDigits,
                SpecialSymbolsReplacement["underlines"][0]);

            resultHtml = ReplaceSpecialCharactersOnSections(resultHtml, sectionsUnpairedDoubleUnderlines,
                SpecialSymbolsReplacement["unpaired"][0]);

            return resultHtml;
        }

        private static string ReplaceSpecialCharacters(string text)
        {
            return HtmlRepresentationOfSpecialCharacter
                    .Keys
                    .Aggregate(text, (current, v) => current.Replace(v, HtmlRepresentationOfSpecialCharacter[v]));
        }

        private static string WrapLineInTags(string line, string tag)
        {
            return string.Format("<{0}>{1}</{0}>", tag, line);
        }

        private static string ConvertSectionCodeToHtml(string sectionCode)
        {
            sectionCode = sectionCode.Substring(1, sectionCode.Length - 2);
            return WrapLineInTags(sectionCode, "code");
        }

        private static string ConvertSectionStrongToHtml(string sectionStrong)
        {
            sectionStrong = sectionStrong.Substring(2, sectionStrong.Length - 4);
            return WrapLineInTags(sectionStrong, "strong");
        }

        private static string ConvertSectionScreeningToHtml(string sectionScreening)
        {
            return sectionScreening.Substring(1);
        }

        private static string ConvertSectionEmToHtml(string sectionEm)
        {
            sectionEm = sectionEm.Substring(1, sectionEm.Length - 2);
            return WrapLineInTags(sectionEm, "em");
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
