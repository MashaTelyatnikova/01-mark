﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Version_3
{
    public class HtmlTree
    {
        private readonly NodeHtmlTree root;
        private const char SymbolReplacementEmSections = '®';
        private const char SymbolReplacementStrongSections = '§';
       
        public HtmlTree(string text)
        {
            root = new NodeHtmlTree(TypeNodeHtmlTree.Root, text);
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
                                             .Select(p => new NodeHtmlTree(TypeNodeHtmlTree.Paragraph, p))
                                             .ToList();
            root.AddRangeChilds(paragraphs);
        }

        private void AddChildsParagraphs()
        {
            foreach (var paragraph in root.Childs)
            {
                AddChildsParagraph(paragraph);
            }
        }

        private void AddChildsParagraph(NodeHtmlTree nodeHtmlTree)
        {
            var content = nodeHtmlTree.Content;
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
                    if (innerText.ToString() != "")
                        nodeHtmlTree.AddChild(new NodeHtmlTree(TypeNodeHtmlTree.Text, innerText.ToString()));
                    innerText.Clear();

                    NodeHtmlTree child;
                    if (character == SymbolReplacementEmSections)
                    {
                        var em = emSections.Dequeue();
                        child = new NodeHtmlTree(TypeNodeHtmlTree.Em, CutExtremeCharactres(em, 1));
                    }
                    else
                    {
                        var strong = strongSections.Dequeue();

                        child = new NodeHtmlTree(TypeNodeHtmlTree.Strong, CutExtremeCharactres(strong, 2));
                    }
                    nodeHtmlTree.AddChild(child);
                    AddChildsParagraph(child);
                }
            }

            if (innerText.ToString() != "")
                nodeHtmlTree.AddChild(new NodeHtmlTree(TypeNodeHtmlTree.Text, innerText.ToString()));
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
