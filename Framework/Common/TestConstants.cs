namespace Framework.Common
{
    /// <summary>
    /// Constante globale folosite in framework pentru testare
    /// Centralizeaza valorile care se repeta in multiple teste
    /// </summary>
    public static class TestConstants
    {
        // ID-uri pentru genurile de filme din TMDB
        public const int ActionGenreId = 28;
        public const int AdventureGenreId = 16;

        // Formate pentru afisarea si parsarea datelor
        public const string UiDateFormat = "dd.MM.yyyy";        // Format pentru interfata (ex: 25.12.2023)
        public const string ApiDateFormat = "yyyy-MM-dd";       // Format pentru API (ex: 2023-12-25)

        // Numele genurilor in engleza (pentru cautare in lista API)
        public const string ActionGenreName = "Action";
        public const string AdventureGenreName = "Adventure";

        // Optiuni de sortare pentru API-ul TMDB
        public const string SortByReleaseDateAsc = "primary_release_date.asc";    // Sortare dupa data lansarii (crescator)
        public const string SortByPopularityDesc = "popularity.desc";            // Sortare dupa popularitate (descrescator)
    }
}