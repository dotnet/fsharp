param(
    [Parameter(Mandatory = $true)][string]$Cache,
    [Parameter(Mandatory = $true)][string]$TestsPath,
    [string]$Repository,
    [string]$Revision,
    [string[]]$BeforeSteps = $null,
    [string[]]$Projects,
    [string[]]$Configurations = @("Debug")
)

class TestCase {
    [CheckoutSpec]$CheckoutSpec
    [string[]]$Projects
    [string[]]$Configurations = @("Debug")
}

class Tests {
    [string]$Cache
    [TestCase[]]$Tests
    # TODO FSC path
}

class CheckoutSpec {
    [string]$Repository
    [string]$Revision
    [string[]]$BeforeSteps = @()
    
    [string]GetDir($CacheBase){
        return Join-Path (Join-Path $CacheBase $this.Repository) $this.Revision
    }
    [string]GetRepositoryUrl(){
        $repo = $this.Repository
        return "https://github.com/$repo"
    }
}

class Checkout {
    [string]$Cache
    [CheckoutSpec]$Spec    
    [string]GetDir(){
        return $this.Spec.GetDir($this.Cache)
    }
    [string]GetRepositoryUrl(){
        return $this.Spec.GetRepositoryUrl()
    }
}

function Checkout-Codebase {
    [OutputType([string])]
    param(
        [Parameter(Mandatory = $true)][Checkout]$checkout
    )
    $repositoryUrl = $checkout.GetRepositoryUrl()
    $cacheDir = $checkout.GetDir()

    if(Test-Path -Path $cacheDir){
        Write-Host "'$cacheDir' exists - assuming that it contains the correct code revision."
    } else {
        Write-Host "git clone '$repositoryUrl' into '$cacheDir'"
        git clone $repositoryUrl $cacheDir > Write-Host
        Write-Host "git clone finished"
    }
}

Set-StrictMode -Version Latest
$oldLocation = Get-Location

function SingleTest{
    param(
        [Parameter(Mandatory = $true)][string]$Cache,
        [Parameter(Mandatory = $true)][TestCase]$Test
    )
    $checkoutSpec = $test.CheckoutSpec
    $checkout = [Checkout]::new()
    $checkout.Cache = $Cache
    $checkout.Spec = $checkoutSpec
    Checkout-Codebase -Checkout $checkout
    $checkoutDir = $checkout.GetDir()
    Set-Location $checkoutDir

    $beforeSteps = $checkoutSpec.BeforeSteps
    if($null -eq $beforeSteps){
        $beforeSteps = @()
    }
    foreach ($step in $beforeSteps) {
        Write-Host "Running BeforeStep: '$step'"
        Invoke-Expression "$step" > $null
    }

    function BuildProjectCollectTimings{
        param(
            [Parameter(Mandatory = $true)][string]$Project,
            [Parameter(Mandatory = $true)][string]$Configuration
        )
        $projectDir = (Get-ChildItem -Path $Project -File).DirectoryName
        $projectName = (Get-ChildItem -Path $Project -File).Name
        Set-Location $projectDir
        dotnet build -c $Configuration $Project > $null
        $logPath = "msbuild.log"
        dotnet build -c $Configuration --no-incremental --no-dependencies $projectName /p:OtherFlags="--times" -fl "-flp:logfile=$logPath;verbosity=detailed" > $null
        $lines = Get-Content $logPath | Select-String -pattern "Realdelta" | % {$_.Line.Trim()}
        return $lines
    }

    foreach ($configuration in $Test.Configurations) {
        foreach ($project in $Test.Projects) {
            $projectPath = Join-Path $checkoutDir -ChildPath $Project
            if(-not (Test-Path -Path $projectPath)) {
                throw "Project file '$projectPath' does not exist."
            }
            $lines = BuildProjectCollectTimings -Project $projectPath -Configuration $configuration
            Write-Host "Timings for '$configuration - $project':"
            $lines | Write-Host
        }
    }
}

function Run-Tests{
    param(
        [Parameter(Mandatory = $true)][Tests]$Tests
    )
    foreach ($test in $Tests.Tests) {
        SingleTest -Cache $Tests.Cache -Test $test
    }
}

function Build-SingleTest {
    $checkoutSpec = [CheckoutSpec]::new()
    $checkoutSpec.Repository = $Repository
    $checkoutSpec.Revision = $Revision

    $test = [TestCase]::new()
    $test.CheckoutSpec = $checkoutSpec
    $test.Projects = $Projects
    $test.Configuration = $Configuration

    $tests = [Tests]::new()
    $tests.Cache = $Cache
    $tests.Tests = @($test)
}

function Parse-Tests {
    return Get-Content $TestsPath | ConvertFrom-Json
}

#$tests = Build-SingleTest
$tests = [Tests]::new()
$tests.Cache = $Cache
$tests.Tests = Parse-Tests
Run-Tests -Tests $tests

Set-Location $oldLocation