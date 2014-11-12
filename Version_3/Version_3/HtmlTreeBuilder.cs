using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Version_3
{
    public static class HtmlTreeBuilder
    {
        private const char SymbolReplacementEmSections = '®';
        private const char SymbolReplacementStrongSections = '§';

        public static HtmlTree Build(string text)
        {
            var root = new HtmlTreeNode(TypeNodeHtmlTree.Root, BuildParagraphs(text));

            return new HtmlTree(root);
        }

        private static IEnumerable<HtmlTreeNode> BuildParagraphs(string rootContent)
        {
            return MarkdownSections.GetParagraphSections(rootContent)
                .Select(paragraphContent =>
                    new HtmlTreeNode(TypeNodeHtmlTree.Paragraph, BuildNodeChilds(paragraphContent.Content)));
        }

        private static IEnumerable<HtmlTreeNode> BuildNodeChilds(string nodeContent)
        {
            var emSections = new Queue<Section>(MarkdownSections.GetEmSections(nodeContent));

            nodeContent = MarkdownSections.ReplaceSectionsWithMarkersOnSpecialCharacter(nodeContent,
                emSections, SymbolReplacementEmSections);

            var strongSections = new Queue<Section>(MarkdownSections.GetStrongSections(nodeContent));

            nodeContent = MarkdownSections.ReplaceSectionsWithMarkersOnSpecialCharacter(nodeContent,
                strongSections, SymbolReplacementStrongSections);

            var innerText = new StringBuilder();

            foreach (var character in nodeContent)
            {
                if (character != SymbolReplacementEmSections && character != SymbolReplacementStrongSections)
                    innerText.Append(character);
                else
                {
                    if (innerText.Length != 0)
                        yield return new HtmlTreeNode(TypeNodeHtmlTree.Text, innerText.ToString());

                    innerText.Clear();

                    HtmlTreeNode child;
                    if (character == SymbolReplacementEmSections)
                    {
                        var emSection = emSections.Dequeue().Content;
                        child = new HtmlTreeNode(TypeNodeHtmlTree.Em, BuildNodeChilds(emSection));
                    }
                    else
                    {
                        var strongSection = strongSections.Dequeue().Content;
                        child = new HtmlTreeNode(TypeNodeHtmlTree.Strong, BuildNodeChilds(strongSection));
                    }

                    yield return child;
                }
            }

            if (innerText.Length != 0)
                yield return new HtmlTreeNode(TypeNodeHtmlTree.Text, innerText.ToString());
        }
    }
}
