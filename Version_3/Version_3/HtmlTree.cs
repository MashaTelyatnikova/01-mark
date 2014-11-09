namespace Version_3
{
    public class HtmlTree
    {
        private readonly HtmlTreeNode root;
       
        public HtmlTree(HtmlTreeNode root)
        {
            this.root = root;
        }

        public override string ToString()
        {
           return root.ToString();
        }
    }
}
