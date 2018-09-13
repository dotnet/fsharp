
# the version under development, update after a release
$version = '4.1.31'

function isVersionTag($tag){
    $v = New-Object Version
    [Version]::TryParse(($tag).Replace('-alpha','').Replace('-beta',''), [ref]$v)
}

# append the AppVeyor build number as the pre-release version
if ($env:appveyor){
    $version = $version + '-b' + [int]::Parse($env:appveyor_build_number).ToString('000')
    if ($env:appveyor_repo_tag -eq 'true' -and (isVersionTag($env:appveyor_repo_tag_name))){
        $version = $env:appveyor_repo_tag_name
    }
    Update-AppveyorBuild -Version $version
} else {
    $version = $version + '-b001'
}

$nuget = (gi .\.nuget\NuGet.exe).FullName

$packagesOutDir = Join-Path $PSScriptRoot "..\release\"

function pack($nuspec){
    $dir = [IO.Path]::GetDirectoryName($nuspec)
    & $nuget pack $nuspec -BasePath "$dir" -Version $version -OutputDirectory "$packagesOutDir" -NoDefaultExcludes -Verbosity d
}

pack(gi .\FSharp.Compiler.Tools.Nuget\FSharp.Compiler.Tools.nuspec)
