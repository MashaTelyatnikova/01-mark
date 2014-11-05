using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Version_3
{
    public static class MarkdownParser
    {
        public static string ParseToHtml(string text)
        {
            var htmlTree = new HtmlTree(text);
            return htmlTree.ToString();
        }
    }
}
