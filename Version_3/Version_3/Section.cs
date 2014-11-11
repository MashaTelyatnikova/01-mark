namespace Version_3
{
    public class Section
    {
        public string LineWithMarkers { get; private set; }
        public string LineWithoutMarkers { get; private set; }

        public Section(string lineWithMarkers, string lineWithoutMarkers)
        {
            LineWithMarkers = lineWithMarkers;
            LineWithoutMarkers = lineWithoutMarkers;
        }

        public Section WrapInTag(string tag)
        {
            return new Section(string.Format("<{0}>{1}</{0}>", tag, LineWithMarkers),
                string.Format("<{0}>{1}</{0}>", tag, LineWithoutMarkers));
        }
    }
}
