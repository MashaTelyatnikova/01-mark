using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Version_3
{
    public class HtmlTreeNode
    {
        private readonly string content;
        private readonly List<HtmlTreeNode> childs;
        private readonly TypeNodeHtmlTree type;

        public HtmlTreeNode(TypeNodeHtmlTree type, string content)
        {
            this.type = type;
            this.content = content;
            childs = new List<HtmlTreeNode>();
        }

        public HtmlTreeNode(TypeNodeHtmlTree type, IEnumerable<HtmlTreeNode> childs)
        {
            this.type = type;
            content = string.Empty;
            this.childs = childs.ToList();
        }

        public override string ToString()
        {
            if (type.Equals(TypeNodeHtmlTree.Text))
                return content;

            var result = new StringBuilder();
            foreach (var child in childs)
            {
                result.Append(child);
            }

            return string.Format("<{0}>{1}</{0}>", GetTag(), result);
        }

        private string GetTag()
        {
            switch (type)
            {
                case TypeNodeHtmlTree.Em:
                    return "em";
                case TypeNodeHtmlTree.Paragraph:
                    return "p";
                case TypeNodeHtmlTree.Strong:
                    return "strong";
                case TypeNodeHtmlTree.Root:
                    return "body";
                default:
                    throw new Exception("no tags for this type");

            }
        }
    }
}
