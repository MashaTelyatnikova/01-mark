using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Version_3
{
    public static class MarkdownParser
    {
        private const char SymbolRemplacementCodeSections = '©';
       
        public static string ParseToHtml(string text)
        {
            if (text == null) throw new ArgumentException("Invalid argument (Null)");

            text = HttpUtility.HtmlEncode(text);
            var codeSections = MarkdownSections.GetCodeSections(text);
            
            text = ReplaceCodeSectionsOnSpecialChar(text, codeSections);
             
            var htmlTree = new HtmlTree(text).ToString();
            htmlTree = ReplaceScreeningSections(htmlTree);
            htmlTree = ReplaceSpecialCharOnCodeSectionsWrappedTags(htmlTree, WrapCodeSectionsInTags(codeSections));

            return htmlTree;
        }

        private static string ReplaceSpecialCharOnCodeSectionsWrappedTags(string text, Queue<string> codeSections)
        {
            var result = new StringBuilder();
            foreach (var c in text)
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

        private static string ReplaceScreeningSections(string text)
        {
            var screeningSections = MarkdownSections.GetSreeningSections(text);
            return screeningSections.Aggregate(text, (current, c) => current.Replace(c, c.Substring(1)));
        }

        private static string ReplaceCodeSectionsOnSpecialChar(string text, Queue<string> codeSections)
        {
            return codeSections.Aggregate(text, (current, c) => current.Replace(c, SymbolRemplacementCodeSections + ""));
        }

        private static Queue<string> WrapCodeSectionsInTags(Queue<string> codeSections)
        {
            return 
                new Queue<string>(codeSections.Select(code => string.Format("<code>{0}</code>",code.Substring(1, code.Length - 2))));
        }
    }
}
