using Microsoft.Extensions.Configuration;

namespace Framework.Config;

/// <summary>
/// Clasa pentru managementul configuratiei testelor
/// Incarca setarile din appsettings.json, variabile de mediu si user secrets
/// </summary>
public static class TestConfig
{
    private static readonly Lazy<IConfigurationRoot> _configurationLazy = new(() =>
    {
        var basePath = AppContext.BaseDirectory;

        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

        try
        {
            // Incarcarea user secrets pentru chei API sensibile
            configBuilder.AddUserSecrets(typeof(TestConfig).Assembly, optional: true);
        }
        catch { /* ignoram daca nu exista user secrets */ }

        return configBuilder.Build();
    });

    public static IConfigurationRoot Root => _configurationLazy.Value;

    /// <summary>URL-ul paginii de discover TMDB pentru testele UI</summary>
    public static string UiBaseUrl => Root["UiBaseUrl"] ?? "";

    /// <summary>URL-ul de baza pentru API-ul TMDB</summary>
    public static string ApiBaseUrl => Root["ApiBaseUrl"] ?? "";

    /// <summary>Timeout-ul implicit pentru operatiile de asteptare (secunde)</summary>
    public static int DefaultTimeoutSec => int.TryParse(Root["DefaultTimeoutSec"], out var value) ? value : 15;

    /// <summary>
    /// Cheia API pentru TMDB - se incarca din user secrets sau variabile de mediu
    /// Pentru setare: dotnet user-secrets set TMDB_API_KEY "cheia_ta_aici"
    /// </summary>
    public static string TmdbApiKey =>
        Root["TMDB_API_KEY"] ??
        Environment.GetEnvironmentVariable("TMDB_API_KEY") ??
        throw new InvalidOperationException("TMDB_API_KEY nu este configurata. Seteaza-o cu: dotnet user-secrets set TMDB_API_KEY <cheia_ta>");
}