﻿using System.Collections.Generic;
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
            var root = new HtmlTreeNode(TypeNodeHtmlTree.Root, text);
            AddParagraphsToRoot(root);
            
            return new HtmlTree(root);
        }

        private static void AddParagraphsToRoot(HtmlTreeNode root)
        {
            foreach (var paragraphContent in MarkdownSections.GetParagraphSections(root.Content))
            {
                var paragraph = new HtmlTreeNode(TypeNodeHtmlTree.Paragraph, paragraphContent);
                paragraph.AddChilds(GetNodeChilds(paragraphContent));
                root.AddChild(paragraph);
            }
        }
        
        private static IEnumerable<HtmlTreeNode> GetNodeChilds(string nodeContent)
        {
            var emSections = MarkdownSections.GetEmSections(nodeContent);
            nodeContent = ReplaceSectionsOnSpecialCharacter(nodeContent, WrapSectionsInOriginalSeparator(emSections, "_"), SymbolReplacementEmSections);

            var strongSections = MarkdownSections.GetStrongSections(nodeContent);
            nodeContent = ReplaceSectionsOnSpecialCharacter(nodeContent, WrapSectionsInOriginalSeparator(strongSections, "__"), SymbolReplacementStrongSections);
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
                        var em = emSections.Dequeue();
                        child = new HtmlTreeNode(TypeNodeHtmlTree.Em, em);
                    }
                    else
                    {
                        var strong = strongSections.Dequeue();

                        child = new HtmlTreeNode(TypeNodeHtmlTree.Strong, strong);
                    }
                    child.AddChilds(GetNodeChilds(child.Content));
                    yield return child;
                }
            }

            if (innerText.Length != 0)
                yield return new HtmlTreeNode(TypeNodeHtmlTree.Text, innerText.ToString());
        }

        private static IEnumerable<string> WrapSectionsInOriginalSeparator(IEnumerable<string> sections,
           string originalSeparator)
        {
            return sections.Select(section => string.Format("{0}{1}{0}", originalSeparator, section));
        }

        private static string ReplaceSectionsOnSpecialCharacter(string text, IEnumerable<string> sections, char specialCharacter)
        {
            return sections.Aggregate(text, (current, c) => current.Replace(c, specialCharacter + ""));
        }
    }
}
