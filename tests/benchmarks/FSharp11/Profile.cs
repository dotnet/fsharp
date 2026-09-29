using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Diagnostics.Tracing;

if (args.Length != 3)
    throw new ArgumentException("Usage: Profile <trace.nettrace> <SDK-FSharp-directory> <output.json>");
var sdk = Path.GetFullPath(args[1]);
var context = new AssemblyLoadContext("profiled-compiler", isCollectible: true);
context.Resolving += (loader, name) =>
{
    var file = Path.Combine(sdk, name.Name + ".dll");
    return File.Exists(file) ? loader.LoadFromAssemblyPath(file) : null;
};
var fcs = context.LoadFromAssemblyPath(Path.Combine(sdk, "FSharp.Compiler.Service.dll"));
var core = context.LoadFromAssemblyPath(Path.Combine(sdk, "FSharp.Core.dll"));
var assemblies = new[] { fcs, core }.ToDictionary(a => a.Location, StringComparer.OrdinalIgnoreCase);
using var source = new EventPipeEventSource(args[0]);
var allocations = new Dictionary<(ulong Id, string Name), (long Bytes, long Ticks)>();
var typeNames = new Dictionary<ulong, (string Name, ulong Module, int Token)>();
var modules = new Dictionary<ulong, string>();
var collections = new int[3];
long ticks = 0, namedTicks = 0;
source.Clr.LoaderModuleLoad += data => modules[unchecked((ulong)data.ModuleID)] = data.ModuleILPath;
source.Clr.TypeBulkType += data =>
{
    for (var i = 0; i < data.Count; i++)
    {
        var value = data.Values(i);
        typeNames[value.TypeID] = (value.TypeName, value.ModuleID, value.TypeNameID);
    }
};
source.Clr.GCAllocationTick += data =>
{
    var name = string.IsNullOrEmpty(data.TypeName) ? "<unnamed>" : data.TypeName;
    var key = (data.TypeID, name);
    allocations.TryGetValue(key, out var previous);
    allocations[key] = (previous.Bytes + data.AllocationAmount64, previous.Ticks + 1);
    ticks++;
    if (name != "<unnamed>") namedTicks++;
};
source.Clr.GCStart += data => collections[data.Depth]++;
source.Process();
if (source.EventsLost != 0 || ticks == 0 || namedTicks == 0)
    throw new InvalidOperationException($"Unusable profile: lost={source.EventsLost}, ticks={ticks}, named={namedTicks}");
var rows = allocations.OrderByDescending(p => p.Value.Bytes).Select(p =>
{
    var hasMetadata = typeNames.TryGetValue(p.Key.Id, out var typeInfo);
    var name = hasMetadata ? typeInfo.Name : p.Key.Name;
    modules.TryGetValue(typeInfo.Module, out var module);
    Type? definition = null;
    if (module != null && assemblies.TryGetValue(module, out var assembly) &&
        (typeInfo.Token & 0xff000000) == 0x02000000 && (typeInfo.Token & 0xffffff) != 0)
        definition = assembly.ManifestModule.ResolveType(typeInfo.Token);
    if (definition != null) name = definition.FullName!;
    var verified = false;
    if (definition?.Name.Contains('@') == true)
        for (var b = definition.BaseType; b != null; b = b.BaseType)
            if (b.FullName?.StartsWith("Microsoft.FSharp.Core.FSharpFunc", StringComparison.Ordinal) == true ||
                b.FullName?.StartsWith("Microsoft.FSharp.Core.OptimizedClosures+", StringComparison.Ordinal) == true)
                verified = true;
    var closure = verified ? new
    {
        assembly = definition!.Assembly.GetName().Name, base_type = definition.BaseType!.FullName,
        fields = definition.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Select(f => new { f.Name, type = f.FieldType.FullName }).ToArray()
    } : null;
    return new { type = name, type_id = $"0x{p.Key.Id:x}", metadata_token = $"0x{typeInfo.Token:x}",
        raw_type_name = p.Key.Name, module, weighted_bytes = p.Value.Bytes,
        allocation_ticks = p.Value.Ticks, has_type_metadata = hasMetadata,
        at_sign_candidate = name.Contains('@'), verified_fsharp_function = verified, closure };
}).ToArray();
File.WriteAllText(args[2], JsonSerializer.Serialize(new
{
    trace_sha256 = Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(args[0]))),
    parser = typeof(EventPipeEventSource).Assembly.GetName().Version!.ToString(),
    fcs_version = fcs.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion,
    events_lost = source.EventsLost, allocation_ticks = ticks, named_allocation_ticks = namedTicks,
    gc_starts_by_generation = collections, weighted_allocation_bytes = rows.Sum(r => r.weighted_bytes),
    verified_function_weighted_bytes = rows.Where(r => r.verified_fsharp_function).Sum(r => r.weighted_bytes),
    allocations = rows
}, new JsonSerializerOptions { WriteIndented = true }));
