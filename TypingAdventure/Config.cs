using Microsoft.Extensions.Configuration;

namespace TypingAdventure;

internal static class Config
{
    private static IConfigurationRoot _configuration;

    static Config()
    {
        _configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();
    }

    public static IConfiguration Configuration => _configuration;
}
