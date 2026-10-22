# Keep reflection-free paths warning-strict and test captured printf compatibility separately.

$ErrorActionPreference = "Stop"

$cwd = Get-Location
try {
    foreach ($fixture in @(
        @{ Directory = $PSScriptRoot; Name = "NativeAOT_Test"; Expected = "Finished" },
        @{ Directory = (Join-Path $PSScriptRoot "PrintfInterpolation"); Name = "PrintfInterpolation_Test"; Expected = "01:02:03" }
    )) {
        Set-Location $fixture.Directory
        $root = $fixture.Name
        $logDirectory = Join-Path $PSScriptRoot "..\..\..\artifacts\log\Release\AheadOfTime\NativeAOT"
        dotnet publish -restore -c release "$root.fsproj" "-bl:$logDirectory\$root.binlog"
        $publishExit = $LASTEXITCODE
        if ($publishExit -ne 0) {
            throw "$root publish failed with exit code $publishExit"
        }

        # Passing -f would override TargetFramework globally, including referenced projects.
        $propertiesOutput = dotnet msbuild "$root.fsproj" -getProperty:TargetFramework -getProperty:FSharpCoreShippedNetTargetFramework -nologo
        $propertiesExit = $LASTEXITCODE
        if ($propertiesExit -ne 0) { throw "$root framework query failed with exit code $propertiesExit" }
        $props = $propertiesOutput | ConvertFrom-Json
        $tfm = $props.Properties.TargetFramework
        $coreAsset = "lib/$($props.Properties.FSharpCoreShippedNetTargetFramework)/FSharp.Core.dll"

        $assetDirectories = @($fixture.Directory)
        if ($root -eq "PrintfInterpolation_Test") {
            $assetDirectories += Join-Path $fixture.Directory "Library"
        }
        foreach ($directory in $assetDirectories) {
            # Check selected compile/runtime assets, not the manifest listing every packaged lib folder.
            $assets = Join-Path $directory "obj\project.assets.json"
            $assetsJson = Get-Content $assets -Raw | ConvertFrom-Json
            $netAssetSelected = $false
            foreach ($target in $assetsJson.targets.PSObject.Properties) {
                foreach ($package in $target.Value.PSObject.Properties) {
                    if ($package.Name -like "FSharp.Core/*") {
                        $compileKeys = if ($package.Value.compile) { @($package.Value.compile.PSObject.Properties.Name) } else { @() }
                        $runtimeKeys = if ($package.Value.runtime) { @($package.Value.runtime.PSObject.Properties.Name) } else { @() }
                        if (($compileKeys -contains $coreAsset) -and ($runtimeKeys -contains $coreAsset)) {
                            $netAssetSelected = $true
                        }
                    }
                }
            }
            if (-not $netAssetSelected) {
                throw "The $tfm consumer did not resolve the $coreAsset FSharp.Core compile/runtime asset. See $assets."
            }
        }

        $exe = Join-Path $fixture.Directory "bin\release\$tfm\win-x64\publish\$root.exe"
        $stderrPath = Join-Path $logDirectory "$root.stderr"
        $lines = @(& $exe 2> $stderrPath)
        $exitCode = $LASTEXITCODE
        $output = $lines -join "`n"
        $stderr = Get-Content $stderrPath -Raw
        if ($exitCode -ne 0 -or $output -cne $fixture.Expected -or $stderr) {
            throw "$root behavior checks failed (exit $exitCode).`nStdout:`n$output`nStderr:`n$stderr"
        }
    }
} finally {
    Set-Location $cwd
}

Write-Host "NativeAOT test passed."
