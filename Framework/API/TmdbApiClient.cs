using Framework.Api.Models;
using Framework.Config;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Framework.Api
{
    /// <summary>
    /// Client pentru API-ul TheMovieDB (TMDB)
    /// Implementeaza functionalitati de baza pentru interactiunea cu API-ul
    /// </summary>
    public class TmdbApiClient : ITmdbApiClient
    {
        private readonly HttpClient _httpClient;

        public TmdbApiClient(HttpClient? httpClient = null)
        {
            // Configurarea clientului HTTP cu URL-ul de baza pentru API
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.BaseAddress = new Uri(TestConfig.ApiBaseUrl.TrimEnd('/') + "/");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Metoda pentru efectuarea cererilor GET catre API
        /// Adauga automat cheia API in query string
        /// </summary>
        public async Task<JsonDocument> GetAsync(string pathWithQuery)
        {
            // Determinarea separatorului pentru query string 
            var separator = pathWithQuery.Contains('?') ? "&" : "?";
            var fullUrl = $"{pathWithQuery.TrimStart('/')}" + $"{separator}api_key={TestConfig.TmdbApiKey}";

            var response = await _httpClient.GetAsync(fullUrl);
            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();
            return await JsonDocument.ParseAsync(contentStream);
        }

        /// <summary>
        /// Obtine lista genurilor de filme disponibile
        /// </summary>
        public async Task<List<Genre>> GetGenresAsync(string language = "en-US")
        {
            var jsonResponse = await GetAsync($"genre/movie/list?language={language}");
            var genresList = new List<Genre>();

            foreach (var genreElement in jsonResponse.RootElement.GetProperty("genres").EnumerateArray())
            {
                genresList.Add(new Genre(
                    genreElement.GetProperty("id").GetInt32(),
                    genreElement.GetProperty("name").GetString()!
                ));
            }
            return genresList;
        }

        /// <summary>
        /// Cautarea filmelor cu aplicarea filtrelor specificate
        /// Implementeaza functionalitatea de discover/movie cu optiuni de filtrare
        /// </summary>
        public async Task<DiscoverResponse> DiscoverMoviesAsync(
            string sortBy = "primary_release_date.asc",
            DateOnly? from = null,
            DateOnly? to = null,
            IEnumerable<int>? genreIds = null,
            double? minVoteAverage = null,
            int page = 1,
            bool includeAdult = false,
            string language = "en-US")
        {
            // Validare simpla
            if (page < 1)
                page = 1; // corectie automat in loc de exceptie

            // Construirea parametrilor pentru query string
            var queryParameters = new List<string>
            {
                $"sort_by={sortBy}",
                $"include_adult={includeAdult.ToString().ToLower()}",
                $"language={language}",
                $"page={page}"
            };

            // Adaugarea filtrelor optionale
            if (from != null) queryParameters.Add($"primary_release_date.gte={from:yyyy-MM-dd}");
            if (to != null) queryParameters.Add($"primary_release_date.lte={to:yyyy-MM-dd}");
            if (genreIds != null && genreIds.Any())
                queryParameters.Add($"with_genres={string.Join(",", genreIds)}");
            if (minVoteAverage != null)
                queryParameters.Add($"vote_average.gte={minVoteAverage}");

            // Efectuarea cererii catre API
            var jsonResponse = await GetAsync("discover/movie?" + string.Join("&", queryParameters));

            // Parsarea raspunsului si construirea obiectului de raspuns
            var discoverResponse = new DiscoverResponse
            {
                Page = jsonResponse.RootElement.GetProperty("page").GetInt32(),
                Total_Pages = jsonResponse.RootElement.GetProperty("total_pages").GetInt32(),
                Total_Results = jsonResponse.RootElement.GetProperty("total_results").GetInt32(),
                Results = new List<MovieResult>()
            };

            // Procesarea fiecarui film din rezultate
            foreach (var movieElement in jsonResponse.RootElement.GetProperty("results").EnumerateArray())
            {
                var movie = new MovieResult
                {
                    Id = movieElement.GetProperty("id").GetInt32(),
                    Title = movieElement.GetProperty("title").GetString() ?? "",
                    Release_Date = movieElement.TryGetProperty("release_date", out var releaseDateProp) ? releaseDateProp.GetString() : null,
                    Vote_Average = movieElement.TryGetProperty("vote_average", out var voteAvgProp) ? voteAvgProp.GetDouble() : null
                };

                // Extragerea ID-urilor genurilor pentru film
                if (movieElement.TryGetProperty("genre_ids", out var genreIdsProperty))
                {
                    var genreIdsList = new List<int>();
                    foreach (var genreId in genreIdsProperty.EnumerateArray())
                    {
                        genreIdsList.Add(genreId.GetInt32());
                    }
                    movie.Genre_Ids = genreIdsList.ToArray();
                }

                discoverResponse.Results.Add(movie);
            }

            return discoverResponse;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}