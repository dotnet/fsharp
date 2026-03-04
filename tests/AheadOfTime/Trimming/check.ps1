function CheckTrim($root, $tfm, $outputfile, $expected_len, $callerLineNumber) {
    Write-Host "Publish and Execute: ${tfm} - ${root}"
    Write-Host "Expecting ${expected_len} for ${outputfile}"

    $errors = @()
    $scriptFile = $PSCommandPath

    $cwd = Get-Location
    Set-Location (Join-Path $PSScriptRoot "${root}")
    $build_output = dotnet publish -restore -c release -f:$tfm "${root}.fsproj" -bl:"../../../../artifacts/log/Release/AheadOfTime/Trimming/${root}_${tfm}.binlog"
    Set-Location ${cwd}
    if ($LASTEXITCODE -ne 0)
    {
        $errors += "Build failed with exit code ${LASTEXITCODE}"
        Write-Host "##vso[task.logissue type=error;sourcepath=${scriptFile};linenumber=${callerLineNumber}]Build failed for ${root} with exit code ${LASTEXITCODE}"
        return $errors
    }

    $process = Start-Process -FilePath $(Join-Path $PSScriptRoot "${root}\bin\release\${tfm}\win-x64\publish\${root}.exe") -Wait -NoNewWindow -PassThru -RedirectStandardOutput $(Join-Path $PSScriptRoot "output.txt")

    # Checking that the test passed
    $output = Get-Content $(Join-Path $PSScriptRoot output.txt)
    $expected = "All tests passed"
    if ($process.ExitCode -ne 0)
    {
        $errors += "Test failed with exit code $($process.ExitCode)"
        Write-Host "##vso[task.logissue type=error;sourcepath=${scriptFile};linenumber=${callerLineNumber}]Test execution failed for ${root} with exit code $($process.ExitCode)"
    }
    elseif ($output -ne $expected)
    {
        $errors += "Test failed with unexpected output: Expected '${expected}', Actual '${output}'"
        Write-Host "##vso[task.logissue type=error;sourcepath=${scriptFile};linenumber=${callerLineNumber}]Test failed for ${root} with unexpected output: Expected '${expected}', Actual '${output}'"
    }
    else
    {
        Write-Host "Test passed"
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
        $errors += "Test failed with unexpected ${tfm} - trimmed ${outputfile} length: Expected ${expected_len} Bytes, Actual ${file_len} Bytes"
        Write-Host "##vso[task.logissue type=error;sourcepath=${scriptFile};linenumber=${callerLineNumber}]Trimmed ${outputfile} size mismatch for ${root}: Expected ${expected_len} Bytes, Actual ${file_len} Bytes. Either codegen or trimming logic have changed. Please investigate and update expected dll size or report an issue."
    }
    
    $fileBeforePublish = Get-Item (Join-Path $PSScriptRoot "${root}\bin\release\${tfm}\win-x64\${outputfile}")
    $sizeBeforePublish = $fileBeforePublish.Length
    $sizeDiff = $sizeBeforePublish - $file_len
    Write-Host "Size of ${tfm} - ${outputfile} before publish: ${sizeBeforePublish} Bytes, which means the diff is ${sizeDiff} Bytes"

    return $errors
}

# NOTE: Trimming now errors out on desktop TFMs, as shown below:
# error NETSDK1124: Trimming assemblies requires .NET Core 3.0 or higher.

$allErrors = @()

# Check net9.0 trimmed assemblies
$allErrors += CheckTrim -root "SelfContained_Trimming_Test" -tfm "net9.0" -outputfile "FSharp.Core.dll" -expected_len 310784 -callerLineNumber 66

# Check net9.0 trimmed assemblies with static linked FSharpCore
$allErrors += CheckTrim -root "StaticLinkedFSharpCore_Trimming_Test" -tfm "net9.0" -outputfile "StaticLinkedFSharpCore_Trimming_Test.dll" -expected_len 9168384 -callerLineNumber 69

# Check net9.0 trimmed assemblies with F# metadata resources removed
$allErrors += CheckTrim -root "FSharpMetadataResource_Trimming_Test" -tfm "net9.0" -outputfile "FSharpMetadataResource_Trimming_Test.dll" -expected_len 7609344 -callerLineNumber 72

# Report all errors and exit with failure if any occurred
if ($allErrors.Count -gt 0) {
    Write-Host ""
    Write-Host "============================================"
    Write-Host "TRIMMING TESTS FAILED"
    Write-Host "============================================"
    Write-Host "Total errors: $($allErrors.Count)"
    foreach ($err in $allErrors) {
        Write-Error $err
    }
    exit 1
}

Write-Host ""
Write-Host "============================================"
Write-Host "ALL TRIMMING TESTS PASSED"
Write-Host "============================================"
