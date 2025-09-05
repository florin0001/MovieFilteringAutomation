using Framework.Common;

namespace Framework.Common
{
    /// <summary>
    /// Date de test predefinite folosite in cadrul testelor
    /// Centralizeaza valorile comune pentru a evita duplicarea codului
    /// </summary>
    public static class TestData
    {
        // Liste cu genurile folosite in testare
        public static readonly string[] ActionAdventureGenres = { TestConstants.ActionGenreName, TestConstants.AdventureGenreName };
        public static readonly int[] ActionAdventureGenreIds = { TestConstants.ActionGenreId, TestConstants.AdventureGenreId };

        // Intervalul de date pentru filtrarea filmelor (1990-2005)
        public static readonly DateOnly FilterFromDate = new(1990, 1, 1);    // 1 ianuarie 1990
        public static readonly DateOnly FilterToDate = new(2005, 12, 31);    // 31 decembrie 2005

        // Formatele datelor pentru interfata (d/M/yyyy format folosit in formulare)
        public static readonly string FilterFromDateUi = "1/1/1990";         // Format pentru input-ul UI
        public static readonly string FilterToDateUi = "31/12/2005";         // Format pentru input-ul UI
    }
}