namespace AwsFeatureFlags;

/// <summary>
/// Defines a flag for a given feature.
/// </summary>
public sealed class FeatureFlag
{
    /// <summary>
    /// The name of the Feature Flag.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Get whether or not the feature is enabled.
    /// </summary>
    public bool Enabled { get; set; }
    
    /// <summary>
    /// The unique key for this feature flag.
    /// </summary>
    public string Key { get; set; }
}