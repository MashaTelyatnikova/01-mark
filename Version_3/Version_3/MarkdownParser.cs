using System;
using System.Linq;
using System.Web;

namespace Version_3
{
    public static class MarkdownParser
    {
        private const char SymbolRemplacementCodeSections = '©';

        public static string ParseToHtml(string text)
        {
            if (text == null) throw new ArgumentNullException();

            text = ReplaceSpecialCharacters(text);
            var codeSections = MarkdownSections.GetCodeSections(text);

            text = MarkdownSections.ReplaceSectionsOnSpecialCharacter(text,
                MarkdownSections.WrapSectionsInOriginalSeparator(codeSections, "`"), SymbolRemplacementCodeSections);

            var htmlTree = HtmlTreeBuilder.Build(text).ToString();
            htmlTree = ReplaceScreeningSections(htmlTree);

            htmlTree = MarkdownSections.ReplaceSymbolOnSections(htmlTree,
                MarkdownSections.WrapSectionsInTag(codeSections, "code"), SymbolRemplacementCodeSections);

            return htmlTree;
        }

        private static string ReplaceSpecialCharacters(string text)
        {
            return HttpUtility.HtmlEncode(text);
        }

        private static string ReplaceScreeningSections(string text)
        {
            var screeningSections = MarkdownSections.GetSreeningSections(text);
            return screeningSections.Aggregate(text, (current, c) => current.Replace(c, c.Substring(1)));
        }
    }
}
