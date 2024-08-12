Write-Host "Publish and Execute: net9.0 - Equality"

$build_output = dotnet publish -restore -c release -f:net9.0 $(Join-Path $PSScriptRoot Equality.fsproj)

# Checking that the build succeeded
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Build failed with exit code ${LASTEXITCODE}"
    Write-Error "${build_output}" -ErrorAction Stop
}

$process = Start-Process `
    -FilePath $(Join-Path $PSScriptRoot bin\release\net9.0\win-x64\publish\Equality.exe) `
    -Wait `
    -NoNewWindow `
    -PassThru `
    -RedirectStandardOutput $(Join-Path $PSScriptRoot output.txt)

# Checking that the test passed
$output = Get-Content $(Join-Path $PSScriptRoot output.txt)
$expected = "All tests passed"
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Test failed with exit code ${LASTEXITCODE}" -ErrorAction Stop
}
if ($output -eq $expected)
{
    Write-Host "Test passed"
}
else
{
    Write-Error "Test failed with unexpected output:`nExpected:`n`t${expected}`nActual`n`t${output}" -ErrorAction Stop
}
