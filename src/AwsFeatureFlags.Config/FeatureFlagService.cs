#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.AppConfigData;
using Amazon.AppConfigData.Model;
using Newtonsoft.Json.Linq;

namespace AwsFeatureFlags;

public class FeatureFlagService : IFeatureFlagService
{
    private readonly IDictionary<string, FeatureFlag> _cache;
    private readonly IAmazonAppConfigData _amazonAppConfigData;
    private readonly FeatureFlagConfiguration _configuration;
    
    private DateTimeOffset _nextQueryTime;
    private string? _configurationToken;
    private bool _initialised;

    public FeatureFlagService(
        IAmazonAppConfigData amazonAppConfigData,
        FeatureFlagConfiguration configuration)
    {
        _amazonAppConfigData = amazonAppConfigData;
        _cache = new Dictionary<string, FeatureFlag>();
        _configuration = configuration;
        _configurationToken = null;
    }

    /// <inheritdoc />
    public FeatureFlag? Get(string key)
        => GetAsync(key).WaitFor();
    
    /// <inheritdoc />
    public async Task<FeatureFlag?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        await Initialise(cancellationToken);
        await UpdateConfiguration(cancellationToken);
        return _cache.TryGetValue(key, out var flag) ? flag : null;
    }

    /// <inheritdoc />
    public bool IsEnabled(string key)
        => IsEnabledAsync(key, CancellationToken.None).WaitFor();

    /// <inheritdoc />
    public async Task<bool> IsEnabledAsync(string key, CancellationToken cancellationToken = default)
    {
        var found = await GetAsync(key, cancellationToken);
        return found == null || found.Enabled;
    }

    /// <inheritdoc />
    public IEnumerable<FeatureFlag> All()
        => AllAsync(CancellationToken.None).WaitFor();

    /// <inheritdoc />
    public async Task<IEnumerable<FeatureFlag>> AllAsync(CancellationToken cancellationToken = default)
    {
        await Initialise(cancellationToken);
        await UpdateConfiguration(cancellationToken);
        var results = _cache.Values.AsEnumerable();
        return results;
    }

    /// <summary>
    /// Initialise the service, and fetch the initial configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    private async Task Initialise(CancellationToken cancellationToken = default)
    {
        if (_initialised) return;
        
        var startResponse = await _amazonAppConfigData.StartConfigurationSessionAsync(new StartConfigurationSessionRequest()
        {
            EnvironmentIdentifier = _configuration.EnvironmentIdentifier,
            ConfigurationProfileIdentifier = _configuration.ConfigurationProfileIdentifier,
            RequiredMinimumPollIntervalInSeconds = _configuration.RequiredMinimumPollIntervalInSeconds,
            ApplicationIdentifier = _configuration.ApplicationIdentifier
        }, cancellationToken);

        _configurationToken = startResponse.InitialConfigurationToken;

        await UpdateConfiguration(cancellationToken);

        _initialised = true;
    }

    /// <summary>
    /// Check for refresh time and update if necessary.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    private async Task UpdateConfiguration(CancellationToken cancellationToken = default)
    {
        if (DateTimeOffset.UtcNow < _nextQueryTime) return;

        var configurationResult = await _amazonAppConfigData.GetLatestConfigurationAsync(new GetLatestConfigurationRequest()
        {
            ConfigurationToken = _configurationToken
        }, cancellationToken);

        _configurationToken = configurationResult.NextPollConfigurationToken;
        _nextQueryTime = DateTimeOffset.UtcNow.AddSeconds(configurationResult.NextPollIntervalInSeconds);

        var configurationAsString = configurationResult.Configuration.DecodeToString();
        if (string.IsNullOrWhiteSpace(configurationAsString))
            return;

        LoadConfiguration(configurationAsString);
    }

    /// <summary>
    /// Load the configuration from the provided JSON.
    /// </summary>
    /// <param name="configurationAsString">The JSON string representing the configuration.</param>
    private void LoadConfiguration(string configurationAsString)
    {
        var jsonObj = JObject.Parse(configurationAsString);
        var childNodes = jsonObj.Children();

        _cache.Clear();
        foreach (var c in childNodes)
        {
            if (c is not JProperty p) continue;

            var key = p.Name;
            var value = p.Value as JObject;
            var enabled = value != null && value.Value<bool>("enabled");
            _cache.Add(key, new FeatureFlag()
            {
                Key = key,
                Enabled = enabled
            });
        }
    }
}