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

            text = HttpUtility.HtmlEncode(text);
            var codeSections = MarkdownSections.GetCodeSections(text);

            text = MarkdownSections.ReplaceSectionsWithMarkersOnSpecialCharacter(text,
                codeSections, SymbolRemplacementCodeSections);

            var htmlTree = HtmlTreeBuilder.Build(text).ToString();
            htmlTree = MarkdownSections.ReplaceScreeningSections(htmlTree);

            htmlTree = MarkdownSections.ReplaceSymbolOnContentSections(htmlTree,
                MarkdownSections.WrapContentSectionsInTag(codeSections, "code"), SymbolRemplacementCodeSections);

            return htmlTree;
        }
    }
}
