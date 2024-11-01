namespace Campus_Events.Models
{
    public class EventFilter
    {
        public int StartPage { get; set; } = -1;
        public int ItemsPerPage { get; set; } = 10;
        public string OrderBy { get; set; } = "";
        public IEnumerable<FilterExpression> FilterExpressions { get; set; }
            = new List<FilterExpression>();
    }

    public class FilterExpression
    {
        public string? PropertyName { get; set; }
        public RelationType Relation { get; set; }
        public string? Value { get; set; }
    }

    public enum RelationType
    {
        Equal = 0,
        NotEqual = 1,
        Larger = 2,
        LargerOrEqual = 3,
        Smaller = 4,
        SmallerOrEqual = 5
    }
}
