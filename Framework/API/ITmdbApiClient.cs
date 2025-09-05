using Framework.Api.Models;
using System.Text.Json;

namespace Framework.Api
{
    public interface ITmdbApiClient
    {
        Task<JsonDocument> GetAsync(string pathWithQuery);
        Task<List<Genre>> GetGenresAsync(string language = "en-US");
        Task<DiscoverResponse> DiscoverMoviesAsync(
            string sortBy = "primary_release_date.asc",
            DateOnly? from = null,
            DateOnly? to = null,
            IEnumerable<int>? genreIds = null,
            double? minVoteAverage = null,
            int page = 1,
            bool includeAdult = false,
            string language = "en-US");
    }
}