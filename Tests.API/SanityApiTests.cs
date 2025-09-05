using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tests.API
{
    public class SanityApiTests
    {
        [Test]
        public async Task Genres_endpoint_returns_list()
        {
            var apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY");
            Assert.That(apiKey, Is.Not.Null.And.Not.Empty, "TMDB_API_KEY missing.");

            using var http = new HttpClient { BaseAddress = new Uri("https://api.themoviedb.org/3/") };

            
            var resp = await http.GetAsync($"genre/movie/list?api_key={apiKey}&language=en-US");
            var body = await resp.Content.ReadAsStringAsync();

            Console.WriteLine($"STATUS={(int)resp.StatusCode} {resp.StatusCode}");
            Console.WriteLine(body);

            Assert.That(resp.IsSuccessStatusCode, Is.True, "HTTP not 200");
        }
    }
}
