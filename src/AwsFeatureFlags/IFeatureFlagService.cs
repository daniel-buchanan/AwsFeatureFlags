using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AwsFeatureFlags;

public interface IFeatureFlagService
{
    /// <summary>
    /// Get a feature flag if known, by it's key.
    /// </summary>
    /// <param name="key">The key of the Feature Flag to get.</param>
    /// <returns>A Feature flag if found, otherwise null.</returns>
    FeatureFlag Get(string key);
    
    /// <summary>
    /// Get a feature flag if known, by it's key.
    /// </summary>
    /// <param name="key">The key of the Feature Flag to get.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>A Feature flag if found, otherwise null.</returns>
    Task<FeatureFlag> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets whether or not a given feature flag is enabled. If the flag is not found it returns true.
    /// </summary>
    /// <param name="key">The key of the feature flag to find.</param>
    /// <returns>True if the feature is enabled or not present, false if it is not enabled.</returns>
    bool IsEnabled(string key);
    
    /// <summary>
    /// Gets whether or not a given feature flag is enabled. If the flag is not found it returns true.
    /// </summary>
    /// <param name="key">The key of the feature flag to find.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>True if the feature is enabled or not present, false if it is not enabled.</returns>
    Task<bool> IsEnabledAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all known feature flags.
    /// </summary>
    /// <returns>An enumeration of all known feature flags.</returns>
    IEnumerable<FeatureFlag> All();
    
    /// <summary>
    /// Get all known feature flags.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>An enumeration of all known feature flags.</returns>
    Task<IEnumerable<FeatureFlag>> AllAsync(CancellationToken cancellationToken = default);
}