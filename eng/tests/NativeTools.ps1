# Verify Perl bootstrapping without downloading tools or running the full build.
$ErrorActionPreference = "Stop"

$buildScript = Join-Path $PSScriptRoot "..\Build.ps1"
$tokens = $null
$parseErrors = $null
$ast = [System.Management.Automation.Language.Parser]::ParseFile($buildScript, [ref]$tokens, [ref]$parseErrors)
if ($parseErrors.Count -ne 0) {
    throw "Unable to parse $buildScript"
}

$buildBody = $ast.EndBlock.Statements | Where-Object { $_ -is [System.Management.Automation.Language.TryStatementAst] }
$statements = $buildBody.Body.Statements
$start = ($statements | Where-Object { $_.Extent.Text -eq "TryDownloadDotnetFrameworkSdk" }).Extent.EndOffset
$end = ($statements | Where-Object { $_.Extent.Text -like '$dotnetPath = InitializeDotNetCli*' }).Extent.StartOffset
$source = Get-Content $buildScript -Raw
$initialize = [scriptblock]::Create($source.Substring($start, $end - $start).Replace('$PSScriptRoot', '$fixture'))

$fixture = Join-Path ([System.IO.Path]::GetTempPath()) ("fsharp-native-tools-" + [guid]::NewGuid())
$installer = Join-Path $fixture "common\native\install-tool.ps1"
New-Item -ItemType Directory -Path (Split-Path $installer) -Force | Out-Null
@'
param($ToolName, $InstallPath, $BaseUri, $CommonLibraryDirectory, $Version)
if ($scenario -eq "InstallFailure") {
    exit 42
}
if ($scenario -eq "MissingExecutable") {
    exit 0
}
$perl = Join-Path $InstallPath "perl\$Version\perl\bin\perl.exe"
New-Item -ItemType Directory -Path (Split-Path $perl) -Force | Out-Null
New-Item -ItemType File -Path $perl -Force | Out-Null
exit 0
'@ | Set-Content $installer

$originalPath = $env:PATH
$originalPerl = $env:PERL5Path
try {
    foreach ($scenario in "Missing", "Present", "InstallFailure", "MissingExecutable", "Local") {
        & {
            $ToolsDir = Join-Path $fixture $scenario
            $expectedPerl = Join-Path $ToolsDir "bin\perl\5.38.2.2\perl\bin\perl.exe"
            $env:PATH = $originalPath
            if ($scenario -ne "Local") {
                $NativeToolsOnMachine = $true
            }

            function Get-Command($Name, $ErrorAction) {
                if ($Name -ne "perl.exe") {
                    throw "Unexpected command lookup: $Name"
                }
                if ($scenario -eq "Present" -or
                    ((Test-Path $expectedPerl) -and ($env:PATH.Split(';') -contains (Split-Path $expectedPerl)))) {
                    [pscustomobject]@{ Source = $expectedPerl }
                }
            }

            function InitializeNativeTools {
                if ($scenario -ne "Local" -and -not (Get-Command "perl.exe")) {
                    throw "Perl must be available before native tool path promotion."
                }
                $script:initialized = $true
                return $ToolsDir
            }

            $script:initialized = $false
            $failure = $null
            try {
                . $initialize
            }
            catch {
                $failure = $_
            }

            if ($scenario -eq "InstallFailure") {
                if (-not $failure -or $script:initialized -or $failure.Exception.Message -notmatch "42") {
                    throw "A failed Perl install must stop before native tool initialization and report its exit code."
                }
            }
            elseif ($scenario -eq "MissingExecutable") {
                if (-not $failure -or $script:initialized -or -not $failure.Exception.Message.Contains($expectedPerl)) {
                    throw "An incomplete Perl install must stop before native tool initialization and report the missing executable."
                }
            }
            else {
                if ($failure) { throw $failure }
                $expectedPath = if ($scenario -eq "Local") {
                    Join-Path $ToolsDir "perl\5.38.2.2\perl\bin\perl.exe"
                } else {
                    $expectedPerl
                }
                if (-not $script:initialized -or $env:PERL5Path -ne $expectedPath) {
                    throw "Incorrect Perl initialization for $scenario."
                }
                if ($scenario -eq "Present" -and (Test-Path $ToolsDir)) {
                    throw "Perl already on PATH must not be downloaded."
                }
            }
            Write-Host "$scenario passed"
        }
    }
}
finally {
    $env:PATH = $originalPath
    $env:PERL5Path = $originalPerl
    Remove-Item -LiteralPath $fixture -Recurse -Force
}
