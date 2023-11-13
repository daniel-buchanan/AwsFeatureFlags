# AwsFeatureFlags

Wrapper around AWS AppConfig for Simple Feature Flags.
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