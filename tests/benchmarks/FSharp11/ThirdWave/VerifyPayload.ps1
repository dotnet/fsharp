param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [switch] $SourceOnly
)

$ErrorActionPreference = 'Stop'
$PerformanceRoot = [IO.Path]::GetFullPath($PerformanceRoot)
$vmr = 'be46bdda4d6599b80dd4ccd89b2de49d96cbf36d'
$expectedTree = '7904104d6778df42799007d3e4d6e19093144325'
$source = "$PerformanceRoot\fsharp-rc2-source"
$tree = & git -C "$PerformanceRoot\vmr11" rev-parse "${vmr}:src/fsharp/src"
if ($LASTEXITCODE -ne 0 -or $tree -ne $expectedTree) { throw 'Pinned VMR production tree mismatch.' }
$entries = @(& git -C "$PerformanceRoot\vmr11" ls-tree -r "${vmr}:src/fsharp/src")
if ($LASTEXITCODE -ne 0) { throw 'Cannot inventory the VMR production tree.' }
$expected = @()
$paths = foreach ($entry in $entries) {
    if ($entry -notmatch '^\d+ blob ([a-f0-9]{40})\t(.+)$') { throw "Unsupported source-tree entry: $entry" }
    $expected += $Matches[1]
    Join-Path "$source\src" ($Matches[2].Replace('/', '\'))
}
$actual = @($paths | & git -c core.longpaths=true -c core.autocrlf=true hash-object --stdin-paths)
if ($LASTEXITCODE -ne 0 -or $actual.Count -ne $expected.Count) { throw 'Source hashing failed.' }
for ($i = 0; $i -lt $expected.Count; $i++) {
    if ($actual[$i] -ne $expected[$i]) { throw "Production source differs from pinned VMR: $($paths[$i])" }
}
if ($SourceOnly) { Write-Output "Verified $($paths.Count) production files against VMR tree $tree."; return }

function Read-Image([string] $Path) {
    $stream = [IO.File]::OpenRead($Path)
    $pe = [System.Reflection.PortableExecutable.PEReader]::new($stream)
    try {
        $metadata = [System.Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($pe)
        $tfm = $null
        $debugFlags = 0
        foreach ($handle in $metadata.GetAssemblyDefinition().GetCustomAttributes()) {
            $attribute = $metadata.GetCustomAttribute($handle)
            if ($attribute.Constructor.Kind -ne [System.Reflection.Metadata.HandleKind]::MemberReference) { continue }
            $constructor = $metadata.GetMemberReference([System.Reflection.Metadata.MemberReferenceHandle]$attribute.Constructor)
            if ($constructor.Parent.Kind -ne [System.Reflection.Metadata.HandleKind]::TypeReference) { continue }
            $type = $metadata.GetTypeReference([System.Reflection.Metadata.TypeReferenceHandle]$constructor.Parent)
            $name = $metadata.GetString($type.Name)
            $blob = $metadata.GetBlobReader($attribute.Value)
            [void]$blob.ReadUInt16()
            if ($name -eq 'TargetFrameworkAttribute') { $tfm = $blob.ReadSerializedString() }
            if ($name -eq 'DebuggableAttribute') { $debugFlags = $blob.ReadInt32() }
        }
        $native = $pe.PEHeaders.CorHeader.ManagedNativeHeaderDirectory
        $r2r = $null
        if ($native.Size -gt 0) {
            $reader = $pe.GetSectionData($native.RelativeVirtualAddress).GetReader()
            if ($reader.ReadUInt32() -ne 0x00525452) { throw "Invalid R2R signature: $Path" }
            $r2r = @{major=$reader.ReadUInt16(); minor=$reader.ReadUInt16(); header_bytes=$native.Size}
        }
        [ordered]@{
            path=$Path; bytes=$stream.Length; sha256=(Get-FileHash $Path).Hash
            mvid=$metadata.GetGuid($metadata.GetModuleDefinition().Mvid).ToString()
            framework=$tfm; disable_optimizations=($debugFlags -band 256) -ne 0
            machine=$pe.PEHeaders.CoffHeader.Machine.ToString(); ready_to_run=$r2r
        }
    } finally { $pe.Dispose(); $stream.Dispose() }
}

$images = foreach ($name in 'fsc.dll', 'FSharp.Compiler.Service.dll', 'FSharp.Core.dll') {
    $original = if ($name -eq 'FSharp.Core.dll') {
        "$source\artifacts\bin\FSharp.Core\Release\net10.0\$name"
    } else { "$PerformanceRoot\rc2-release-il\$name" }
    $il = Read-Image $original
    $native = Read-Image "$PerformanceRoot\rc2-r2r\$name"
    if (-not $native.ready_to_run -or $native.mvid -ne $il.mvid -or $native.disable_optimizations) {
        throw "R2R/Release/MVID verification failed: $name"
    }
    $expectedFramework = if ($name -eq 'FSharp.Core.dll') { '.NETCoreApp,Version=v10.0' } else { '.NETCoreApp,Version=v11.0' }
    if ($native.framework -ne $expectedFramework) { throw "Incorrect target framework: $name" }
    [ordered]@{file=$name; il=$il; native=$native}
}
$payload = [ordered]@{
    vmr=$vmr; production_tree=$tree; verified_production_files=$paths.Count
    source_normalization='Git text normalization (Windows archive/checkout line endings)'
    archive_sha256=(Get-FileHash "$PerformanceRoot\fsharp-rc2-source.zip").Hash
    build_overlay_sha256=(Get-FileHash "$source\Directory.Build.props.user").Hash
    runtimeconfig=(Get-Content "$PerformanceRoot\rc2-r2r\fsc.runtimeconfig.json" -Raw | ConvertFrom-Json)
    images=@($images)
}
New-Item -ItemType Directory -Force "$PerformanceRoot\third-wave" | Out-Null
$payload | ConvertTo-Json -Depth 12 | Set-Content "$PerformanceRoot\third-wave\payload.json"
Write-Output "Verified $($paths.Count) unchanged production files, optimized IL, matching MVIDs and R2R images."
