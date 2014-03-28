param(
   [string] $testPath,
   [string] $fscPath
)

$errorActionPreference = 'Stop'

if(-not (Test-Path "$testPath\dont.run.peverify"))
{
    # seed for random selection is based on build # and test area
    # this ensures re-runs are predictable, but different test areas get different random choices
    $seed = (dir $fscPath).VersionInfo.FileBuildPart -bxor $testPath.GetHashCode()

    # 50% chance that we will skip PEVerify step
    $skipPEVerify = (Get-Random -SetSeed $seed) % 2

    if($skipPEVerify){ 'Randomly decided not to do peverify' | out-file "$testPath\dont.run.peverify" }
}