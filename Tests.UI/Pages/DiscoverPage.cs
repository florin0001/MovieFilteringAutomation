using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Tests.UI.Pages
{
    public record UiMovie(int Id, string Title, DateOnly? ReleaseDate);

    /// <summary>
    /// Page Object pentru pagina Discover (TMDB).
    /// Acopera: Sortare, selectare Genuri, setare interval Data, apasare Search,
    /// citirea cardurilor din #page_1 (Titlu + Data), plus utilitari de asteptare / cookies.
    /// </summary>
    public class DiscoverPage
    {
        private readonly IWebDriver _driver;
        private WebDriverWait Wait => new(_driver, TimeSpan.FromSeconds(10));

        /// <summary>Formatul pe care il trimit in input-urile de data.</summary>
        public const string UiDateFormat = "dd.MM.yyyy";

        public DiscoverPage(IWebDriver driver) => _driver = driver;

        // ------------------- LOCATORI GLOBALI -------------------

        // Panou/trigger Sortare (in functie de limba)
        private By SortPanel =>
            By.XPath("//div[@class='name' and (normalize-space()='Sort' or normalize-space()='Sortare')]");
        private By SortTrigger =>
            By.XPath("//span[contains(@class,'k-picker')]//button[@aria-label='select']");
        private By SortList => By.Id("sort_by_listbox");
        // Release Date Ascending - in Kendo are de obicei data-offset-index=5
        private By SortOptionReleaseDateAsc =>
            By.XPath("//ul[@id='sort_by_listbox']//li[@data-offset-index='5']");

        // Genuri: lista cu li data-value='XX' in ul id="with_genres"
        private By GenreLink(int id) => By.CssSelector($"#with_genres li[data-value='{id}'] a");

        // Interval date
        private By DateFromInput => By.Id("release_date_gte");
        private By DateToInput => By.Id("release_date_lte");

        // Search / Cauta (aplica filtrele)
        private By SearchButtonByText =>
            By.XPath("//a[contains(@class,'no_click') and (normalize-space()='Search' or normalize-space()='Cauta')]");

        // Rezultate (pagina 1, carduri)
        private By ResultsContainer => By.Id("page_1");
        private By CardsUnderContainer =>
            By.XPath(".//div[contains(@class,'card') and contains(@class,'style_1')]");

        // In interiorul cardului - structura simpla
        private By CardTitlePrimary => By.XPath(".//h2/a");
        private By CardDatePrimary => By.XPath(".//div[contains(@class,'content')]/p");

        // Cookie banner (OneTrust)
        private By OneTrustBanner => By.Id("onetrust-banner-sdk");
        private By OneTrustAccept => By.Id("onetrust-accept-btn-handler");

        // ------------------- PASI (TASK 1) -------------------

        /// <summary>Astepta ca lista de carduri din #page_1 sa fie incarcata.</summary>
        public void WaitForResultsLoaded()
        {
            var container = Wait.Until(ExpectedConditions.ElementExists(ResultsContainer));
            Wait.Until(_ => container.FindElements(CardsUnderContainer).Count > 0);
        }

        /// <summary>Alege Release Date Ascending.</summary>
        public void SortByReleaseDateAscending()
        {
            AcceptCookiesIfPresent();

            SafeClick(SortPanel);
            SafeClick(SortTrigger);
            Wait.Until(ExpectedConditions.ElementIsVisible(SortList));
            SafeClick(SortOptionReleaseDateAsc);
        }

        /// <summary>Selecteaza unul sau mai multe genuri (TMDB ids, ex: 28=Action, 12=Adventure).</summary>
        public void SelectGenres(params int[] ids)
        {
            AcceptCookiesIfPresent();
            foreach (var id in ids)
            {
                SafeClick(GenreLink(id));
            }
        }

        /// <summary>Seteaza intervalul de data in format d/M/yyyy (ex: 1/1/1990 - 31/12/2005).</summary>
        public void SetDateRange(string fromDate, string toDate)
        {
            AcceptCookiesIfPresent();

            var fromEl = Wait.Until(ExpectedConditions.ElementIsVisible(DateFromInput));
            fromEl.Clear();
            Thread.Sleep(500);
            fromEl.SendKeys(fromDate);
            fromEl.SendKeys(Keys.Tab);

            Thread.Sleep(1000); // pauza scurta inainte de urmatorul camp

            var toEl = Wait.Until(ExpectedConditions.ElementIsVisible(DateToInput));
            toEl.Clear();
            Thread.Sleep(500);
            toEl.SendKeys(toDate);
            fromEl.SendKeys(Keys.Tab); 
            Thread.Sleep(1000);
        }

        /// <summary>Apasa Search/Cauta si asteapta re-incarcarea listei.</summary>
        public void ClickSearch()
        {
            AcceptCookiesIfPresent();

            // container vechi pentru asteptare
            var oldContainer = _driver.FindElements(ResultsContainer).FirstOrDefault();

            SafeClick(SearchButtonByText);

            // asteapta disparitia vechiului container
            if (oldContainer != null)
            {
                try
                {
                    Wait.Until(ExpectedConditions.StalenessOf(oldContainer));
                }
                catch
                {
                    // ignor
                }
            }

            // container nou + cel putin un card
            var container = Wait.Until(ExpectedConditions.ElementExists(ResultsContainer));
            Wait.Until(_ => container.FindElements(CardsUnderContainer).Count > 0);
        }

        // ------------------- CITIREA CARDURILOR -------------------

        /// <summary>
        /// Citeste lista de filme din #page_1: Id, Title, ReleaseDate.
        /// </summary>
        public List<UiMovie> ReadResults()
        {
            var container = Wait.Until(ExpectedConditions.ElementExists(ResultsContainer));
            Wait.Until(_ => container.FindElements(CardsUnderContainer).Count > 0);

            var items = new List<UiMovie>();
            var cards = container.FindElements(CardsUnderContainer);

            foreach (var card in cards)
            {
                try
                {
                    // --- Titlu + href ---
                    var titleLink = card.FindElement(CardTitlePrimary);
                    string title = titleLink.Text.Trim();
                    string href = titleLink.GetAttribute("href") ?? "";

                    // --- Id din href /movie/{id} - varianta simpla ---
                    int id = 0;
                    if (href.Contains("/movie/"))
                    {
                        var parts = href.Split('/');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (parts[i] == "movie" && i + 1 < parts.Length)
                            {
                                var idPart = parts[i + 1].Split('-')[0]; // ia prima parte inainte de '-'
                                int.TryParse(idPart, out id);
                                break;
                            }
                        }
                    }

                    // --- Data ---
                    DateOnly? date = null;
                    try
                    {
                        var dateEl = card.FindElement(CardDatePrimary);
                        var dateText = dateEl.Text.Trim();
                        date = ParseDate(dateText);
                    }
                    catch
                    {
                        // nu gaseste data, nu e problema
                    }

                    items.Add(new UiMovie(id, title, date));
                }
                catch
                {
                    // skip daca nu poate citi cardul
                    continue;
                }
            }

            return items;
        }

        // ------------------- HELPERI -------------------

        private void SafeClick(By locator)
        {
            var el = Wait.Until(ExpectedConditions.ElementToBeClickable(locator));
            try
            {
                el.Click();
            }
            catch (ElementClickInterceptedException)
            {
                // foloseste JavaScript daca click-ul normal nu merge
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", el);
            }
        }

        private void AcceptCookiesIfPresent()
        {
            try
            {
                var banner = _driver.FindElement(OneTrustBanner);
                if (banner.Displayed)
                {
                    var acceptBtn = banner.FindElement(OneTrustAccept);
                    acceptBtn.Click();
                    Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(OneTrustBanner));
                }
            }
            catch
            {
                // nu exista banner sau eroare, nu e problema
            }
        }

        /// <summary>Parser simplu pentru date - incearca cateva formate comune.</summary>
        private static DateOnly? ParseDate(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText))
                return null;

            // incearca dd.MM.yyyy
            if (DateTime.TryParseExact(dateText, UiDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result1))
                return DateOnly.FromDateTime(result1);

            // incearca format roman gen "01 ian 1990"
            if (DateTime.TryParse(dateText, new CultureInfo("ro-RO"), DateTimeStyles.None, out var result2))
                return DateOnly.FromDateTime(result2);

            // incearca format ISO
            if (DateTime.TryParseExact(dateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result3))
                return DateOnly.FromDateTime(result3);

            return null; // nu a reusit sa parseze
        }
    }
}