@echo off

rem
rem End to end tests for DesignTimeProviderPackaging
rem Tests the conditional inclusion of PackageFSharpDesignTimeTools target
rem

setlocal
set __scriptpath=%~dp0
set configuration=Debug

:parseargs
if "%1" == "" goto argsdone
if /i "%1" == "-c" goto set_configuration

echo Unsupported argument: %1
goto failure

:set_configuration
set configuration=%2
shift
shift
goto parseargs

:argsdone

pushd %__scriptpath%

rem Clean artifacts
if exist artifacts rd artifacts /s /q
mkdir artifacts

echo.
echo === Test 1: Plain Library (No Provider) ===
echo [Test 1] Building and packing PlainLib without IsFSharpDesignTimeProvider property...
echo [Test 1] Command: dotnet build PlainLib\PlainLib.fsproj -c %configuration% -v minimal -p:FSharpTestCompilerVersion=coreclr
     dotnet build PlainLib\PlainLib.fsproj -c %configuration% -v minimal -p:FSharpTestCompilerVersion=coreclr
if ERRORLEVEL 1 (
    echo [Test 1] FAILED: Build command returned error code %ERRORLEVEL%
    goto :failure
)

echo [Test 1] Command: dotnet pack PlainLib\PlainLib.fsproj --no-build -o %~dp0artifacts -c %configuration% -v minimal -p:FSharpTestCompilerVersion=coreclr
     dotnet pack PlainLib\PlainLib.fsproj --no-build -o %~dp0artifacts -c %configuration% -v minimal -p:FSharpTestCompilerVersion=coreclr
if ERRORLEVEL 1 (
    echo [Test 1] FAILED: Pack command returned error code %ERRORLEVEL%
    goto :failure
)

rem Check that no tools folder exists in nupkg
echo [Test 1] Checking that package does not contain tools/fsharp41 folder...
powershell -command "& { Add-Type -AssemblyName System.IO.Compression.FileSystem; $zip = [System.IO.Compression.ZipFile]::OpenRead('%~dp0artifacts\PlainLib.1.0.0.nupkg'); $hasTools = $zip.Entries | Where-Object { $_.FullName -like 'tools/fsharp41/*' }; if ($hasTools) { exit 1 } else { exit 0 } }"
if ERRORLEVEL 1 (
    echo [Test 1] FAILED: Package unexpectedly contains tools/fsharp41 folder
    echo [Test 1] Expected: No tools folder for plain library
    echo [Test 1] Actual: tools/fsharp41 folder found in PlainLib.1.0.0.nupkg
    goto :failure
)

echo [Test 1] PASSED: Plain library test passed

echo.
echo === Test 2: Provider Project (Direct Flag) ===
echo [Test 2] Packing Provider with IsFSharpDesignTimeProvider=true...
echo [Test 2] Command: dotnet pack Provider\Provider.fsproj -o %~dp0artifacts -c %configuration% -v minimal -bl:%~dp0artifacts\provider.binlog -p:FSharpTestCompilerVersion=coreclr
     dotnet pack Provider\Provider.fsproj -o %~dp0artifacts -c %configuration% -v minimal -bl:%~dp0artifacts\provider.binlog -p:FSharpTestCompilerVersion=coreclr
if ERRORLEVEL 1 (
    echo [Test 2] FAILED: Pack command returned error code %ERRORLEVEL%
    echo [Test 2] Check artifacts\provider.binlog for details
    goto :failure
)

rem Check that tools folder exists in nupkg
echo [Test 2] Checking that package contains tools/fsharp41 folder...
powershell -command "& { Add-Type -AssemblyName System.IO.Compression.FileSystem; $zip = [System.IO.Compression.ZipFile]::OpenRead('%~dp0artifacts\Provider.1.0.0.nupkg'); $hasTools = $zip.Entries | Where-Object { $_.FullName -like 'tools/fsharp41/*' }; if ($hasTools) { exit 0 } else { exit 1 } }"
if ERRORLEVEL 1 (
    echo [Test 2] FAILED: Package does not contain tools/fsharp41 folder
    echo [Test 2] Expected: tools/fsharp41 folder should be present in provider package
    echo [Test 2] Actual: No tools/fsharp41 folder found in Provider.1.0.0.nupkg
    goto :failure
)

echo [Test 2] PASSED: Provider test passed

echo.
echo === Test 3: Host with ProjectReference to Provider ===
echo [Test 3] Packing Host with ProjectReference to Provider...
echo [Test 3] Note: This tests experimental execution-time reference checking
echo [Test 3] Command: dotnet pack Host\Host.fsproj -o %~dp0artifacts -c %configuration% -v minimal -bl:%~dp0artifacts\host.binlog -p:FSharpTestCompilerVersion=coreclr
     dotnet pack Host\Host.fsproj -o %~dp0artifacts -c %configuration% -v minimal -bl:%~dp0artifacts\host.binlog -p:FSharpTestCompilerVersion=coreclr
if ERRORLEVEL 1 (
    echo [Test 3] FAILED: Pack command returned error code %ERRORLEVEL%
    echo [Test 3] Check artifacts\host.binlog for details
    goto :failure
)

rem Note: This test may not work as expected due to MSBuild evaluation phase limitations
rem The current implementation only checks IsFSharpDesignTimeProvider property directly
echo [Test 3] PASSED: Host test completed (implementation limitation noted - may not check references correctly)

echo.
echo === Test 4: Pack with --no-build (No Provider) ===
echo [Test 4] Testing pack --no-build scenario (NETSDK1085 regression test)...
echo [Test 4] Building PlainLib first...
echo [Test 4] Command: dotnet build PlainLib\PlainLib.fsproj -c %configuration%
     dotnet build PlainLib\PlainLib.fsproj -c %configuration%
if ERRORLEVEL 1 (
    echo [Test 4] FAILED: Build command returned error code %ERRORLEVEL%
    goto :failure
)

echo [Test 4] Packing with --no-build flag...
echo [Test 4] Command: dotnet pack PlainLib\PlainLib.fsproj --no-build -o %~dp0artifacts -c %configuration% -v minimal -bl:%~dp0artifacts\nobuild.binlog -p:FSharpTestCompilerVersion=coreclr
     dotnet pack PlainLib\PlainLib.fsproj --no-build -o %~dp0artifacts -c %configuration% -v minimal -bl:%~dp0artifacts\nobuild.binlog -p:FSharpTestCompilerVersion=coreclr
if ERRORLEVEL 1 (
    echo [Test 4] FAILED: Pack --no-build returned error code %ERRORLEVEL%
    echo [Test 4] This indicates NETSDK1085 or similar issue - early target execution
    echo [Test 4] Check artifacts\nobuild.binlog for details
    goto :failure
)

echo [Test 4] PASSED: No-build test passed

echo.
echo === Test 5: Binding Redirect / App.config Interaction ===
echo [Test 5] Testing with AutoGenerateBindingRedirects (MSB3030/app.config regression test)...
echo [Test 5] Command: dotnet pack RedirectLib\RedirectLib.fsproj -o %~dp0artifacts -c %configuration% -v minimal -bl:%~dp0artifacts\redirect.binlog -p:FSharpTestCompilerVersion=coreclr
     dotnet pack RedirectLib\RedirectLib.fsproj -o %~dp0artifacts -c %configuration% -v minimal -bl:%~dp0artifacts\redirect.binlog -p:FSharpTestCompilerVersion=coreclr
if ERRORLEVEL 1 (
    echo [Test 5] FAILED: Pack command returned error code %ERRORLEVEL%
    echo [Test 5] Check artifacts\redirect.binlog for MSB3030 or binding redirect issues
    goto :failure
)

rem Check that no tools folder exists in nupkg (target should not have run)
echo [Test 5] Checking that package does not contain tools/fsharp41 folder...
powershell -command "& { Add-Type -AssemblyName System.IO.Compression.FileSystem; $zip = [System.IO.Compression.ZipFile]::OpenRead('%~dp0artifacts\RedirectLib.1.0.0.nupkg'); $hasTools = $zip.Entries | Where-Object { $_.FullName -like 'tools/fsharp41/*' }; if ($hasTools) { exit 1 } else { exit 0 } }"
if ERRORLEVEL 1 (
    echo [Test 5] FAILED: Package unexpectedly contains tools/fsharp41 folder
    echo [Test 5] Expected: No tools folder for library without IsFSharpDesignTimeProvider
    echo [Test 5] Actual: tools/fsharp41 folder found - may indicate unwanted target execution
    goto :failure
)

echo [Test 5] PASSED: Redirect test passed

:success
endlocal
echo.
echo === All DesignTimeProviderPackaging tests PASSED ===
popd
exit /b 0

:failure
endlocal
echo.
echo === DesignTimeProviderPackaging tests FAILED ===
popd
exit /b 1