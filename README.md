# AwsFeatureFlags

Wrapper around AWS AppConfig for Simple Feature Flags.

[![Build](https://github.com/daniel-buchanan/AwsFeatureFlags/actions/workflows/build.yml/badge.svg)](https://github.com/daniel-buchanan/AwsFeatureFlags/actions/workflows/build.yml)  
NuGet: https://www.nuget.org/packages/AwsFeatureFlags/

---
- [Configuration](#configuration)
- [Usage](#usage)
- [Methods](#methods)
---

To use simply add:
```csharp
var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

services.AddSingleton(configuration);
services.AddDefaultAWSOptions(p => p.GetService<IConfiguration>().GetAWSOptions());
services.AddFeatureFlags();

var provider = services.BuildServiceProvider();
```

## Configuration
By default AwsFeatureFlags pulls its configuration from `IConfiguration` which can be configured in many ways and come from any number of sources.

Without specifying a configuration source, AwsFeatureFlags will look for a `FeatureFlags` section in the `appsettings.json` file, or other configuration source.

AwsFeatureFlags can also be configured using one of the overrides of `AddFeatureFlags()`, as follows:
```csharp
services.AddFeatureFlags(o => myPreconfiguredOptions);
```

Or:
```csharp
services.AddFeatureFlags(o => {
    o.ApplicationIdentifier = "myApp";
    o.EnvironmentIdentifier = "dev";
    o.ConfigurationProfileIdentifier = "default";
});
```

> Note that you don't have to set `RequiredMinimumPollIntervalInSeconds` as it will default to 120 seconds (2 minutes).

Or:
```csharp
services.AddFeatureFlags(p => {
    var myService = p.GetService<MyService>();
    return myService.GetOptions();
});
```

In all of the above cases the `AddFeatureFlags()` method takes care of everything that AwsFeatureFlags needs to work.

### AWS Configuration
AwsFeatureFlags does use the `AWSSDK.Extensions.NETCore.Setup` package, and as such you will need the following somewhere in your dependency tree:
```csharp
services.AddDefaultAWSOptions(p => p.GetService<IConfiguration>().GetAWSOptions());
```

### Usage
AwsFeatureFlags provides a simple interface for checking feature flags:  
1. Simply configure as above and the `IFeatureFlagService` will be added to your DI tree
2. Inject into a class you are using:
   ```csharp
   namespace: mynamespace;
   
   public class MyClass
   {
        private readonly IFeatureFlagService _featureFlags;
   
        public MyClass(IFeatureFlagService featureFlags)
           => _featureFlags = featureFlags;
   
        public async Task Get()
        {
            var enabled = await _featureFlags.IsEnabledAsync("my_flag");
            if(!enabled) return;
   
            // do some things here
        }
   }
   ```
   
## Methods
AwsFeatureFlags provides a single service `IFeatureFlagService` which encapsulates all the functionality that this package provides.
This provides the following methods:  

| Signature                                                                   | Return Type                      | Description                                                                                                 |
|-----------------------------------------------------------------------------|----------------------------------|-------------------------------------------------------------------------------------------------------------|
| `Get(string key)`                                                           | `FeatureFlag`                    | Gets a single Feature Flag.                                                                                 |
| `GetAsync(string key, CancellationToken cancellationToken = default)`       | `Task<FeatureFlag>`              | Gets a single feature flag asynchronously.                                                                  |
| `IsEnabled(string key)`                                                     | `bool`                           | Returns whether or not a given flag is enabled. If the flag is not found, it returns `true`.                |
| `IsEnabledAsync(string key, CancellationToken cancellationToken = default)` | `Task<bool>`                     | Returns whether or not a given flag is enabled asynchronously. If the flag is not found, it returns `true`. |
| `All()`                                                                     | `IEnumerable<FeatureFlag>`       | Gets all known feature flags.                                                                               |
| `AllAsync(CancellationToken cancellationToken = default)`                   | `Task<IEnumerable<FeatureFlag>>` | Gets all know feature flags asynchronously.                                                                 |

### Notes
1. The feature flags need to be defined, and setup as per the AWS documentation ([here](https://docs.aws.amazon.com/appconfig/latest/userguide/what-is-appconfig.html)).
2. The `key` used needs to exactly correspond to what has been created in AppConfig.
3. AwsFeatureFlags **will not** throw an exception if a flag is not found, the default behaviour is to return `true`, or `null` if not found.