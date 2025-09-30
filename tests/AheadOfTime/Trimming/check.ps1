function CheckTrim($root, $tfm, $outputfile, $expected_len) {
    Write-Host "Publish and Execute: ${tfm} - ${root}"
    Write-Host "Expecting ${expected_len} for ${outputfile}"

    $cwd = Get-Location
    Set-Location (Join-Path $PSScriptRoot "${root}")
    $build_output = dotnet publish -restore -c release -f:$tfm "${root}.fsproj" -bl:"../../../../artifacts/log/Release/AheadOfTime/Trimming/${root}_${tfm}.binlog"
    Set-Location ${cwd}
    if ($LASTEXITCODE -ne 0)
    {
        Write-Error "Build failed with exit code ${LASTEXITCODE}"
        Write-Error "${build_output}" -ErrorAction Stop
    }

    $process = Start-Process -FilePath $(Join-Path $PSScriptRoot "${root}\bin\release\${tfm}\win-x64\publish\${root}.exe") -Wait -NoNewWindow -PassThru -RedirectStandardOutput $(Join-Path $PSScriptRoot "output.txt")

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

    # Checking that the trimmed outputfile binary is of expected size (needs adjustments if test is updated).
    $file = Get-Item (Join-Path $PSScriptRoot "${root}\bin\release\${tfm}\win-x64\publish\${outputfile}")
    $file_len = $file.Length
    if ($expected_len -eq -1)
    {
        Write-Host "Actual ${tfm} - trimmed ${outputfile} length: ${file_len} Bytes (expected length is placeholder -1, update test with this actual value)"
    }
    elseif ($file_len -ne $expected_len)
    {
        Write-Error "Test failed with unexpected ${tfm}  -  trimmed ${outputfile} length:`nExpected:`n`t${expected_len} Bytes`nActual:`n`t${file_len} Bytes`nEither codegen or trimming logic have changed. Please investigate and update expected dll size or report an issue." -ErrorAction Stop
    }
    
    $fileBeforePublish = Get-Item (Join-Path $PSScriptRoot "${root}\bin\release\${tfm}\win-x64\${outputfile}")
    $sizeBeforePublish = $fileBeforePublish.Length
    $sizeDiff = $sizeBeforePublish - $file_len
    Write-Host "Size of ${tfm} - ${outputfile} before publish: ${sizeBeforePublish} Bytes, which means the diff is ${sizeDiff} Bytes"
}

# NOTE: Trimming now errors out on desktop TFMs, as shown below:
# error NETSDK1124: Trimming assemblies requires .NET Core 3.0 or higher.

# Check net9.0 trimmed assemblies
CheckTrim -root "SelfContained_Trimming_Test" -tfm "net9.0" -outputfile "FSharp.Core.dll" -expected_len 300032

# Check net9.0 trimmed assemblies with static linked FSharpCore
CheckTrim -root "StaticLinkedFSharpCore_Trimming_Test" -tfm "net9.0" -outputfile "StaticLinkedFSharpCore_Trimming_Test.dll" -expected_len 9154048

# Check net9.0 trimmed assemblies with F# metadata resources removed
CheckTrim -root "FSharpMetadataResource_Trimming_Test" -tfm "net9.0" -outputfile "FSharpMetadataResource_Trimming_Test.dll" -expected_len 7607296

