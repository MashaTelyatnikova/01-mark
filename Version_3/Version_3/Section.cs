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
    }
}
