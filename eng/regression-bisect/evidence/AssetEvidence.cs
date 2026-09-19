using System.Diagnostics;
using System.IO.Compression;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace ReproEvidence;

internal static class AssetEvidence
{
    internal static string Hash(Stream stream) => Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();

    internal static object Inspect(string file)
    {
        string path = Path.GetFullPath(file);
        using var stream = File.OpenRead(path);
        string sha256 = Hash(stream);
        stream.Position = 0;
        if (Path.GetExtension(path).Equals(".nupkg", StringComparison.OrdinalIgnoreCase))
        {
            using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
            var manifest = zip.Entries.Single(e => !e.FullName.Contains('/') && e.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));
            using var manifestStream = manifest.Open();
            var metadata = XDocument.Load(manifestStream).Root!.Elements().Single(e => e.Name.LocalName == "metadata");
            string Field(string name) => metadata.Elements().Single(e => e.Name.LocalName == name).Value;
            var entries = zip.Entries.Where(e => !e.FullName.EndsWith('/')).Select(e =>
            {
                using var contents = e.Open();
                return new { path = e.FullName, sha256 = Hash(contents) };
            }).ToArray();
            return new { path, sha256, id = Field("id"), version = Field("version"), entries };
        }

        string? assemblyName = null, assemblyVersion = null;
        var resources = new List<object>();
        // Imports and runtime configuration files need byte identity too, but are not PE files.
        bool isPe = stream.ReadByte() == 'M' && stream.ReadByte() == 'Z';
        stream.Position = 0;
        using var pe = isPe ? new PEReader(stream) : null;
        if (pe?.HasMetadata == true)
        {
            var reader = pe.GetMetadataReader();
            if (reader.IsAssembly)
            {
                var definition = reader.GetAssemblyDefinition();
                assemblyName = reader.GetString(definition.Name);
                assemblyVersion = definition.Version.ToString();
            }
            foreach (var handle in reader.ManifestResources)
            {
                var resource = reader.GetManifestResource(handle);
                string name = reader.GetString(resource.Name);
                if (!resource.Implementation.IsNil)
                {
                    resources.Add(new { name, sha256 = (string?)null, embedded = false });
                    continue;
                }
                var block = pe.GetSectionData(pe.PEHeaders.CorHeader!.ResourcesDirectory.RelativeVirtualAddress);
                var content = block.GetReader(checked((int)resource.Offset), block.Length - checked((int)resource.Offset));
                int length = content.ReadInt32();
                string hash = Convert.ToHexString(SHA256.HashData(content.ReadBytes(length))).ToLowerInvariant();
                resources.Add(new { name, sha256 = hash, embedded = true });
            }
        }
        return new
        {
            path,
            sha256,
            assemblyName,
            assemblyVersion,
            fileVersion = FileVersionInfo.GetVersionInfo(path).FileVersion,
            resources
        };
    }
}
