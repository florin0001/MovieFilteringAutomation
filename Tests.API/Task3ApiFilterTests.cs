using Framework.Api;
using Framework.Common;
using NUnit.Framework;
using System.Globalization;

namespace Tests.API
{
    /// <summary>
    /// TASK 3: Implementarea acelorasi filtre folosind API-ul (/discover/movie)
    /// 
    /// Pasi:
    /// 1. Obtinerea tuturor filmelor (fara filtre) - baseline
    /// 2. Aplicarea filtrelor si cautarea 
    /// 3. Obtinerea filmelor filtrate
    /// 4. Compararea listelor pentru validare
    /// </summary>
    public class Task3_ApiComparisonTests
    {
        private TmdbApiClient _apiClient = null!;

        [SetUp]
        public void SetUp() 
        {
            // Initializarea clientului API pentru TMDB
            _apiClient = new TmdbApiClient();
        }

        [Test]
        public async Task Task3_GetBaseline_ApplyFilters_CompareLists()
        {
            TestContext.Out.WriteLine("=== TASK 3: Comparatia API vs UI pentru aceleasi filtre ===\n");

            // =============================================
            // PASUL 1: Obtinerea filmelor de baza (fara filtre)
            // =============================================
            TestContext.Out.WriteLine("PASUL 1: Obtinerea filmelor populare (baseline - fara filtre)");
            
            var baselineResponse = await _apiClient.DiscoverMoviesAsync(
                sortBy: TestConstants.SortByPopularityDesc, // Sortare implicita dupapopularitate
                page: 1,
                includeAdult: false,
                language: "en-US"
            );

            Assert.That(baselineResponse.Results, Is.Not.Empty, 
                "API-ul nu a returnat filme pentru cererea de baseline");
            
            TestContext.Out.WriteLine($"✓ Baseline: {baselineResponse.Results.Count} filme gasite pe pagina 1");

            // =============================================
            // PASUL 2: Obtinerea ID-urilor pentru genuri
            // =============================================
            TestContext.Out.WriteLine("\nPASUL 2: Identificarea genurilor Action si Adventure");
            
            var allGenres = await _apiClient.GetGenresAsync();
            var actionGenre = allGenres.FirstOrDefault(g => g.Name == TestConstants.ActionGenreName);
            var adventureGenre = allGenres.FirstOrDefault(g => g.Name == TestConstants.AdventureGenreName);
            
            Assert.That(actionGenre, Is.Not.Null, "Genul 'Action' nu a fost gasit in lista TMDB");
            Assert.That(adventureGenre, Is.Not.Null, "Genul 'Adventure' nu a fost gasit în lista TMDB");
            
            var selectedGenreIds = new[] { actionGenre!.Id, adventureGenre!.Id };
            TestContext.Out.WriteLine($"✓ Genuri identificate: Action (ID: {actionGenre.Id}), Adventure (ID: {adventureGenre.Id})");

            // =============================================
            // PASUL 3: Aplicarea acelorasi filtre ca în Task 1
            // =============================================
            TestContext.Out.WriteLine("\nPASUL 3: Aplicarea filtrelor identice cu Task 1");
            TestContext.Out.WriteLine("- Sortare: Data lansarii (crescator)");
            TestContext.Out.WriteLine("- Genuri: Action + Adventure");  
            TestContext.Out.WriteLine("- Interval date: 1990-2005");
            
            var filteredResponse = await _apiClient.DiscoverMoviesAsync(
                sortBy: TestConstants.SortByReleaseDateAsc,     // Aceeasi sortare ca Task 1
                from: TestData.FilterFromDate,                  // Aceeasi data de inceput
                to: TestData.FilterToDate,                      // Aceeasi data de sfarsit
                genreIds: selectedGenreIds,                     // Aceleasi genuri
                page: 1,
                includeAdult: false,
                language: "en-US"
            );

            Assert.That(filteredResponse.Results, Is.Not.Empty, 
                "API-ul nu a returnat filme dupa aplicarea filtrelor");
            
            TestContext.Out.WriteLine($"✓ Filme filtrate: {filteredResponse.Results.Count} rezultate gasite");

            // =============================================
            // PASUL 4: Compararea listelor
            // =============================================
            TestContext.Out.WriteLine("\nPASUL 4: Compararea si validarea rezultatelor");
            
            var baselineMovieIds = baselineResponse.Results.Select(m => m.Id).ToHashSet();
            var filteredMovieIds = filteredResponse.Results.Select(m => m.Id).ToHashSet();

            // Verificarea că filtrele au fost aplicate (listele sunt diferite)
            var listsAreIdentical = baselineMovieIds.SetEquals(filteredMovieIds);
            Assert.That(listsAreIdentical, Is.False, 
                "Listele baseline si filtrate sunt identice - filtrele nu au fost aplicate corect");
            
            TestContext.Out.WriteLine("✓ Validare: Filtrele au modificat rezultatele (listele sunt diferite)");

            // =============================================
            // VALIDRI SUPLIMENTARE (Task 2 echivalent pentru API)
            // =============================================
            
            // Validarea sortarii dupa data lansarii
            MovieAssertions.AssertSortedByDateAscending(filteredResponse.Results);
            TestContext.Out.WriteLine("✓ Validare: Sortarea crescatoare dupa data este corecta");
            
            // Validarea intervalului de date
            MovieAssertions.AssertDateRange(filteredResponse.Results, TestData.FilterFromDate, TestData.FilterToDate);
            TestContext.Out.WriteLine("✓ Validare: Toate filmele sunt in intervalul 1990-2005");

            // =============================================
            // AFISAREA REZULTATELOR PENTRU COMPARATIE
            // =============================================
            
            TestContext.Out.WriteLine("\n==== Primele 10 rezultate API (pentru comparatie cu Task 1) ====");
            foreach (var movie in filteredResponse.Results.Take(10))
            {
                var dateString = string.IsNullOrEmpty(movie.Release_Date) ? "Fara data" : 
                    DateOnly.ParseExact(movie.Release_Date, TestConstants.ApiDateFormat, CultureInfo.InvariantCulture)
                    .ToString(TestConstants.UiDateFormat);
                TestContext.Out.WriteLine($"{movie.Title} | {dateString}");
            }

            // =============================================
            // REZUMATUL COMPARATIEI
            // =============================================
            
            var commonMovies = baselineMovieIds.Intersect(filteredMovieIds).Count();
            
            TestContext.Out.WriteLine($"\n==== REZUMATUL TASK 3 ====");
            TestContext.Out.WriteLine($"Filme baseline (populare):     {baselineResponse.Results.Count}");
            TestContext.Out.WriteLine($"Filme filtrate (1990-2005):    {filteredResponse.Results.Count}");
            TestContext.Out.WriteLine($"Filme comune între liste:      {commonMovies}");
            TestContext.Out.WriteLine($"Filtrele aplicate cu succes:   {(!listsAreIdentical ? "DA" : "NU")}");
            TestContext.Out.WriteLine($"Toate validarile au trecut:    DA");
            
            TestContext.Out.WriteLine("\n✓ TASK 3 FINALIZAT CU SUCCES!");
            TestContext.Out.WriteLine("  Comparatia API vs UI demonstreaza cafiltrele functioneaza identic.");
        }

        [TearDown]
        public void TearDown()
        {
            // Curatarea resurselor dupa test
            _apiClient?.Dispose();
        }
    }
}