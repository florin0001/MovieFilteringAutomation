using OpenQA.Selenium;

namespace Framework.Web
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElementWithRetry(this IWebDriver driver, By by, int maxAttempts = 3)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    return driver.FindElement(by);
                }
                catch (NoSuchElementException) when (i < maxAttempts - 1)
                {
                    Thread.Sleep(1000);
                }
            }
            throw new NoSuchElementException($"Element not found after {maxAttempts} attempts: {by}");
        }

        public static void ClickWithRetry(this IWebElement element, int maxAttempts = 3)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    element.Click();
                    return;
                }
                catch (ElementClickInterceptedException) when (i < maxAttempts - 1)
                {
                    Thread.Sleep(500);
                }
            }
            throw new ElementClickInterceptedException($"Element could not be clicked after {maxAttempts} attempts");
        }
    }
}