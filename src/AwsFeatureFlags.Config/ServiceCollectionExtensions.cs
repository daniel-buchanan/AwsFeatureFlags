using System;
using Amazon.AppConfigData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AwsFeatureFlags;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddConfiguration<T>(this IServiceCollection services, string section)
        where T: class, new() 
        => services.AddScoped<T>(p => p.GetRequiredService<IConfiguration>().GetConfigSection<T>(section));

    /// <summary>
    /// Add AwsFeatureFlags to the dependency provider, using the named configuration section.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add AwsFeatureFlags to.</param>
    /// <param name="configurationSection">The configuration section to use, defaults to "FeatureFlags"</param>
    /// <returns>The same <see cref="IServiceCollection"/> used.</returns>
    public static IServiceCollection AddFeatureFlags(
        this IServiceCollection services,
        string configurationSection = "FeatureFlags") 
        => services.AddAWSService<IAmazonAppConfigData>()
            .AddConfiguration<FeatureFlagConfiguration>(configurationSection)
            .AddSingleton<IFeatureFlagService, FeatureFlagService>();

    /// <summary>
    /// Add AwsFeatureFlags to the dependency provider, using a builder for the options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add AwsFeatureFlags to.</param>
    /// <param name="builder">A builder function to set the options.</param>
    /// <returns>The same <see cref="IServiceCollection"/> used.</returns>
    public static IServiceCollection AddFeatureFlags(
        this IServiceCollection services,
        Action<FeatureFlagConfiguration> builder)
    {
        var options = new FeatureFlagConfiguration();
        builder?.Invoke(options);
        return services.AddSingleton(options)
            .AddAWSService<IAmazonAppConfigData>()
            .AddSingleton<IFeatureFlagService, FeatureFlagService>();
    }
    
    /// <summary>
    /// Add AwsFeatureFlags to the dependency provider, using a builder for the options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add AwsFeatureFlags to.</param>
    /// <param name="builder">A builder function to set the options.</param>
    /// <returns>The same <see cref="IServiceCollection"/> used.</returns>
    public static IServiceCollection AddFeatureFlags(
        this IServiceCollection services,
        Func<IServiceProvider, FeatureFlagConfiguration> builder) 
        => services.AddAWSService<IAmazonAppConfigData>()
            .AddScoped(builder)
            .AddSingleton<IFeatureFlagService, FeatureFlagService>();
}