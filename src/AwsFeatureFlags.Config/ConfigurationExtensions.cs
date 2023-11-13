using Microsoft.Extensions.Configuration;

namespace AwsFeatureFlags;

internal static class ConfigurationExtensions
{
    /// <summary>
    /// Get a configuration section.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="section">The name of the configuration section.</param>
    /// <returns>The configuration instance.</returns>
    internal static T GetConfigSection<T>(this IConfiguration configuration, string section)
        where T : new()
    {
        var config = new T();
        configuration.GetSection(section).Bind(config, options => options.BindNonPublicProperties = true);
        return config;
    }
}