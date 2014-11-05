using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Version_3
{
    public class HtmlTree
    {
        private readonly NodeHtmlTree root;
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

        private List<string> sectionsCode;
        private List<string> sectionsScreening;
 
        public HtmlTree(string text)
        {
            this.root = new NodeHtmlTree(TypeNodeHtmlTree.Root, text);
            this.sectionsCode = new List<string>();
            this.sectionsScreening = new List<string>();

            BuildTree(text);
        }

        private void BuildTree(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            var replacementText = ReplaceSpecialCharacters(text);
            replacementText = SaveAndReplaceSectionsCodeAndScreening(replacementText);
            DefineNextLevelTree(root);
        }

        private void DefineNextLevelTree(NodeHtmlTree node)
        {
            
        }

        private string SaveAndReplaceSectionsCodeAndScreening(string text)
        {
            sectionsCode = MarkdownSeparator.GetCodeSections(text);

            var replacementText = ReplaceSectionsOnSpecialCharacter(text, sectionsCode, '©');

            sectionsScreening = MarkdownSeparator.GetSreeningSections(replacementText);

            replacementText = ReplaceSectionsOnSpecialCharacter(replacementText, sectionsScreening, '®');

            return replacementText;
        }

        private static string ReplaceSectionsOnSpecialCharacter(string text, List<string> sections,
            char specialCharacter)
        {
            return sections.Aggregate(text, (current, c) => current.Replace(c, specialCharacter+""));
        }

        private static string ReplaceSpecialCharactersOnSections(string text, char specialCharacter,
            List<string> sections)
        {
            var result = new StringBuilder();
            var index = 0;
            foreach (var t in text)
            {
                if (t == specialCharacter)
                {
                    result.Append(sections[index]);
                    index++;
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
            var htmlSectionsCode =
                sectionsCode.Select(code => string.Format("<code>{0}</code>", code.Substring(1, code.Length - 2)))
                    .ToList();
            var htmlSectionsScreening = sectionsScreening.Select(scr => scr.Substring(1)).ToList();

            var result = root.ToString();
            result = ReplaceSpecialCharactersOnSections(result, '©', htmlSectionsCode);
            result = ReplaceSpecialCharactersOnSections(result, '®', htmlSectionsScreening);

            return result;
        }
    }
}
