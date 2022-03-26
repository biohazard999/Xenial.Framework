@echo off
@pushd %~dp0
SET DEVEXPRESS_NUGET_FEED_DEFAULT=artifacts\nuget
if defined DEVEXPRESS_NUGET_FEED (
    echo %%DEVEXPRESS_NUGET_FEED%% is set
    if "%DEVEXPRESS_NUGET_FEED%"=="%DEVEXPRESS_NUGET_FEED_DEFAULT%" (
        echo %%DEVEXPRESS_NUGET_FEED%% is local source you only will be able to build docs
    )
) ELSE (
    echo %%DEVEXPRESS_NUGET_FEED%% is not set, setting it to %DEVEXPRESS_NUGET_FEED_DEFAULT%
    echo %%DEVEXPRESS_NUGET_FEED%% is local source you only will be able to build docs
    setx DEVEXPRESS_NUGET_FEED %DEVEXPRESS_NUGET_FEED_DEFAULT%
    echo Refreshing EnvironmentVariables
    call refreshenv.bat
)

@dotnet run --project ".\build\build.csproj" --no-launch-profile -- %*
@popd