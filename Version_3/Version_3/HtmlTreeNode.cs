using System;
using System.Collections.Generic;
using System.Text;

namespace Version_3
{
    public class HtmlTreeNode
    {
        public string Content { get; private set; }
        public List<HtmlTreeNode> Childs { get; private set; }

        private readonly TypeNodeHtmlTree type;

        public HtmlTreeNode(TypeNodeHtmlTree type, string content)
        {
            this.type = type;
            Content = content;
            Childs = new List<HtmlTreeNode>();
        }

        public void AddChild(HtmlTreeNode child)
        {
            Childs.Add(child);
        }

        public void AddChilds(IEnumerable<HtmlTreeNode> childsNodes)
        {
            Childs.AddRange(childsNodes);
        }

        public override string ToString()
        {
            if (type.Equals(TypeNodeHtmlTree.Text))
                return Content;

            var result = new StringBuilder();
            foreach (var child in Childs)
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
