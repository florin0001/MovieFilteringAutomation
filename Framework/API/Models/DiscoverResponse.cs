namespace Framework.Api.Models
{
    public class DiscoverResponse
    {
        public int Page { get; set; }
        public int Total_Pages { get; set; }
        public int Total_Results { get; set; }
        public List<MovieResult> Results { get; set; } = new();
    }
}