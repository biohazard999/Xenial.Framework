#!/usr/bin/env bash
set -euo pipefail
dotnet run --project "./build/Xenial.Build/Xenial.Build.csproj" -- "$@"