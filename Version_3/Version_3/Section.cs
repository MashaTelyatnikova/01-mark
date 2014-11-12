namespace Version_3
{
    public class Section
    {
        public string LineWithMarkers { get; private set; }
        public string Content { get; private set; }

        public Section(string lineWithMarkers, string content)
        {
            LineWithMarkers = lineWithMarkers;
            Content = content;
        }

        public Section WrapContentInTag(string tag)
        {
            return new Section(LineWithMarkers, string.Format("<{0}>{1}</{0}>", tag, Content));
        }
    }
}
