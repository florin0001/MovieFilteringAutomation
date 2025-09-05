namespace Framework.Api.Models
{
    public class MovieResult
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Release_Date { get; set; }
        public double? Vote_Average { get; set; }
        public int[] Genre_Ids { get; set; } = Array.Empty<int>();
    }
}