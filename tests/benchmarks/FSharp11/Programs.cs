using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Horology;
using Perfolizer.Mathematics.OutlierDetection;

[MemoryDiagnoser]
public class Programs
{
    [Params(0, 1, 4, 16, 64, 1024)]
    public int Length { get; set; }

    [Params("Cart", "Rules", "Telemetry", "Option", "Nested", "FilterMap", "Escaping", "NonCapturing")]
    public string Kernel { get; set; } = "";

    private static Assembly workload = null!;
    private Func<int, int> invoke = null!;
    private int state;

    [GlobalSetup]
    public void Setup()
    {
        invoke = (Func<int, int>)workload.GetType("FSharp11Kernels", true)!.GetMethod("Create")!
            .Invoke(null, [Length, Kernel])!;
        foreach (var sample in new[] { -1, 0, 1, 7, 31, 63, 127, 1023, int.MaxValue })
            if (invoke(sample) != Expected(Length, Kernel, sample))
                throw new InvalidOperationException($"Semantic mismatch: {Kernel}, length {Length}, state {sample}");
    }

    [Benchmark]
    public int Run() => invoke(unchecked(++state));

    private static int Expected(int length, string kernel, int state)
    {
        var values = Enumerable.Range(0, length).Select(i => i * 17 % 97 + 1).ToArray();
        var offset = state & 31;
        return kernel switch
        {
            "Cart" => values.Select((v, i) => v * (1 + i % 5) * (100 - offset) / 100).Sum(),
            "Rules" => (values.Any(v => v > (state & 127)) ? 1 : 0) + (values.All(v => v < (state & 127) + 50) ? 2 : 0),
            "Telemetry" => values.Select((v, i) => v * (1 + 1 + i % 7) * (offset + 1)).Sum(),
            "Option" => length == 0 ? 0 : values[0] + offset,
            "Nested" => values.Sum() * (offset + 1),
            "FilterMap" => values.Where(v => v > (state & 63)).Sum(v => v + offset),
            "Escaping" => length + offset,
            "NonCapturing" => values.Sum() + offset,
            _ => throw new ArgumentException(kernel)
        };
    }

    private static void Inspect(string output)
    {
        var opcodes = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(OpCode)).Select(f => (OpCode)f.GetValue(null)!)
            .ToDictionary(op => unchecked((ushort)op.Value));
        bool IsFunction(Type type)
        {
            for (var t = type.BaseType; t != null; t = t.BaseType)
                if (t.FullName?.StartsWith("Microsoft.FSharp.Core.FSharpFunc", StringComparison.Ordinal) == true ||
                    t.FullName?.StartsWith("Microsoft.FSharp.Core.OptimizedClosures+", StringComparison.Ordinal) == true)
                    return true;
            return false;
        }
        var sites = new List<object>();
        foreach (var type in workload.GetTypes())
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            var bytes = method.GetMethodBody()?.GetILAsByteArray();
            if (bytes == null) continue;
            for (var offset = 0; offset < bytes.Length;)
            {
                var at = offset;
                ushort code = bytes[offset++];
                if (code == 0xfe) code = (ushort)(0xfe00 | bytes[offset++]);
                var op = opcodes[code];
                if (op == OpCodes.Newobj)
                {
                    var constructor = method.Module.ResolveMethod(BitConverter.ToInt32(bytes, offset),
                        type.GetGenericArguments(), method.GetGenericArguments())!;
                    sites.Add(new { method = $"{type.FullName}.{method.Name}", il_offset = at,
                        allocated_type = constructor.DeclaringType!.FullName,
                        fsharp_function = IsFunction(constructor.DeclaringType) });
                }
                offset += op.OperandType switch
                {
                    OperandType.InlineNone => 0,
                    OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => 1,
                    OperandType.InlineVar => 2,
                    OperandType.InlineI8 or OperandType.InlineR => 8,
                    OperandType.InlineSwitch => 4 + 4 * BitConverter.ToInt32(bytes, offset),
                    _ => 4
                };
            }
        }
        var types = workload.GetTypes().Where(t => t.Name.Contains('@')).Select(t => new
        {
            name = t.FullName, fsharp_function = IsFunction(t), base_type = t.BaseType?.FullName,
            captured_fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(f => new { f.Name, type = f.FieldType.FullName })
        });
        File.WriteAllText(output, JsonSerializer.Serialize(new { types, newobj_sites = sites }, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static int Main(string[] args)
    {
        var dll = Path.GetFullPath(Environment.GetEnvironmentVariable("FSHARP_WORKLOAD_DLL")
            ?? throw new ArgumentException("FSHARP_WORKLOAD_DLL is required"));
        var corePath = Path.GetFullPath(Environment.GetEnvironmentVariable("FSHARP_WORKLOAD_CORE")
            ?? throw new ArgumentException("FSHARP_WORKLOAD_CORE is required"));
        AssemblyLoadContext.Default.LoadFromAssemblyPath(corePath);
        workload = AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);
        var output = Path.GetFullPath(Environment.GetEnvironmentVariable("FSHARP_PROGRAM_RESULTS")
            ?? throw new ArgumentException("FSHARP_PROGRAM_RESULTS is required"));
        Directory.CreateDirectory(output);
        File.WriteAllText(Path.Combine(output, "provenance.json"), JsonSerializer.Serialize(new
        {
            started_utc = DateTimeOffset.UtcNow, process_id = Environment.ProcessId,
            runtime = RuntimeInformation.FrameworkDescription,
            workload_sha256 = Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(dll))),
            core_sha256 = Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(corePath))),
            core_version = AssemblyLoadContext.Default.Assemblies.Single(a => a.GetName().Name == "FSharp.Core")
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion,
            driver_sha256 = Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(typeof(Programs).Assembly.Location))),
            benchmarkdotnet = typeof(BenchmarkAttribute).Assembly.GetName().Version!.ToString()
        }, new JsonSerializerOptions { WriteIndented = true }));
        Inspect(Path.Combine(output, "il-closures.json"));
        foreach (var kernel in new[] { "Cart", "Rules", "Telemetry", "Option", "Nested", "FilterMap", "Escaping", "NonCapturing" })
        foreach (var length in new[] { 0, 1, 4, 16, 64, 1024 })
            new Programs { Kernel = kernel, Length = length }.Setup();
        if (args.Contains("--validate")) return 0;
        var config = DefaultConfig.Instance
            .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance)
                .WithWarmupCount(6).WithIterationCount(15).WithIterationTime(TimeInterval.FromMilliseconds(250))
                .WithOutlierMode(OutlierMode.DontRemove)
                .WithUnrollFactor(1).WithId("FixedRuntime"))
            .AddExporter(JsonExporter.Full)
            .WithArtifactsPath(output)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);
        var summaries = BenchmarkSwitcher.FromTypes([typeof(Programs)]).Run(args, config).ToArray();
        return summaries.Length > 0 && summaries.All(s => s.Reports.All(r => r.Success)) ? 0 : 1;
    }
}
