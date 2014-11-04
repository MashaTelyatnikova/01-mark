using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProcessorMarkdown
{
    public class HtmlTree
    {
        private readonly string text;
        private readonly NodeHtmlTree root;
        public HtmlTree(string text)
        {
            this.text = text;
            root = new NodeHtmlTree(TypesNodeHtmlTree.Root, String.Empty);
            Build();
        }
        private static readonly Dictionary<string, string> SpecialSymbolsReplacement = new Dictionary<string, string>()
        {
            {"code", "©"},
            {"escape", "®"},
            {"em", "«"},
            {"strong", "\""},
            {"underlines", "§"},
            {"unpaired", "¡"}
        };

        private void Build()
        {
            AddParagraphs();
            foreach (var child in root.GetChilds())
            {
                DefineNextLevel(child);
            }
        }

        private void DefineNextLevel(NodeHtmlTree node)
        {
            var contents = node.GetContents();
            var emSections = GetEmSections(contents).ToList();
            contents = emSections.Aggregate(contents, (current, c) => current.Replace(c, SpecialSymbolsReplacement["em"]));
            var strongSections = GetStrongSections(contents).ToList();
            contents = strongSections.Aggregate(contents, (current, c) => current.Replace(c, SpecialSymbolsReplacement["strong"]));

            var index1 = 0;
            var index2 = 0;
            var innerText = "";
         
            foreach (var content in contents)
            {
                if (content + "" != SpecialSymbolsReplacement["em"] &&
                    content + "" != SpecialSymbolsReplacement["strong"])
                    innerText += content;
                else
                {
                    if (innerText != "")
                        node.AddChild(new NodeHtmlTree(TypesNodeHtmlTree.Text, innerText));
                    
                    innerText = "";
                    if (content + "" == SpecialSymbolsReplacement["em"])
                    {
                        var e1 = emSections[index1];
                        var node1 = new NodeHtmlTree(TypesNodeHtmlTree.Em, e1.Substring(1, e1.Length - 2));
                        
                        node.AddChild(node1);
                        DefineNextLevel(node1);
                        index1++;
                    }
                    else 
                    {
                        var e1 = strongSections[index2];
                        var node1 = new NodeHtmlTree(TypesNodeHtmlTree.Strong, e1.Substring(2, e1.Length - 4));
                        node1.AddChild(new NodeHtmlTree(TypesNodeHtmlTree.Text, e1.Substring(2, e1.Length - 4)));
                        node.AddChild(node1);
                        index2++;
                    }
                }
            }

            if (innerText != "")
            {
                node.AddChild(new NodeHtmlTree(TypesNodeHtmlTree.Text, innerText));
            }
        }

        private void AddParagraphs()
        {
            var paragraphs = Regex.Split(text, @"\n\s*\n").ToList();
            foreach (var paragraph in paragraphs)
            {
                root.AddChild(new NodeHtmlTree(TypesNodeHtmlTree.Paragraph, paragraph));
            }
        }

        private IEnumerable<string> GetEmSections(string t)
        {
            var reg1 = new Regex(@"\s*_([a-zA-Z0-9а-яА-Я\s]+(__[^_]+(__)*)*[a-zA-Z0-9а-яА-Я\s]+)_\s*");
            var r1 = reg1.Matches(t);

            return from object v in r1 select v.ToString().Trim();
        }

        private IEnumerable<string> GetStrongSections(string t)
        {
            var reg1 = new Regex(@"__[^_]+__");
            var r1 = reg1.Matches(t);

            return from object v in r1 select v.ToString().Trim();
        }

        public string ConvertTreeToText()
        {
            return root.ToString();
        }
    }
}
