using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
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
            .Invoke(null, new object[] { Length, Kernel })!;
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

    private static string Hash(string path)
    {
        using var sha = SHA256.Create();
        return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
    }

    public static int Main(string[] args)
    {
        if (!Environment.Is64BitProcess) throw new InvalidOperationException("Expected x64.");
        var dll = Environment.GetEnvironmentVariable("FSHARP_WORKLOAD_DLL") ?? throw new ArgumentException("Missing workload.");
        var core = Environment.GetEnvironmentVariable("FSHARP_WORKLOAD_CORE") ?? throw new ArgumentException("Missing Core.");
        var output = Environment.GetEnvironmentVariable("FSHARP_PROGRAM_RESULTS") ?? throw new ArgumentException("Missing output.");
        Assembly.LoadFrom(core);
        workload = Assembly.LoadFrom(dll);
        Directory.CreateDirectory(output);
        File.WriteAllText(Path.Combine(output, "provenance.json"), JsonSerializer.Serialize(new
        {
            started_utc = DateTimeOffset.UtcNow, runtime = RuntimeInformation.FrameworkDescription,
            gc_server = GCSettings.IsServerGC, workload_sha256 = Hash(dll), core_sha256 = Hash(core),
            driver_sha256 = Hash(typeof(Programs).Assembly.Location),
            benchmarkdotnet = typeof(BenchmarkAttribute).Assembly.GetName().Version!.ToString()
        }, new JsonSerializerOptions { WriteIndented = true }));
        foreach (var kernel in new[] { "Cart", "Rules", "Telemetry", "Option", "Nested", "FilterMap", "Escaping", "NonCapturing" })
        foreach (var length in new[] { 0, 1, 4, 16, 64, 1024 })
            new Programs { Kernel = kernel, Length = length }.Setup();
        if (args.Contains("--validate")) return 0;
        var config = DefaultConfig.Instance
            .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance)
                .WithWarmupCount(6).WithIterationCount(15).WithIterationTime(TimeInterval.FromMilliseconds(250))
                .WithOutlierMode(OutlierMode.DontRemove).WithUnrollFactor(1).WithId("PortablePayload"))
            .AddExporter(JsonExporter.Full).WithArtifactsPath(output)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);
        var summaries = BenchmarkSwitcher.FromTypes(new[] { typeof(Programs) }).Run(args, config).ToArray();
        return summaries.Length > 0 && summaries.All(s => s.Reports.All(r => r.Success)) ? 0 : 1;
    }
}
