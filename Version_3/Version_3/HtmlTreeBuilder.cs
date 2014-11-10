using System.Collections.Generic;
using System.Text;

namespace Version_3
{
    public static class HtmlTreeBuilder
    {
        private const char SymbolReplacementEmSections = '®';
        private const char SymbolReplacementStrongSections = '§';

        public static HtmlTree Build(string text)
        {
            var root = new HtmlTreeNode(TypeNodeHtmlTree.Root);
            AddParagraphsToRoot(root, text);

            return new HtmlTree(root);
        }

        private static void AddParagraphsToRoot(HtmlTreeNode root, string rootContent)
        {
            foreach (var paragraphContent in MarkdownSections.GetParagraphSections(rootContent))
            {
                var paragraph = new HtmlTreeNode(TypeNodeHtmlTree.Paragraph);
                paragraph.AddChilds(GetNodeChilds(paragraphContent));
                root.AddChild(paragraph);
            }
        }

        private static IEnumerable<HtmlTreeNode> GetNodeChilds(string nodeContent)
        {
            var emSections = new Queue<string>(MarkdownSections.GetEmSections(nodeContent));

            nodeContent = MarkdownSections.ReplaceSectionsOnSpecialCharacter(nodeContent,
                MarkdownSections.WrapSectionsInOriginalSeparator(emSections, "_"), SymbolReplacementEmSections);

            var strongSections = new Queue<string>(MarkdownSections.GetStrongSections(nodeContent));

            nodeContent = MarkdownSections.ReplaceSectionsOnSpecialCharacter(nodeContent,
                MarkdownSections.WrapSectionsInOriginalSeparator(strongSections, "__"), SymbolReplacementStrongSections);

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
                    var content = string.Empty;
                    if (character == SymbolReplacementEmSections)
                    {
                        content = emSections.Dequeue();
                        child = new HtmlTreeNode(TypeNodeHtmlTree.Em);
                    }
                    else
                    {
                        content = strongSections.Dequeue();

                        child = new HtmlTreeNode(TypeNodeHtmlTree.Strong);
                    }

                    child.AddChilds(GetNodeChilds(content));
                    yield return child;
                }
            }

            if (innerText.Length != 0)
                yield return new HtmlTreeNode(TypeNodeHtmlTree.Text, innerText.ToString());
        }
    }
}
