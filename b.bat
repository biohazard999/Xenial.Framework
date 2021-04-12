@echo off
@pushd %~dp0
if defined DEVEXPRESS_NUGET_FEED (
    echo "%%DEVEXPRESS_NUGET_FEED%% is set"
    if "%DEVEXPRESS_NUGET_FEED%"=="artifacts\nuget" (
        echo "%%DEVEXPRESS_NUGET_FEED%% is local source you only will be able to build docs"    
    )
) ELSE (
    echo "%%DEVEXPRESS_NUGET_FEED%% is not set, setting it to ./artifacts/nuget"
    setx /M DEVEXPRESS_NUGET_FEED "artifacts\nuget"
    echo "PLEASE RESTART YOUR CONSOLE"
    EXIT
)

@dotnet run --project ".\build\build.csproj" -- %*
@popd