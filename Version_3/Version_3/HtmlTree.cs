using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Version_3
{
    public class HtmlTree
    {
        private string text;
        private NodeHtmlTree root;
        private const char SymbolRemplacementCodeSections = '©';
        private const char SymbolReplacementEmSections = '®';
        private const char SymbolReplacementStrongSections = '§';
        private Queue<string> codeSections;
       
        public HtmlTree(string text)
        {
            if (text == null) throw new ArgumentException("Invalid argument (Null)");
            
            this.text = HttpUtility.HtmlEncode(text);
            codeSections = new Queue<string>();
            Build();
        }

        private void Build()
        {
            HideCodeSections();
            
            root = new NodeHtmlTree(TypeNodeHtmlTree.Root, text);
            
            if (string.IsNullOrEmpty(text)) return;
            
            AddParagraphsToRoot();
            AddChildsParagraphs();
        }

        private void HideCodeSections()
        {
            codeSections = MarkdownSections.GetCodeSections(text);

            text = codeSections.Aggregate(text, (current, c) => current.Replace(c, SymbolRemplacementCodeSections + ""));
        }

        private void AddParagraphsToRoot()
        {
            var paragraphs = MarkdownSections.GetParagraphSections(text)
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
        
        private string OpenCodeSections(string t)
        {
            WrapCodeSectionsInTags();
            var result = new StringBuilder();
            foreach (var c in t)
            {
                if (c == SymbolRemplacementCodeSections)
                {
                    result.Append(codeSections.Dequeue());
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        private void WrapCodeSectionsInTags()
        {
            codeSections =
                new Queue<string>(codeSections.Select(code => string.Format("<code>{0}</code>", CutExtremeCharactres(code, 1))));
        }

        private static string CutEscape(string t)
        {
            var screeningSections = MarkdownSections.GetSreeningSections(t);

            return screeningSections.Aggregate(t, (current, c) => current.Replace(c, c.Substring(1)));
        }

        public override string ToString()
        {
            var rootString = root.ToString();
            rootString = OpenCodeSections(rootString);
            rootString = CutEscape(rootString);
            return rootString;
        }
    }
}
