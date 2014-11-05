using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorMarkdown
{
    public class NodeHtmlTree
    {
        private readonly TypesNodeHtmlTree type;
        private readonly string contents;
        private readonly List<NodeHtmlTree> childs;

        public NodeHtmlTree(TypesNodeHtmlTree type, string contents)
        {
            this.type = type;
            this.contents = contents;
            this.childs = new List<NodeHtmlTree>();
        }

        public string GetContents()
        {
            return contents;
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
            if (type.Equals(TypesNodeHtmlTree.Text))
                return contents;

            var result = new StringBuilder();
            foreach (var nodeHtmlTree in childs)
            {
                result.Append(nodeHtmlTree.ToString());
            }

            return type.Equals(TypesNodeHtmlTree.Root) ? result.ToString() : string.Format("<{0}>{1}</{0}>", GetTag(), result.ToString());
        }

        private string GetTag()
        {
            switch (type)
            {
                case TypesNodeHtmlTree.Em:
                    return "em";
                case TypesNodeHtmlTree.Paragraph:
                    return "p";
                case TypesNodeHtmlTree.Strong:
                    return "strong";
                default:
                    throw new Exception("no tags for this type");

            }
        }
    }
}
