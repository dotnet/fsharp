function CheckTrim($root, $tfm, $outputfile, $expected_len) {
    Write-Host "Publish and Execute: ${tfm} - ${root}"

    $cwd = Get-Location
    Set-Location (Join-Path $PSScriptRoot "${root}")
    $build_output = dotnet publish -restore -c release -f:$tfm $root.fsproj -bl:"../../../../artifacts/log/Release/AheadOfTime/Trimming/${root}_${tfm}.binlog"
    Set-Location ${cwd}
    if (-not ($LASTEXITCODE -eq 0))
    {
        Write-Error "Build failed with exit code ${LASTEXITCODE}"
        Write-Error "${build_output}" -ErrorAction Stop
    }

    $process = Start-Process -FilePath $(Join-Path $PSScriptRoot "${root}\bin\release\${tfm}\win-x64\publish\${root}.exe") -Wait -NoNewWindow -PassThru -RedirectStandardOutput $(Join-Path $PSScriptRoot "output.txt")
    $output = Get-Content $(Join-Path $PSScriptRoot "output.txt")
    # Checking that it is actually running.
    if (-not ($LASTEXITCODE -eq 0))
    {
        Write-Error "Test failed with exit code ${LASTEXITCODE}" -ErrorAction Stop
    }
 
    # Checking that the output is as expected.
    $expected = "All tests passed"
    if (-not ($output -eq $expected))
    {
        Write-Error "Test failed with unexpected output:`nExpected:`n`t${expected}`nActual`n`t${output}" -ErrorAction Stop
    }
 
    # Checking that the trimmed outputfile binary is of expected size (needs adjustments if test is updated).
    $file = Get-Item (Join-Path $PSScriptRoot "${root}\bin\release\${tfm}\win-x64\publish\${outputfile}")
    $file_len = $file.Length
    if (-not ($expected_len -eq -1 -or $file_len -eq $expected_len))
    {
        Write-Error "Test failed with unexpected ${tfm}  -  trimmed ${outputfile} length:`nExpected:`n`t${expected_len} Bytes`nActual:`n`t${file_len} Bytes`nEither codegen or trimming logic have changed. Please investigate and update expected dll size or report an issue." -ErrorAction Stop
    }
}


# Check net8.0 trimmed assemblies
CheckTrim -root "SelfContained_Trimming_Test" -tfm "net8.0" -outputfile "FSharp.Core.dll" -expected_len 287744

# Check net472 trimmed assemblies -- net472 doesn't actually trim, this just checks that everything is usable when published trimmed
CheckTrim -root "SelfContained_Trimming_Test" -tfm "net472" -outputfile "FSharp.Core.dll" -expected_len -1

# Disabled due to bug:   https://github.com/dotnet/fsharp/issues/15167
# Check net472 trimmed / static linked assemblies
CheckTrim -root "StaticLinkedFSharpCore_Trimming_Test" -tfm "net472" -outputfile "StaticLinkedFSharpCore_Trimming_Test.exe" -expected_len -1

# Check net8.0 trimmed assemblies
CheckTrim -root "StaticLinkedFSharpCore_Trimming_Test" -tfm "net8.0" -outputfile "StaticLinkedFSharpCore_Trimming_Test.dll" -expected_len 8821248
