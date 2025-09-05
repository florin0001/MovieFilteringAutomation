using NUnit.Framework;
using Tests.UI.Pages;
using Framework.Common;

namespace Tests.UI
{
    /// <summary>
    /// TASK 1 & TASK 2: Testarea filtrelor UI si validarea rezultatelor
    /// Acest test implementeaza toate cerintele pentru Task 1 si Task 2
    /// </summary>
    public class FiltersUiTests : BaseUiTest
    {
        [Test]
        public void Baseline_then_Filter_and_Compare_Sort_and_Range()
        {
            var page = new DiscoverPage(Driver!);

            // 0) BASELINE (fara filtre) - obtinerea rezultatelor initiale
            page.WaitForResultsLoaded();
            var baseline = page.ReadResults();
            Assert.That(baseline, Is.Not.Empty, "Baseline (page_1) is empty.");

            // ===========================================
            // TASK 1: IMPLEMENTAREA FILTRELOR
            // ===========================================

            // TASK 1.A: Filtrare dupa data lansarii (crescator)
            page.SortByReleaseDateAscending();

            // TASK 1.B: Selectarea genurilor (Action + Adventure)  
            page.SelectGenres(TestData.ActionAdventureGenreIds);

            // TASK 1.C: Cautare dupa intervalul de date 1990-2005
            page.SetDateRange(TestData.FilterFromDateUi, TestData.FilterToDateUi);

            // Aplicarea tuturor filtrelor prin cautare
            page.ClickSearch();

            // ===========================================
            // TASK 2: VERIFICAREA FILTRARII
            // ===========================================

            // Citirea rezultatelor dupa aplicarea filtrelor
            var after = page.ReadResults();
            Assert.That(after, Is.Not.Empty, "No results after applying filters + search.");

            // TASK 2 - VALIDARE a) Lista s-a schimbat fata de baseline
            var baseIds = baseline.Where(x => x.Id > 0).Select(x => x.Id).ToList();
            var afterIds = after.Where(x => x.Id > 0).Select(x => x.Id).ToList();
            Assert.That(baseIds.SequenceEqual(afterIds), Is.False,
                "Results after filtering are identical to baseline – Search likely didn't apply filters.");

            // TASK 2 - VALIDARE b) Ordinea: data lansarii ASC (ignora itemii fara data)
            var dated = after.Where(x => x.ReleaseDate is not null)
                             .Select(x => x.ReleaseDate!.Value)
                             .ToList();

            if (dated.Count > 1)
            {
                var sorted = dated.OrderBy(d => d).ToList();
                Assert.That(sorted.SequenceEqual(dated), Is.True,
                    "Results are not sorted ascending by release date.");
            }

            // TASK 2 - VALIDARE c) In intervalul 1990–2005 (inclusiv)
            foreach (var d in dated)
                Assert.That(d.Year, Is.InRange(1990, 2005),
                    $"Release date {d:dd.MM.yyyy} is outside 1990–2005.");

            // TASK 2 - VALIDARE d) Afisarea primelor 10 rezultate pentru verificare
            TestContext.Out.WriteLine("---- First 10 results after filter ----");
            foreach (var r in after.Take(10))
                TestContext.Out.WriteLine($"{r.Title} | {(r.ReleaseDate is null ? "-" : r.ReleaseDate.Value.ToString(DiscoverPage.UiDateFormat))}");
        }
    }
}