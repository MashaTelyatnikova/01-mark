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

            text = MarkdownSections.ReplaceSectionsWithMarkersOnSpecialCharacter(text,
                codeSections, SymbolRemplacementCodeSections);

            var htmlTree = HtmlTreeBuilder.Build(text).ToString();
            htmlTree = MarkdownSections.ReplaceScreeningSections(htmlTree);

            htmlTree = MarkdownSections.ReplaceSymbolOnLineSectionsWithoutMarkers(htmlTree,
                MarkdownSections.WrapSectionsInTag(codeSections, "code"), SymbolRemplacementCodeSections);

            return htmlTree;
        }

        private static string ReplaceSpecialCharacters(string text)
        {
            return HttpUtility.HtmlEncode(text);
        }
    }
}
