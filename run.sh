#!/usr/bin/env bash

set -eu
set -o pipefail

export DOTNET_NOLOGO=true
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
export DOTNET_CLI_TELEMETRY_OPTOUT=true

dotnet fsi run.fsx -- "$@"
