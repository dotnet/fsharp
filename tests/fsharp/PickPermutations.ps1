param(
   [string] $testPath,  # path to test directory
   [string] $fscPath,   # path to FSC.exe
   [string] $allPermutations   # space-delimited list of all permutation labels
)

$errorActionPreference = 'Stop'

# some permutations do not apply to all test areas
# we don't want to randomly pick a permutation which does not apply to the current test area
$specialCaseCriteria = @{
   'FSC_HW'                = (Test-Path "$testPath\test-hw.*")
   'FSC_BASIC_64'          = ($env:PROCESSOR_ARCHITECTURE -eq 'AMD64')
   'GENERATED_SIGNATURE'   = ((Test-Path "$testPath\test.ml") -and (-not (Test-Path "$testPath\dont.use.generated.signature")))
   'EMPTY_SIGNATURE'       = ((Test-Path "$testPath\test.ml") -and (-not (Test-Path "$testPath\dont.use.empty.signature")))
   'EMPTY_SIGNATURE_OPT'   = ((Test-Path "$testPath\test.ml") -and (-not (Test-Path "$testPath\dont.use.empty.signature")))
   'AS_DLL'                = (-not (Test-Path "$testPath\dont.compile.test.as.dll"))
   'WRAPPER_NAMESPACE'     = ((Test-Path "$testPath\test.ml") -and (-not (Test-Path "$testPath\dont.use.wrapper.namespace")))
   'WRAPPER_NAMESPACE_OPT' = ((Test-Path "$testPath\test.ml") -and (-not (Test-Path "$testPath\dont.use.wrapper.namespace")))
   'FSI_FILE'              = (-not (Test-Path "$testPath\dont.run.as.script"))
   'FSI_STDIN'             = (-not (Test-Path "$testPath\dont.pipe.to.stdin"))
   'FSI_STDIN_OPT'         = (-not (Test-Path "$testPath\dont.pipe.to.stdin"))
   'FSI_STDIN_GUI'         = (-not (Test-Path "$testPath\dont.pipe.to.stdin"))   
}

# seed for random selection is based on build # and test area
# this ensures re-runs are predictable, but different test areas get different random choices
$seed = (dir $fscPath).VersionInfo.FileBuildPart -bxor $testPath.GetHashCode()
$permutations = ($allPermutations -split '\s+') |?{ $specialCaseCriteria[$_] -ne $false } #| Get-Random -SetSeed $seed

$permutations -join ' '