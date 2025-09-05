using Framework.Api;
using NUnit.Framework;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.API
{
    public class DiscoverApi
    {
        [Test]
        public async Task Can_get_genres_via_framework_client()
        {
            var api = new TmdbApiClient();
            using var json = await api.GetAsync("genre/movie/list?language=en-US");

            Assert.That(json.RootElement.TryGetProperty("genres", out var genres), Is.True);
            Assert.That(genres.GetArrayLength(), Is.GreaterThan(0));
        }
    }
}
