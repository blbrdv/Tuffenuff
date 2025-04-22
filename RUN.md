# Automation script

Run `.\run.cmd` (Windows OS) or `./run.sh` (Unix-based OS):

```
Automation script.

Usage:
    script <TARGET> [-s|-n|-v] [--rebuild] [--filter <FILTER>] [-e <ENV> ...]
    script (-h | --help)
    script --list
    script --version

Options:
    -h --help     Print this.
    --list        Print list of available targets.
    --version     Print module version.
    -s            Silent trace level.
    -n            Normal trace level.
    -v            Verbose trace level.
    --rebuild     Force building CICD project.
    --filter      Sets filter expression for tests run.
    -e            Sets environment variables.
```

### Available targets

- `AllTests` - Run "UnitTests" then "IntegrationTests"
- `Build` - Build Tuffenuff
- `CheckFormat` - Check if code files need formatting
- `Clean` - Delete "bin" and "obj" directories
- `Format` - Format code files
- `GenerateAllSyntaxVersions` - Run "GenerateSyntaxVersions" then "GenerateUpstreamSyntaxVersions"
- `GenerateSyntaxVersions` - Generates module with syntax versions from "dockerfile" repository
- `GenerateUpstreamSyntaxVersions` - Generates module with syntax versions from "dockerfile-upstream" repository
- `IntegrationTests` - Tests scripts in examples directory
- `Release` - Push library to Nuget
- `UnitTests` - Tests DSL

### Trace level

- `Silent` - equivalent of `-s` flag for FAKE and `-v q` flag for dotnet commands. By default.
- `Normel` - equivalent of no flag for FAKE and `-v n` flag for dotnet command.
- `Verbose` - equivalent of `-v` flag for FAKE and `-v d` flag for dotnet command.

See [Command line guide](https://fake.build/guide/commandline.html) for FAKE and 
[LoggerVerbosity documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.build.framework.loggerverbosity?view=msbuild-17-netcore) 
for dotnet.

### Filter tests

See [Run selected unit tests](https://learn.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests?pivots=mstest)
documentation page.
