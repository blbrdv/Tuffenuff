@echo off

set "DOTNET_NOLOGO=true"
set "DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true"
set "DOTNET_CLI_TELEMETRY_OPTOUT=true"

dotnet fsi run.fsx -- %*
