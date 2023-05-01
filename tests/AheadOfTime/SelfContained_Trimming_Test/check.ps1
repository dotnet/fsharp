function CheckTrim($tfm, $expected_len) {
    Write-Host "Verify trimming ${tfm}:  Expected length: ${expected_len}"

    $cwd = Get-Location
    Set-Location (Join-Path $PSScriptRoot "bin\Release\${tfm}\win-x64\publish\")
    $output = .\SelfContained_Trimming_Test.exe
    Set-Location ${cwd}

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

    # Checking that the trimmed FSharp.Core binary is of expected size (needs adjustments if test is updated).
    $file = Get-Item (Join-Path $PSScriptRoot "bin\Release\net7.0\win-x64\publish\FSharp.Core.dll")
    $file_len = $file.Length
    if (-not ($file_len -eq $expected_len))
    {
        Write-Error "Test failed with unexpected ${tfm}  -  trimmed FSharp.Core length:`nExpected:`n`t${expected_len} Bytes`nActual:`n`t${file_len} Bytes`nEither codegen or trimming logic have changed. Please investigate and update expected dll size or report an issue." -ErrorAction Stop
    }
}

# Check net472 trimmed assemblies
CheckTrim -tfm "net472" -expected_len 287744

# Check net7.0 trimmed assemblies
CheckTrim -tfm "net7.0" -expected_len 287744
