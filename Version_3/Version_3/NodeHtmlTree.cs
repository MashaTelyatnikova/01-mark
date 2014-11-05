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
        private TypeNodeHtmlTree typeNode;
        private string content;
        private List<NodeHtmlTree> childs;

        public NodeHtmlTree(TypeNodeHtmlTree typeNode, string content)
        {
            this.typeNode = typeNode;
            this.content = content;
            this.childs = new List<NodeHtmlTree>();
        }

        public void AddChild(NodeHtmlTree child)
        {
            childs.Add(child);
        }

        public IEnumerable<NodeHtmlTree> GetChilds()
        {
            return childs;
        }

        public override string ToString()
        {
            if (typeNode.Equals(TypeNodeHtmlTree.Text))
                return content;

            var result = new StringBuilder();
            foreach (var child in childs)
            {
                result.Append(child.ToString());
            }

            return string.Format("<{0}>{1}</{0}>", GetTag(), result.ToString());
        }

        private string GetTag()
        {
            switch (typeNode)
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
