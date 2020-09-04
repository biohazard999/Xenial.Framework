@pushd %~dp0
@dotnet run --project ".\build\Xenial.Build\Xenial.Build.csproj" -- %*
@popd