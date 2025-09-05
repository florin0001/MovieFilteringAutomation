using Framework.Api;
using Framework.Common;
using NUnit.Framework;

namespace Tests.API
{
    public class DiscoverApiTests
    {
        private TmdbApiClient _api = null!;

        [SetUp]
        public void SetUp() => _api = new TmdbApiClient();

        [Test]
        public async Task Discover_filters_releaseDateAsc_genres_range_and_minVote()
        {
            
            var allGenres = await _api.GetGenresAsync();
            var wantedIds = allGenres
                .Where(g => TestData.ActionAdventureGenres.Contains(g.Name))
                .Select(g => g.Id)
                .ToList();
            Assert.That(wantedIds, Is.Not.Empty, "Could not find required genres in TMDB list.");

            
            var resp = await _api.DiscoverMoviesAsync(
                sortBy: TestConstants.SortByReleaseDateAsc,
                from: TestData.FilterFromDate,
                to: TestData.FilterToDate,
                genreIds: wantedIds,
                minVoteAverage: 7.0,  // bonus "user score"
                page: 1,
                includeAdult: false,
                language: "en-US"
            );

            
            Assert.That(resp.Results, Is.Not.Empty, "TMDB returned 0 movies for given filters.");

            
            MovieAssertions.AssertDateRange(resp.Results, TestData.FilterFromDate, TestData.FilterToDate);
            MovieAssertions.AssertSortedByDateAscending(resp.Results);
            MovieAssertions.AssertMinimumVoteAverage(resp.Results, 7.0);
        }

        [TearDown]
        public void TearDown()
        {
            _api?.Dispose();
        }
    }
}