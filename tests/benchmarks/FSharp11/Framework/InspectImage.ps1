param([Parameter(Mandatory)][string] $Path)

$ErrorActionPreference = 'Stop'
function Hash-Bytes([byte[]] $Bytes) {
    [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($Bytes))
}
$stream = [IO.File]::OpenRead($Path)
$pe = [Reflection.PortableExecutable.PEReader]::new($stream)
try {
    $metadata = [Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($pe)
    $methods = @(
        foreach ($handle in $metadata.MethodDefinitions) {
            $method = $metadata.GetMethodDefinition($handle)
            if ($method.RelativeVirtualAddress -eq 0) { continue }
            $body = [Reflection.Metadata.PEReaderExtensions]::GetMethodBody($pe, $method.RelativeVirtualAddress)
            [ordered]@{
                name = $metadata.GetString($method.Name)
                body_sha256 = Hash-Bytes ([byte[]]$pe.GetSectionData($method.RelativeVirtualAddress).GetContent(0, $body.Size))
            }
        }
    )
    $resources = @(
        foreach ($handle in $metadata.ManifestResources) {
            $resource = $metadata.GetManifestResource($handle)
            if (-not $resource.Implementation.IsNil) { throw 'External resource not supported.' }
            $name = $metadata.GetString($resource.Name)
            $rva = [int]($pe.PEHeaders.CorHeader.ResourcesDirectory.RelativeVirtualAddress + $resource.Offset)
            $reader = $pe.GetSectionData($rva).GetReader()
            $bytes = $reader.ReadBytes($reader.ReadInt32())
            $rawHash = Hash-Bytes $bytes
            if ($name -like '*Compressed*') {
                $inputStream = [IO.MemoryStream]::new($bytes, $false)
                $outputStream = [IO.MemoryStream]::new()
                $deflate = [IO.Compression.DeflateStream]::new($inputStream, [IO.Compression.CompressionMode]::Decompress)
                try { $deflate.CopyTo($outputStream); $bytes = $outputStream.ToArray() }
                finally { $deflate.Dispose(); $inputStream.Dispose(); $outputStream.Dispose() }
            }
            [ordered]@{ name = $name; stored_sha256 = $rawHash; content_sha256 = Hash-Bytes $bytes }
        }
    )
    [ordered]@{
        path = [IO.Path]::GetFullPath($Path)
        sha256 = (Get-FileHash $Path).Hash
        mvid = $metadata.GetGuid($metadata.GetModuleDefinition().Mvid).ToString()
        methods = $methods
        resources = $resources
        debug = @($pe.ReadDebugDirectory() | ForEach-Object {
            [ordered]@{type=$_.Type.ToString();size=$_.DataSize;stamp=$_.Stamp}
        })
    } | ConvertTo-Json -Depth 6
} finally { $pe.Dispose(); $stream.Dispose() }
