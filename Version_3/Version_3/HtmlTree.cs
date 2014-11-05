using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Version_3
{
    public class HtmlTree
    {
        private NodeHtmlTree root;
        private static readonly Dictionary<string, string> HtmlRepresentationOfSpecialCharacter = new Dictionary<string, string>()
            {
                    {"<", "&lt;"},
                    {">", "&gt;"},
                    {"\"", "&quot;"},
                    {"©", "&copy;"},
                    {"®", "&reg;"},
                    {"«", "&laquo;"},
                    {"§", "&sect;"},
                    {"¡", "&iexcl;"}
            };
        private readonly Dictionary<string, char> specialSymbolsReplacement = new Dictionary<string, char>()
        {
            {"code", '©'},
            {"screening", '®'},
            {"em", '«'},
            {"strong", '\"'},
            {"underlines", '§'},
            {"between", '¡'}
        };

        private List<string> sectionsCode;
        private List<string> sectionsScreening;

        public HtmlTree(string text)
        {
            this.sectionsCode = new List<string>();
            this.sectionsScreening = new List<string>();
            this.root = new NodeHtmlTree(TypeNodeHtmlTree.Root, text);
            BuildTree(text);
        }

        private void BuildTree(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            var replacementText = ReplaceSpecialCharacters(text);
            replacementText = SaveAndReplaceSectionsCodeAndScreening(replacementText);

            this.root = new NodeHtmlTree(TypeNodeHtmlTree.Root, replacementText);
            DefineNextLevelTree(root);
        }

        private void DefineNextLevelTree(NodeHtmlTree node)
        {
            var content = node.Content;
            var type = node.Type;

            if (type.Equals(TypeNodeHtmlTree.Root))
            {
                var paragraphs = MarkdownSeparator.GetParagraphSections(content)
                                                    .Select(p => new NodeHtmlTree(TypeNodeHtmlTree.Paragraph, p))
                                                    .ToList();
                node.AddChilds(paragraphs);
                foreach (var paragraph in paragraphs)
                {
                    DefineNextLevelTree(paragraph);
                }
            }
            else
            {
                var childs = GetEmStrongTextChilds(content).ToList();
                node.AddChilds(childs);

                foreach (var child in childs.Where(ch => !ch.Type.Equals(TypeNodeHtmlTree.Text)))
                {
                    DefineNextLevelTree(child);
                }
            }
        }

        private string SaveAndReplaceSectionsCodeAndScreening(string text)
        {
            sectionsCode = MarkdownSeparator.GetCodeSections(text);

            var replacementText = ReplaceSectionsOnSpecialCharacter(text, sectionsCode, specialSymbolsReplacement["code"]);

            sectionsScreening = MarkdownSeparator.GetSreeningSections(replacementText);

            replacementText = ReplaceSectionsOnSpecialCharacter(replacementText, sectionsScreening, specialSymbolsReplacement["screening"]);

            return replacementText;
        }

        private IEnumerable<NodeHtmlTree> GetEmStrongTextChilds(string content)
        {
            //Делаю замену, что были как можно проще регулярные выражения, в которых можно гораздо меньше запутаться и сделать баг
            var underlinesBetweenLettersAndDigits = MarkdownSeparator.GetUnderlinwBetweenLettersAndDigits(content);
            var replacementContent = ReplaceSectionsOnSpecialCharacter(content, underlinesBetweenLettersAndDigits,
                specialSymbolsReplacement["between"]);

            var strongSections = MarkdownSeparator.GetStrongSections(replacementContent);
            replacementContent = ReplaceSectionsOnSpecialCharacter(replacementContent, strongSections,
                specialSymbolsReplacement["underlines"]);

            var emSections = MarkdownSeparator.GetEmSections(replacementContent);
            replacementContent = ReplaceSectionsOnSpecialCharacter(replacementContent, emSections,
                specialSymbolsReplacement["em"]);


            var currentTextChild = new StringBuilder();
            foreach (var character in replacementContent)
            {
                if (character != specialSymbolsReplacement["em"] && character != specialSymbolsReplacement["between"] &&
                                character != specialSymbolsReplacement["underlines"])
                {
                    currentTextChild.Append(character);
                }
                else
                {
                    if (currentTextChild.ToString() != "")
                        yield return new NodeHtmlTree(TypeNodeHtmlTree.Text, currentTextChild.ToString());
                    currentTextChild.Clear();

                    if (character == specialSymbolsReplacement["em"])
                    {

                        var em = emSections[0];
                        em = ReplaceSpecialCharactersOnSections(em, specialSymbolsReplacement["between"],
                            underlinesBetweenLettersAndDigits);
                        em = ReplaceSpecialCharactersOnSections(em, specialSymbolsReplacement["underlines"],
                                strongSections);
                        emSections.RemoveAt(0);
                        yield return new NodeHtmlTree(TypeNodeHtmlTree.Em, em);
                    }
                    if (character == specialSymbolsReplacement["between"])
                    {
                        currentTextChild.Append(underlinesBetweenLettersAndDigits[0]);
                        underlinesBetweenLettersAndDigits.RemoveAt(0);
                    }
                    if (character == specialSymbolsReplacement["underlines"])
                    {
                        yield return new NodeHtmlTree(TypeNodeHtmlTree.Strong, strongSections[0]);
                        strongSections.RemoveAt(0);
                    }
                }
            }

            if (currentTextChild.ToString() != "")
                yield return new NodeHtmlTree(TypeNodeHtmlTree.Text, currentTextChild.ToString());
        }

        private static string ReplaceSectionsOnSpecialCharacter(string text, List<string> sections, char specialCharacter)
        {
            return sections.Aggregate(text, (current, c) => current.Replace(c, specialCharacter + ""));
        }

        private static string ReplaceSpecialCharactersOnSections(string text, char specialCharacter, List<string> sections)
        {
            var result = new StringBuilder();
            foreach (var t in text)
            {
                if (t == specialCharacter)
                {
                    result.Append(sections[0]);
                    sections.RemoveAt(0);
                }
                else
                {
                    result.Append(t);
                }
            }

            return result.ToString();
        }

        private static string ReplaceSpecialCharacters(string text)
        {
            return HtmlRepresentationOfSpecialCharacter
                    .Keys
                    .Aggregate(text, (current, v) => current.Replace(v, HtmlRepresentationOfSpecialCharacter[v]));
        }

        public override string ToString()
        {
            var htmlSectionsCode = sectionsCode.Select(code => string.Format("<code>{0}</code>", code.Substring(1, code.Length - 2))).ToList();

            var htmlSectionsScreening = sectionsScreening.Select(scr => scr.Substring(1)).ToList();

            var result = root.ToString();
            result = ReplaceSpecialCharactersOnSections(result, specialSymbolsReplacement["code"], htmlSectionsCode);
            result = ReplaceSpecialCharactersOnSections(result, specialSymbolsReplacement["screening"], htmlSectionsScreening);

            return result;
        }
    }
}
