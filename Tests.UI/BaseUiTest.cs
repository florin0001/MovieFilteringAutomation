using Framework.Config;
using Framework.Web;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Tests.UI
{
    /// <summary>
    /// Clasa de baza pentru toate testele UI
    /// Gestioneaza initializarea si curatarea driver-ului pentru fiecare test
    /// </summary>
    public abstract class BaseUiTest
    {
        protected IWebDriver? Driver;

        [SetUp]
        public void SetUp()
        {
            // Initializarea driver-ului Chrome pentru testare
            Driver = DriverFactory.CreateChrome(headless: false);

            // Configurarea timeout-ului implicit pentru cautarea elementelor
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            // Navigarea la pagina de discover TMDB
            Driver.Navigate().GoToUrl(TestConfig.UiBaseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            // Curatarea resurselor dupa fiecare test
            if (Driver is not null)
            {
                try { Driver.Quit(); } catch { /* ignoram erorile la inchidere */ }
                try { Driver.Dispose(); } catch { /* ignoram erorile la dispose */ }
                Driver = null;
            }
        }
    }
}