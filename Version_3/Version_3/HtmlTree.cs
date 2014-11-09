using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Version_3
{
    public class HtmlTree
    {
        private readonly HtmlTreeNode root;
        private const char SymbolReplacementEmSections = '®';
        private const char SymbolReplacementStrongSections = '§';
       
        public HtmlTree(string text)
        {
            root = new HtmlTreeNode(TypeNodeHtmlTree.Root, text);
            Build();
        }

        private void Build()
        {
            AddParagraphsToRoot();
            AddChildsParagraphs();
        }

        private void AddParagraphsToRoot()
        {
            var paragraphs = MarkdownSections.GetParagraphSections(root.Content)
                                             .Select(p => new HtmlTreeNode(TypeNodeHtmlTree.Paragraph, p))
                                             .ToList();
            root.AddChilds(paragraphs);
        }

        private void AddChildsParagraphs()
        {
            foreach (var paragraph in root.Childs)
            {
                AddChildsParagraph(paragraph);
            }
        }

        private void AddChildsParagraph(HtmlTreeNode htmlTreeNode)
        {
            var content = htmlTreeNode.Content;
            var emSections = MarkdownSections.GetEmSections(content);
            content = ReplaceSectionsOnSpecialCharacter(content, emSections, SymbolReplacementEmSections);

            var strongSections = MarkdownSections.GetStrongSections(content);
            content = ReplaceSectionsOnSpecialCharacter(content, strongSections, SymbolReplacementStrongSections);
            var innerText = new StringBuilder();

            foreach (var character in content)
            {
                if (character != SymbolReplacementEmSections && character != SymbolReplacementStrongSections)
                    innerText.Append(character);
                else
                {
                    if (innerText.Length != 0)
                        htmlTreeNode.AddChild(new HtmlTreeNode(TypeNodeHtmlTree.Text, innerText.ToString()));
                    innerText.Clear();

                    HtmlTreeNode child;
                    if (character == SymbolReplacementEmSections)
                    {
                        var em = emSections.Dequeue();
                        child = new HtmlTreeNode(TypeNodeHtmlTree.Em, CutExtremeCharactres(em, 1));
                    }
                    else
                    {
                        var strong = strongSections.Dequeue();

                        child = new HtmlTreeNode(TypeNodeHtmlTree.Strong, CutExtremeCharactres(strong, 2));
                    }
                    htmlTreeNode.AddChild(child);
                    AddChildsParagraph(child);
                }
            }

            if (innerText.Length != 0)
                htmlTreeNode.AddChild(new HtmlTreeNode(TypeNodeHtmlTree.Text, innerText.ToString()));
        }

        private static string ReplaceSectionsOnSpecialCharacter(string text, Queue<string> sections, char specialCharacter)
        {
            return sections.Aggregate(text, (current, c) => current.Replace(c, specialCharacter + ""));
        }

        private static string CutExtremeCharactres(string text, int count)
        {
            return text.Substring(0 + count, text.Length - 2 * count);
        }

        public override string ToString()
        {
           return root.ToString();
        }
    }
}
