Write-Host "Publish and Execute: net8.0 - Equality"

$build_output = dotnet publish -restore -c release -f:net8.0 $(Join-Path $PSScriptRoot Equality.fsproj)

# Checking that the build succeeded
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Build failed with exit code ${LASTEXITCODE}"
    Write-Error "${build_output}" -ErrorAction Stop
}

$process = Start-Process `
    -FilePath $(Join-Path $PSScriptRoot bin\release\net8.0\win-x64\publish\Equality.exe) `
    -Wait `
    -NoNewWindow `
    -PassThru `
    -RedirectStandardOutput $(Join-Path $PSScriptRoot output.txt)

$output = Get-Content $(Join-Path $PSScriptRoot output.txt)

# Checking that it is actually running.
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Test failed with exit code ${LASTEXITCODE}" -ErrorAction Stop
}

# Checking that the output is as expected.
$expected = "All tests passed"
if ($output -ne $expected)
{
    Write-Error "Test failed with unexpected output:`nExpected:`n`t${expected}`nActual`n`t${output}" -ErrorAction Stop
}
