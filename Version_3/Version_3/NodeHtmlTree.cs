using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Version_3
{
    public class NodeHtmlTree
    {
        public TypeNodeHtmlTree Type { get; private set; }
        public string Content { get; private set; }
        private readonly List<NodeHtmlTree> childs;

        public NodeHtmlTree(TypeNodeHtmlTree type, string content)
        {
            this.Type = type;
            this.Content = content;
            this.childs = new List<NodeHtmlTree>();
            CutContent();
        }

        private void CutContent()
        {
            switch (Type)
            {
                case TypeNodeHtmlTree.Em:
                    {
                        Content = Content.Substring(1, Content.Length - 2);
                        break;
                    }
                case TypeNodeHtmlTree.Strong:
                    {
                        Content = Content.Substring(2, Content.Length - 4);
                        break;
                    }
                default:
                    {
                        Content = Content;
                        break;
                    }
            }
        }

        public void AddChild(NodeHtmlTree child)
        {
            childs.Add(child);
        }

        public void AddChilds(IEnumerable<NodeHtmlTree> ch)
        {
            this.childs.AddRange(ch);
        }

        public IEnumerable<NodeHtmlTree> GetChilds()
        {
            return childs;
        }

        public override string ToString()
        {
            if (Type.Equals(TypeNodeHtmlTree.Text))
                return Content;

            var result = new StringBuilder();
            foreach (var child in childs)
            {
                result.Append(child.ToString());
            }

            return string.Format("<{0}>{1}</{0}>", GetTag(), result.ToString());
        }

        private string GetTag()
        {
            switch (Type)
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
