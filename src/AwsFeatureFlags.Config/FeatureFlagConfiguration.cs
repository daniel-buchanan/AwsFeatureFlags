namespace AwsFeatureFlags;

public class FeatureFlagConfiguration
{
    /// <summary>
    /// Get or set the "ApplicationIdentifier" setup in AWS AppConfig.
    /// </summary>
    public string ApplicationIdentifier { get; set; }
    
    /// <summary>
    /// Get or set the "EnvironmentIdentifier" setup in AWS AppConfig.
    /// </summary>
    public string EnvironmentIdentifier { get; set; }
    
    /// <summary>
    /// Get or set the "ConfigurationProfileIdentifier" setup in AWS AppConfig.
    /// </summary>
    public string ConfigurationProfileIdentifier { get; set; }
    
    /// <summary>
    /// Get or set the "RequiredMinimumPollIntervalInSections" for which to poll AWS AppConfig.
    /// </summary>
    public int RequiredMinimumPollIntervalInSeconds { get; set; } = 120;
}