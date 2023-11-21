#!/usr/bin/bash

./.sonar/scanner/dotnet-sonarscanner begin \
    /k:"daniel-buchanan_AwsFeatureFlags" \
    /o:"daniel-buchanan" \
    /d:sonar.token="${SONAR_TOKEN}" \
    /d:sonar.host.url="https://sonarcloud.io"

dotnet build --no-incremental AwsFeatureFlags.sln 

./.sonar/scanner/dotnet-sonarscanner end \
    /d:sonar.token="${SONAR_TOKEN}"