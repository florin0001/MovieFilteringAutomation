using Framework.Config;
using NUnit.Framework;

namespace Tests.API
{
    [SetUpFixture]
    public class TestSetupValidation
    {
        [OneTimeSetUp]
        public void ValidateConfiguration()
        {
            
            var apiKey = TestConfig.TmdbApiKey;
            Assert.That(apiKey, Is.Not.Null.And.Not.Empty, "TMDB_API_KEY not configured");
            Assert.That(apiKey.Length, Is.GreaterThan(10), "TMDB_API_KEY appears to be invalid (too short)");

            
            var apiBaseUrl = TestConfig.ApiBaseUrl;
            Assert.That(Uri.IsWellFormedUriString(apiBaseUrl, UriKind.Absolute),
                Is.True, "ApiBaseUrl is not a valid URL");

            var uiBaseUrl = TestConfig.UiBaseUrl;
            Assert.That(Uri.IsWellFormedUriString(uiBaseUrl, UriKind.Absolute),
                Is.True, "UiBaseUrl is not a valid URL");

            
            Assert.That(TestConfig.DefaultTimeoutSec, Is.GreaterThan(0),
                "DefaultTimeoutSec must be positive");

            TestContext.Out.WriteLine("Configuration validation passed");
        }
    }
}