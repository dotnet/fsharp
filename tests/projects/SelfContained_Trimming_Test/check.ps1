$output = .\bin\Release\net7.0\win-x64\publish\SelfContained_Trimming_Test.exe

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

# Checking that FSharp.Core binary is of expected size (needs adjustments if test is updated).
$expected_len = 245248 # In bytes
$file = Get-Item .\bin\Release\net7.0\win-x64\publish\FSharp.Core.dll
$file_len = $file.Length
if (-not ($file_len -eq $expected_len))
{
    Write-Error "Test failed with unexpected FSharp.Core length:`nExpected:`n`t${expected_len} Bytes`nActual:`n`t${file_len} Bytes" -ErrorAction Stop
}