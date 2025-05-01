# Cross-platform PowerShell script to verify the integrity of the produced dlls, using dotnet-ilverify.

# Set build script based on which OS we're running on - Windows (build.cmd), Linux or macOS (build.sh)

Write-Host "Checking whether running on Windows: $IsWindows"

[string] $repo_path = (Get-Item -Path $PSScriptRoot).Parent.Parent

Write-Host "Repository path: $repo_path"

[string] $script = if ($IsWindows) { Join-Path $repo_path "build.cmd" } else { Join-Path $repo_path "build.sh" }
[string] $additional_arguments = if ($IsWindows) { "-noVisualStudio" } else { "" }

# Set configurations to build
[string[]] $configurations = @("Debug", "Release")

# The following are not passing ilverify checks, so we ignore them for now
[string[]] $ignore_errors = @() # @("StackUnexpected", "UnmanagedPointer", "StackByRef", "ReturnPtrToStack", "ExpectedNumericType", "StackUnderflow")

[string] $default_tfm = "netstandard2.0"

[string] $artifacts_bin_path = Join-Path (Join-Path $repo_path "artifacts") "bin"

# List projects to verify, with TFMs
$projects = @{
    "FSharp.Core" = @($default_tfm, "netstandard2.1")
    "FSharp.Compiler.Service" = @($default_tfm, "net9.0")
}

# Check ilverify can run
Write-Host "ILVerify version:"
dotnet ilverify --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "Could not run ILVerify, see output above"
    exit 2
}

# Run build script for each configuration (NOTE: We don't build Proto)
foreach ($configuration in $configurations) {
    Write-Host "Building $configuration configuration..."
    & $script -c $configuration $additional_arguments
    if ($LASTEXITCODE -ne 0 -And $LASTEXITCODE -ne '') {
        Write-Host "Build failed for $configuration configuration (last exit code: $LASTEXITCODE)."
        exit 1
    }
}

# Check if ilverify is installed and available from the tool list (using `dotnet tool list -g `), and install it globally if not found.
Write-Host "Checking if dotnet-ilverify is installed..."
$dotnet_ilverify = dotnet tool list -g | Select-String -SimpleMatch -CaseSensitive "dotnet-ilverify"

if ([string]::IsNullOrWhiteSpace($dotnet_ilverify)) {
    Write-Host " dotnet-ilverify is not installed. Installing..."
    dotnet tool install dotnet-ilverify -g --prerelease
} else {
    Write-Host " dotnet-ilverify is installed:`n  $dotnet_ilverify"
}

# Get the path to latest currently installed runtime
[string[]] $runtimes = @(dotnet --list-runtimes | Select-String -SimpleMatch -CaseSensitive -List "Microsoft.NETCore.App")
if ($runtimes -eq "") {
    Write-Host "No runtime found. Exiting..."
    exit 1
} else {
    Write-Host "Found the following runtimes: "
    foreach ($runtime in $runtimes) {
        Write-Host " $runtime"
    }
}

# Selecting the most recent runtime (e.g. last one in the list)
[string] $runtime = $runtimes[-1]
Write-Host " Selected runtime:`n  $runtime"

# Parse path to runtime from something like "Microsoft.NETCore.App 5.0.0 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.0]" to "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.0"
[string] $runtime_path = $runtime -replace 'Microsoft.NETCore.App (?<RuntimeVersion>.+) \[(?<RuntimePath>.+)\]', '${RuntimePath}/${RuntimeVersion}'
Write-Host " Using the following path to runtime:`n  $runtime_path"

# Check whether path exists, if it doesn't something unexpected happens and needs investigation
if (-not (Test-Path $runtime_path)) {
    Write-Host "Path to runtime not found. Exiting..."
    exit 1
}

# For every artifact, every configuration and TFM, run a dotnet-ilverify with references from discovered runtime directory:
[bool] $failed = $false
foreach ($project in $projects.Keys) {
    foreach ($tfm in $projects[$project]) {
        foreach ($configuration in $configurations) {
            $dll_path = "$artifacts_bin_path/$project/$configuration/$tfm/$project.dll"
            if (-not (Test-Path $dll_path)) {
                Write-Host "DLL not found: $dll_path"
                exit 1
            }
            Write-Host "Verifying $dll_path..."
            # If there are any errors to ignore in the array, ignore them with `-g` flag
            $ignore_errors_string =
                if ($ignore_errors.Length -gt 0) {
                    $ignore_errors | ForEach-Object {
                        "-g $_"
                    }
                } else { "" }
            $ilverify_cmd = "dotnet ilverify --sanity-checks --tokens $dll_path -r '$runtime_path/*.dll' -r '$artifacts_bin_path/$project/$configuration/$tfm/FSharp.Core.dll' $ignore_errors_string"
            Write-Host "Running ilverify command:`n $ilverify_cmd"

            # Append output to output array
            $ilverify_output = @(Invoke-Expression "& $ilverify_cmd" -ErrorAction SilentlyContinue)

            # Normalize output, get rid of paths in log like
            #  [IL]: Error [StackUnexpected]: [/Users/u/code/fsharp3/artifacts/bin/FSharp.Compiler.Service/Release/net9.0/FSharp.Core.dll : Microsoft.FSharp.Collections.ArrayModule+Parallel::Choose([FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,Microsoft.FSharp.Core.FSharpOption`1<!!1>>, !!0[])][offset 0x00000081][found Byte] Unexpected type on the stack.
            # This is a quick and dirty way to do it, but it works for now.
            $ilverify_output = $ilverify_output | ForEach-Object {
                if ($_ -match "\[IL\]: Error \[") {
                    $parts = $_ -split " "
                    "$($parts[0]) $($parts[1]) $($parts[2]) $($parts[4..$parts.Length])"
                } elseif ($_ -match "Error\(s\) Verifying") {
                    # do nothing
                } else {
                    $_
                }
            }

            $baseline_file = Join-Path $repo_path "tests/ILVerify" "ilverify_${project}_${configuration}_${tfm}.bsl"

            $baseline_actual_file = [System.IO.Path]::ChangeExtension($baseline_file, 'bsl.actual')

            if (-not (Test-Path $baseline_file)) {
                Write-Host "Baseline file not found: $baseline_file"
                if ($env:TEST_UPDATE_BSL -eq "1") {
                    Write-Host "Creating initial baseline file: $baseline_file"
                    $ilverify_output | Set-Content $baseline_file
                } else {
                    Write-Host "Creating .actual baseline file: $baseline_actual_file"
                    $ilverify_output | Set-Content $baseline_actual_file
                    $failed = $true
                }
                continue
            }

            # Read baseline file into string array
            [string[]] $baseline = Get-Content $baseline_file

            if ($baseline.Length -eq 0) {
                Write-Host "Baseline file is empty: $baseline_file"
                if ($env:TEST_UPDATE_BSL -eq "1") {
                    Write-Host "Updating empty baseline file: $baseline_file"
                    $ilverify_output | Set-Content $baseline_file
                } else {
                    Write-Host "Creating initial .actual baseline file: $baseline_actual_file"
                    $ilverify_output | Set-Content $baseline_actual_file
                    $failed = $true
                }
                continue
            }

            # Compare contents of both arrays, error if they're not equal

            $cmp = Compare-Object $ilverify_output $baseline

            if (-not $cmp) {
                Write-Host "ILverify output matches baseline."
            } else {
                Write-Host "ILverify output does not match baseline, differences:"

                $cmp | Format-Table -AutoSize -Wrap | Out-String | Write-Host

                # Update baselines if TEST_UPDATE_BSL is set to 1
                if ($env:TEST_UPDATE_BSL -eq "1") {
                    Write-Host "Updating baseline file: $baseline_file"
                    $ilverify_output | Set-Content $baseline_file
                } else {
                    $ilverify_output | Set-Content $baseline_actual_file
                }
                $failed = $true
                continue
            }


        }
    }
}

if ($failed) {
    Write-Host "ILverify failed."
    exit 1
}

exit 0
