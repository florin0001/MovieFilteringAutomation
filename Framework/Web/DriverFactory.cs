using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Framework.Web
{
    /// <summary>
    /// Factory pentru crearea driver-ilor Selenium WebDriver
    /// Configureaza optiunile specifice pentru diferite browsere
    /// </summary>
    public static class DriverFactory
    {
        /// <summary>
        /// Creaza un nou driver Chrome cu optiunile optimizate pentru testare
        /// </summary>
        /// <param name="headless">Ruleaza browser-ul in mod headless (fara interfata grafica)</param>
        /// <returns>Driver Chrome configurat pentru testare</returns>
        public static IWebDriver CreateChrome(bool headless = false)
        {
            var chromeOptions = new ChromeOptions();

            // Optiuni pentru modul headless (util pentru CI/CD)
            if (headless)
                chromeOptions.AddArgument("--headless=new");

            // Optiuni pentru stabilitate si performanta
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--window-size=1920,1080");

            // Dezactivarea notificarilor si pop-up-urilor
            chromeOptions.AddArgument("--disable-notifications");
            chromeOptions.AddArgument("--disable-popup-blocking");

            // Optimizari pentru performanta in testare
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--disable-images");

            // Selenium Manager descarca automat versiunea corecta de ChromeDriver
            return new ChromeDriver(chromeOptions);
        }
    }
}