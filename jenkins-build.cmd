@if "%_echo%"=="" echo off

:ARGUMENTS_VALIDATION

if /I "%1" == "/help"   (goto :USAGE)
if /I "%1" == "/h"      (goto :USAGE)
if /I "%1" == "/?"      (goto :USAGE)

set BUILD_PROFILE=%*

if /I "%BUILD_PROFILE%" == "debug" (
    set BUILD_ARGS=debug compiler coreclr pcls vs notests
    goto :ARGUMENTS_OK
)
if /I "%BUILD_PROFILE%" == "release" (
    set BUILD_ARGS=release compiler coreclr pcls vs notests
    goto :ARGUMENTS_OK
)

if /I "%BUILD_PROFILE%" == "ci_part1" (
    set BUILD_ARGS=release ci_part1
    goto :ARGUMENTS_OK
)
if /I "%BUILD_PROFILE%" == "ci_part2" (
    set BUILD_ARGS=release ci_part2
    goto :ARGUMENTS_OK
)
echo '%BUILD_PROFILE%' is not a valid profile
goto :USAGE

:USAGE

echo Usage:
echo Builds the source tree using a specific configuration
echo jenkins-build.cmd ^<debug^|release^>
exit /b 1

:ARGUMENTS_OK

rem Do build only for now
call build.cmd %BUILD_ARGS% coreclr

goto :eof

:failure
exit /b 1
