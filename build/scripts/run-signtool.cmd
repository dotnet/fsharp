@echo off
:: Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
setlocal enableDelayedExpansion
set scriptdir=%~dp0
set MSBuild=
set SignType=
set ConfigFile=

::
:: Validate arguments
::

:parsearg
if "%1" == "" goto doneargs
set arg=%1
set argv=%2

if /i "%arg%" == "/?" goto help
if /i "%arg%" == "-h" goto help
if /i "%arg%" == "--help" goto help
if /i "%arg%" == "-MSBuild" (
    set MSBuild=%argv%
    shift
)
if /i "%arg%" == "-SignType" (
    set SignType=%argv%
    shift
)
if /i "%arg%" == "-Configuration" (
    set Configuration=%argv%
    shift
)
if /i "%arg%" == "-ConfigFile" (
    set ConfigFile=%argv%
    shift
)

shift
goto parsearg

:doneargs

if not defined MSBuild echo Location of MSBuild.exe not specified. && goto error
if not defined ConfigFile echo Configuration file not specified. && goto error
if not exist "%MSBuild%" echo The specified MSBuild.exe does not exist. && goto error

set NUGET_PACKAGES=%USERPROFILE%\.nuget\packages
set _signtoolexe=%NUGET_PACKAGES%\RoslynTools.SignTool\1.0.0-beta2-dev3\tools\SignTool.exe
set SignToolArgs=-msbuildPath %MSBuild% -config "%ConfigFile%" -nugetPackagesPath "%NUGET_PACKAGES%"
if /i "%SignType%" == "real" goto runsigntool
if /i "%SignType%" == "test" set SignToolArgs=%SignToolArgs% -testSign && goto runsigntool
set SignToolArgs=%SignToolArgs% -test

:runsigntool

if not exist "%_signtoolexe%" echo The signing tool could not be found at location '%_signtoolexe%' && goto error
set SignToolArgs=%SignToolArgs% "%scriptdir%..\..\%Configuration%"
echo "%_signtoolexe%" %SignToolArgs%
     "%_signtoolexe%" %SignToolArgs%
if errorlevel 1 goto error
goto :EOF

:help
echo Usage: %0 -MSBuild path\to\msbuild.exe -ConfigFile path\to\SignToolData.json [-SignType ^<real/test^>]
goto :EOF

:error
echo Error running the sign tool.
exit /b 1
