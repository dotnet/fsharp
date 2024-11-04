# Cross-platform PowerShell script to verify the integrity of the produced dlls, using dotnet-ilverify.

# Set build script based on which OS we're running on - Windows (build.cmd), Linux or macOS (build.sh)

Write-Host "Checking whether running on Windows: $IsWindows"

[string] $repo_path = (Get-Item -Path $PSScriptRoot).Parent

Write-Host "Repository path: $repo_path"

[string] $script = if ($IsWindows) { Join-Path $repo_path "build.cmd" } else { Join-Path $repo_path "build.sh" }

# Set configurations to build
[string[]] $configurations = @("Debug", "Release")

# The following are not passing ilverify checks, so we ignore them for now
[string[]] $ignore_errors = @() # @("StackUnexpected", "UnmanagedPointer", "StackByRef", "ReturnPtrToStack", "ExpectedNumericType", "StackUnderflow")

[string] $default_tfm = "netstandard2.0"

[string] $artifacts_bin_path = Join-Path $repo_path "artifacts" "bin"

# List projects to verify, with TFMs
$projects = @{
    "FSharp.Core" = @($default_tfm, "netstandard2.1")
    "FSharp.Compiler.Service" = @($default_tfm, "net9.0")
}

# Run build script for each configuration (NOTE: We don't build Proto)
foreach ($configuration in $configurations) {
    Write-Host "Building $configuration configuration..."
#    & $script -c $configuration
#    if ($LASTEXITCODE -ne 0 -And $LASTEXITCODE -ne '') {
#        Write-Host "Build failed for $configuration configuration (last exit code: $LASTEXITCODE)."
#        exit 1
#    }
}

# Check if ilverify is installed and available from the tool list (using `dotnet tool list -g `), and install it globally if not found.
Write-Host "Checking if dotnet-ilverify is installed..."
$dotnet_ilverify = dotnet tool list -g | Select-String -SimpleMatch -CaseSensitive "dotnet-ilverify"
if ($dotnet_ilverify -eq "") {
    Write-Host " dotnet-ilverify is not installed. Installing..."
    dotnet tool install dotnet-ilverify -g
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
foreach ($project in $projects.Keys) {
    foreach ($tfm in $projects[$project]) {
        foreach ($configuration in $configurations) {
            $dll_path = "$artifacts_bin_path/$project/$configuration/$tfm/$project.dll"
            if (-not (Test-Path $dll_path)) {
                Write-Host "DLL not found: $dll_path"
                continue
            }
            Write-Host "Verifying $dll_path..."
            # If there are any errors to ignore in the array, ignore them with `-g` flag
            $ignore_errors_string =
                if ($ignore_errors.Length -gt 0) {
                    $ignore_errors | ForEach-Object {
                        "-g $_"
                    }
                } else { "" }
            $ilverify_cmd = "dotnet ilverify $dll_path -r '$runtime_path/*.dll' -r '$artifacts_bin_path/$project/$configuration/$tfm/FSharp.Core.dll' $ignore_errors_string"
            Write-Host "Running ilverify command:`n $ilverify_cmd"
            $ilverify_output = @(Invoke-Expression "& $ilverify_cmd")
            # Check ilverify output for errors by simply checking if it contains `[IL]: Error [` substring:
            if ($ilverify_output -match "\[IL\]: Error \[") {
                Write-Host "Errors found in ${dll_path}:"
                $ilverify_output | ForEach-Object {
                    Write-Host " $_"
                }
                exit 1
            }
        }
    }
}