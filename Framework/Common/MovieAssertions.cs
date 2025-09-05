using Framework.Api.Models;
using Framework.Common;
using NUnit.Framework;
using System.Globalization;

namespace Framework.Common
{
    /// <summary>
    /// Metode helper pentru validarea datelor filmelor
    /// Centralizeaza logica de verificare pentru a fi reutilizata in multiple teste
    /// </summary>
    public static class MovieAssertions
    {
        /// <summary>
        /// Verifica ca toate filmele din lista sunt in intervalul de date specificat
        /// </summary>
        /// <param name="movies">Lista filmelor de verificat</param>
        /// <param name="from">Data de inceput a intervalului</param>
        /// <param name="to">Data de sfarsit a intervalului</param>
        public static void AssertDateRange(IEnumerable<MovieResult> movies, DateOnly from, DateOnly to)
        {
            foreach (var movie in movies.Where(m => !string.IsNullOrEmpty(m.Release_Date)))
            {
                var releaseDate = DateOnly.ParseExact(movie.Release_Date!, TestConstants.ApiDateFormat, CultureInfo.InvariantCulture);
                Assert.That(releaseDate, Is.InRange(from, to),
                    $"Filmul '{movie.Title}' cu data {releaseDate:dd.MM.yyyy} este in afara intervalului {from:dd.MM.yyyy}-{to:dd.MM.yyyy}");
            }
        }

        /// <summary>
        /// Verifica ca filmele sunt sortate crescator dupa data lansarii
        /// </summary>
        /// <param name="movies">Lista filmelor de verificat</param>
        public static void AssertSortedByDateAscending(IEnumerable<MovieResult> movies)
        {
            var movieDates = movies
                .Where(m => !string.IsNullOrEmpty(m.Release_Date))
                .Select(m => DateOnly.ParseExact(m.Release_Date!, TestConstants.ApiDateFormat, CultureInfo.InvariantCulture))
                .ToList();

            if (movieDates.Count > 1)
            {
                var sortedDates = movieDates.OrderBy(d => d).ToList();
                Assert.That(movieDates.SequenceEqual(sortedDates), Is.True,
                    "Filmele nu sunt sortate crescator dupa data lansarii");
            }
        }

        /// <summary>
        /// Verifica ca filmele contin cel putin unul din genurile specificate
        /// </summary>
        /// <param name="movies">Lista filmelor de verificat</param>
        /// <param name="requiredGenreIds">ID-urile genurilor care trebuie sa fie prezente</param>
        public static void AssertContainsGenres(IEnumerable<MovieResult> movies, int[] requiredGenreIds)
        {
            foreach (var movie in movies)
            {
                var hasRequiredGenre = movie.Genre_Ids.Any(id => requiredGenreIds.Contains(id));
                Assert.That(hasRequiredGenre, Is.True,
                    $"Filmul '{movie.Title}' nu contine niciunul din genurile cerute: {string.Join(", ", requiredGenreIds)}");
            }
        }

        /// <summary>
        /// Verifica ca filmele au scorul minim specificat
        /// </summary>
        /// <param name="movies">Lista filmelor de verificat</param>
        /// <param name="minVoteAverage">Scorul minim acceptat</param>
        public static void AssertMinimumVoteAverage(IEnumerable<MovieResult> movies, double minVoteAverage)
        {
            foreach (var movie in movies.Where(m => m.Vote_Average.HasValue))
            {
                Assert.That(movie.Vote_Average!.Value, Is.GreaterThanOrEqualTo(minVoteAverage),
                    $"Filmul '{movie.Title}' are scorul {movie.Vote_Average} sub minimul de {minVoteAverage}");
            }
        }
    }
}