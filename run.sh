#!/usr/bin/env bash

set -eu
set -o pipefail

dotnet fsi run.fsx -- "$@"
