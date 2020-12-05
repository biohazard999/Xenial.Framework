@pushd %~dp0
@dotnet run --project ".\build\build.csproj" -- %*
@popd