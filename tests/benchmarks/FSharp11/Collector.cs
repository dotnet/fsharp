using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text.Json;

internal static class Collector
{
    [StructLayout(LayoutKind.Sequential)]
    private struct MemoryCounters
    {
        public uint Size, PageFaultCount;
        public nuint PeakWorkingSet, WorkingSet, QuotaPeakPagedPool, QuotaPagedPool;
        public nuint QuotaPeakNonPagedPool, QuotaNonPagedPool, Pagefile, PeakPagefile, PrivateUsage;
    }

    [DllImport("psapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetProcessMemoryInfo(nint process, ref MemoryCounters counters, uint size);

    private readonly record struct Snapshot(long Allocated, long Timestamp, long UserTicks, long KernelTicks,
        int Gen0, int Gen1, int Gen2, ulong PeakWorkingSet, ulong WorkingSet, ulong PeakCommit, ulong Commit);

    private static Snapshot TakeSnapshot(Process process)
    {
        process.Refresh();
        var memory = new MemoryCounters { Size = (uint)Marshal.SizeOf<MemoryCounters>() };
        if (!GetProcessMemoryInfo(process.Handle, ref memory, memory.Size))
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        var user = process.UserProcessorTime.Ticks;
        var kernel = process.PrivilegedProcessorTime.Ticks;
        return new(GC.GetTotalAllocatedBytes(true), Stopwatch.GetTimestamp(), user, kernel,
            GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2),
            memory.PeakWorkingSet, memory.WorkingSet, memory.PeakPagefile, memory.PrivateUsage);
    }

    private static object Call(MethodInfo method, object? target, params object?[] supplied)
    {
        var args = new object?[method.GetParameters().Length];
        supplied.CopyTo(args, 0);
        return method.Invoke(target, args) ?? throw new InvalidOperationException($"{method.Name} returned null.");
    }

    private static void FullGc()
    {
        GC.Collect(2, GCCollectionMode.Forced, true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, true);
    }

    public static int Main(string[] args)
    {
        if (args.Length != 4)
            throw new ArgumentException("Usage: Collector <sdk-FSharp-directory> <case.json> <result.json> <compile|check|calibrate>");

        var sdk = Path.GetFullPath(args[0]);
        using var input = JsonDocument.Parse(File.ReadAllText(args[1]));
        var root = input.RootElement;
        Directory.SetCurrentDirectory(root.GetProperty("working_directory").GetString()!);
        AssemblyLoadContext.Default.Resolving += (context, name) =>
        {
            var path = Path.Combine(sdk, name.Name + ".dll");
            return File.Exists(path) ? context.LoadFromAssemblyPath(path) : null;
        };
        var fcs = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(sdk, "FSharp.Compiler.Service.dll"));
        var core = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(sdk, "FSharp.Core.dll"));
        var checkerType = fcs.GetType("FSharp.Compiler.CodeAnalysis.FSharpChecker", true)!;
        var create = checkerType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static)!;
        var asyncModule = core.GetType("Microsoft.FSharp.Control.FSharpAsync", true)!;
        var runAsync = asyncModule.GetMethods().Single(m => m.Name == "RunSynchronously");
        object Run(object computation) =>
            Call(runAsync.MakeGenericMethod(computation.GetType().GetGenericArguments()[0]), null, computation);

        var argv = root.GetProperty("arguments").EnumerateArray().Select(x => x.GetString()!).ToArray();
        using var process = Process.GetCurrentProcess();
        _ = TakeSnapshot(process);
        var record = new Dictionary<string, object?>
        {
            ["schema_version"] = 1, ["started_utc"] = DateTimeOffset.UtcNow,
            ["case"] = root.GetProperty("id").GetString(), ["scope"] = args[3],
            ["runtime"] = RuntimeInformation.FrameworkDescription, ["pid"] = Environment.ProcessId,
            ["fcs_version"] = fcs.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion,
            ["core_version"] = core.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion,
            ["gc_server"] = GCSettings.IsServerGC, ["gc_configuration"] = GC.GetConfigurationVariables(),
            ["visible_processors"] = Environment.ProcessorCount,
            ["input_sha256"] = Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(args[1]))),
        };
        object? checker = null;
        var results = new List<object>();
        var errorCount = 0;
        var exitCode = 0;
        var start = TakeSnapshot(process);
        if (args[3] == "calibrate")
        {
            Parallel.For(0, 4, _ =>
            {
                for (var i = 0; i < 1000; ++i)
                    GC.KeepAlive(new byte[1024]);
            });
            var buffer = new byte[128 * 1024 * 1024];
            for (var i = 0; i < buffer.Length; i += 4096) buffer[i] = 1;
            GC.KeepAlive(buffer);
            record["minimum_payload_bytes"] = 128L * 1024 * 1024 + 4000L * 1024;
        }
        else if (args[3] == "compile")
        {
            checker = Call(create, null);
            var compile = checkerType.GetMethods().Single(m => m.Name == "Compile" && m.GetParameters()[0].ParameterType == typeof(string[]));
            var result = Run(Call(compile, checker, (object)argv));
            results.Add(result);
            var tuple = (ITuple)result;
            if (tuple[1] is int numericExitCode)
                exitCode = numericExitCode;
            else if (tuple[1] is { } exceptionOption)
            {
                var exception = exceptionOption.GetType().GetProperty("Value")?.GetValue(exceptionOption) as Exception
                    ?? throw new InvalidOperationException($"Unexpected Compile result: {exceptionOption.GetType()}.");
                Console.Error.WriteLine(exception);
                exitCode = 1;
            }
            var diagnostics = (IEnumerable)tuple[0]!;
            foreach (var diagnostic in diagnostics)
                if (diagnostic!.GetType().GetProperty("Severity")!.GetValue(diagnostic)!.ToString() == "Error")
                    errorCount++;
        }
        else if (args[3] == "check")
        {
            var createArgs = new object?[create.GetParameters().Length];
            var cacheIndex = Array.FindIndex(create.GetParameters(), p => p.Name == "projectCacheSize");
            var optionInt = core.GetType("Microsoft.FSharp.Core.FSharpOption`1", true)!.MakeGenericType(typeof(int));
            createArgs[cacheIndex] = Activator.CreateInstance(optionInt, 200);
            checker = create.Invoke(null, createArgs)!;
            var getOptions = checkerType.GetMethod("GetProjectOptionsFromCommandLineArgs")!;
            var parseCheck = checkerType.GetMethods().Single(m => m.Name == "ParseAndCheckProject"
                && m.GetParameters()[0].ParameterType.Name == "FSharpProjectOptions");
            var options = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var project in root.GetProperty("projects").EnumerateArray())
            {
                var path = project.GetProperty("path").GetString()!;
                var projectArgs = project.GetProperty("arguments").EnumerateArray().Select(x => x.GetString()!).ToArray();
                var opts = Call(getOptions, checker, path, projectArgs);
                var refsProperty = opts.GetType().GetProperty("ReferencedProjects")!;
                var refType = refsProperty.PropertyType.GetElementType()!;
                var references = project.GetProperty("references").EnumerateArray().ToArray();
                var refs = Array.CreateInstance(refType, references.Length);
                for (var i = 0; i < references.Length; ++i)
                {
                    var reference = references[i];
                    refs.SetValue(Call(refType.GetMethod("NewFSharpReference")!, null,
                        reference.GetProperty("output").GetString()!, options[reference.GetProperty("path").GetString()!]), i);
                }
                var ctor = opts.GetType().GetConstructors().Single();
                var ctorArgs = ctor.GetParameters().Select(p =>
                    opts.GetType().GetProperty(p.Name!, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)!.GetValue(opts)).ToArray();
                ctorArgs[Array.FindIndex(ctor.GetParameters(), p => p.Name!.Equals("referencedProjects", StringComparison.OrdinalIgnoreCase))] = refs;
                opts = ctor.Invoke(ctorArgs);
                options.Add(path, opts);
                var result = Run(Call(parseCheck, checker, opts));
                results.Add(result);
                foreach (var diagnostic in (IEnumerable)result.GetType().GetProperty("Diagnostics")!.GetValue(result)!)
                    if (diagnostic!.GetType().GetProperty("Severity")!.GetValue(diagnostic)!.ToString() == "Error")
                        errorCount++;
            }
            record["project_count"] = results.Count;
            record["graph"] = options;
        }
        else throw new ArgumentException("Unknown operation: " + args[3]);

        var end = TakeSnapshot(process);
        record["allocated_bytes"] = end.Allocated - start.Allocated;
        record["wall_ns"] = (end.Timestamp - start.Timestamp) * (1e9 / Stopwatch.Frequency);
        record["cpu_user_ns"] = (end.UserTicks - start.UserTicks) * 100;
        record["cpu_kernel_ns"] = (end.KernelTicks - start.KernelTicks) * 100;
        record["gen0"] = end.Gen0 - start.Gen0;
        record["gen1"] = end.Gen1 - start.Gen1;
        record["gen2"] = end.Gen2 - start.Gen2;
        record["peak_working_set_bytes"] = end.PeakWorkingSet;
        record["peak_private_commit_bytes"] = end.PeakCommit;
        record["working_set_end_bytes"] = end.WorkingSet;
        record["private_commit_end_bytes"] = end.Commit;
        record["working_set_start_bytes"] = start.WorkingSet;
        record["exit_code"] = exitCode;
        record["error_count"] = errorCount;
        record["status"] = exitCode == 0 && errorCount == 0 ? "accepted" : "failed";
        if (args[3] is "check" or "calibrate")
        {
            Thread.Sleep(10000);
            var idle = TakeSnapshot(process);
            record["idle_10s_working_set_bytes"] = idle.WorkingSet;
            record["idle_10s_private_commit_bytes"] = idle.Commit;
            FullGc();
            record["retained_heap_bytes"] = GC.GetTotalMemory(false);
            record["post_gc_working_set_bytes"] = TakeSnapshot(process).WorkingSet;
        }
        if (errorCount > 0)
            foreach (var result in results)
            {
                var diagnostics = result.GetType().GetProperty("Diagnostics")?.GetValue(result) ?? ((ITuple)result)[0]!;
                foreach (var diagnostic in (IEnumerable)diagnostics)
                    Console.Error.WriteLine(diagnostic);
            }
        GC.KeepAlive(checker);
        GC.KeepAlive(results);
        record.Remove("graph");
        File.WriteAllText(args[2], JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true }));
        return exitCode != 0 ? exitCode : errorCount > 0 ? 1 : 0;
    }
}
