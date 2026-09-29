using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;

internal static class Collector
{
    [StructLayout(LayoutKind.Sequential)]
    private struct MemoryCounters
    {
        public uint Size, PageFaultCount;
        public UIntPtr PeakWorkingSet, WorkingSet, QuotaPeakPagedPool, QuotaPagedPool;
        public UIntPtr QuotaPeakNonPagedPool, QuotaNonPagedPool, Pagefile, PeakPagefile, PrivateUsage;
    }

    [DllImport("psapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetProcessMemoryInfo(IntPtr process, ref MemoryCounters counters, uint size);

    private static long Allocated()
    {
#if NETFRAMEWORK
        return AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize;
#else
        return GC.GetTotalAllocatedBytes(true);
#endif
    }

    private static (long allocated, long time, long user, long kernel, int g0, int g1, int g2,
        ulong peakRam, ulong ram, ulong peakCommit, ulong commit) Snapshot(Process process, bool monitor)
    {
        process.Refresh();
        var counters = new MemoryCounters { Size = (uint)Marshal.SizeOf<MemoryCounters>() };
        if (!GetProcessMemoryInfo(process.Handle, ref counters, counters.Size))
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        return (monitor ? Allocated() : 0, Stopwatch.GetTimestamp(), process.UserProcessorTime.Ticks,
            process.PrivilegedProcessorTime.Ticks, GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2),
            counters.PeakWorkingSet.ToUInt64(), counters.WorkingSet.ToUInt64(),
            counters.PeakPagefile.ToUInt64(), counters.PrivateUsage.ToUInt64());
    }

    private static object Call(MethodInfo method, object? target, params object?[] supplied)
    {
        var arguments = new object?[method.GetParameters().Length];
        supplied.CopyTo(arguments, 0);
        return method.Invoke(target, arguments) ?? throw new InvalidOperationException($"{method.Name} returned null.");
    }

    private static void FullGc()
    {
        GC.Collect(2, GCCollectionMode.Forced, true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, true);
    }

    public static int Main(string[] args)
    {
        if (args.Length < 4 || args.Length > 5)
            throw new ArgumentException("Collector <payload> <case.json> <result.json> <compile|check|calibrate> [no-allocation]");
        if (!Environment.Is64BitProcess) throw new InvalidOperationException("This experiment requires x64.");
        var monitor = args.Length == 4;
        if (!monitor && args[4] != "no-allocation") throw new ArgumentException("Unknown allocation mode.");
#if NETFRAMEWORK
        if (monitor) AppDomain.MonitoringIsEnabled = true;
#endif
        var payload = Path.GetFullPath(args[0]);
        AppDomain.CurrentDomain.AssemblyResolve += (_, request) =>
        {
            var file = Path.Combine(payload, new AssemblyName(request.Name).Name + ".dll");
            return File.Exists(file) ? Assembly.LoadFrom(file) : null;
        };
        using var input = JsonDocument.Parse(File.ReadAllText(args[1]));
        var root = input.RootElement;
        Directory.SetCurrentDirectory(root.GetProperty("working_directory").GetString()!);
        var core = Assembly.LoadFrom(Path.Combine(payload, "FSharp.Core.dll"));
        var fcs = Assembly.LoadFrom(Path.Combine(payload, "FSharp.Compiler.Service.dll"));
        var checkerType = fcs.GetType("FSharp.Compiler.CodeAnalysis.FSharpChecker", true)!;
        var create = checkerType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static)!;
        var runAsync = core.GetType("Microsoft.FSharp.Control.FSharpAsync", true)!.GetMethods()
            .Single(method => method.Name == "RunSynchronously");
        object Run(object computation) =>
            Call(runAsync.MakeGenericMethod(computation.GetType().GetGenericArguments()[0]), null, computation);
        using var process = Process.GetCurrentProcess();
        using var sha = SHA256.Create();
        _ = Snapshot(process, monitor);
        var record = new Dictionary<string, object?>
        {
            ["schema_version"] = 1, ["started_utc"] = DateTimeOffset.UtcNow,
            ["case"] = root.GetProperty("id").GetString(), ["scope"] = args[3],
            ["runtime"] = RuntimeInformation.FrameworkDescription, ["pid"] = process.Id,
            ["fcs_version"] = fcs.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion,
            ["core_version"] = core.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion,
            ["fcs_path"] = fcs.Location, ["core_path"] = core.Location,
            ["fcs_target_framework"] = fcs.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>()!.FrameworkName,
            ["core_target_framework"] = core.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>()!.FrameworkName,
            ["appdomain_id"] = AppDomain.CurrentDomain.Id,
            ["gc_server"] = GCSettings.IsServerGC, ["visible_processors"] = Environment.ProcessorCount,
            ["allocation_monitoring"] = monitor,
            ["input_sha256"] = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(args[1]))).Replace("-", "").ToLowerInvariant(),
#if NETFRAMEWORK
            ["allocation_api"] = "AppDomain.MonitoringTotalAllocatedMemorySize (current domain, all threads)",
            ["gc_configuration"] = new { ServerGC = GCSettings.IsServerGC, LatencyMode = GCSettings.LatencyMode.ToString(), DATAS = "not-supported" },
            ["runtime_family"] = "framework",
#else
            ["allocation_api"] = "GC.GetTotalAllocatedBytes(precise: true)",
            ["gc_configuration"] = GC.GetConfigurationVariables(), ["runtime_family"] = "core",
#endif
        };
        object? checker = null;
        var results = new List<object>();
        var errors = new List<object>();
        var options = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        var exitCode = 0;
        var start = Snapshot(process, monitor);
        if (args[3] == "calibrate")
        {
            if (!monitor) throw new ArgumentException("Calibration requires allocation monitoring.");
            var before = Allocated();
            var threads = Enumerable.Range(0, 4).Select(_ => new Thread(() =>
            {
                for (var i = 0; i < 10000; i++) GC.KeepAlive(new byte[1024]);
            })).ToArray();
            foreach (var thread in threads) thread.Start();
            foreach (var thread in threads) thread.Join();
            record["thread_payload_bytes"] = 40000L * 1024;
            record["threads_before_gc_bytes"] = Allocated() - before;
            FullGc();
            record["threads_after_gc_bytes"] = Allocated() - before;
            var buffer = new byte[128 * 1024 * 1024];
            for (var i = 0; i < buffer.Length; i += 4096) buffer[i] = 1;
            GC.KeepAlive(buffer);
        }
        else if (args[3] == "compile")
        {
            checker = Call(create, null);
            var argv = root.GetProperty("arguments").EnumerateArray().Select(x => x.GetString()!).ToArray();
            var compile = checkerType.GetMethods().Single(m => m.Name == "Compile" && m.GetParameters()[0].ParameterType == typeof(string[]));
            var result = Run(Call(compile, checker, (object)argv));
            results.Add(result);
            var code = result.GetType().GetProperty("Item2")!.GetValue(result);
            if (code is int numeric) exitCode = numeric;
            else if (code != null)
            {
                Console.Error.WriteLine(code.GetType().GetProperty("Value")!.GetValue(code));
                exitCode = 1;
            }
            foreach (var diagnostic in (IEnumerable)result.GetType().GetProperty("Item1")!.GetValue(result)!)
                if (diagnostic.GetType().GetProperty("Severity")!.GetValue(diagnostic)!.ToString() == "Error")
                    errors.Add(diagnostic);
        }
        else if (args[3] == "check")
        {
            var createArgs = new object?[create.GetParameters().Length];
            var optionInt = core.GetType("Microsoft.FSharp.Core.FSharpOption`1", true)!.MakeGenericType(typeof(int));
            createArgs[Array.FindIndex(create.GetParameters(), p => p.Name == "projectCacheSize")] = Activator.CreateInstance(optionInt, 200);
            checker = create.Invoke(null, createArgs)!;
            var getOptions = checkerType.GetMethod("GetProjectOptionsFromCommandLineArgs")!;
            var check = checkerType.GetMethods().Single(m => m.Name == "ParseAndCheckProject" &&
                m.GetParameters()[0].ParameterType.Name == "FSharpProjectOptions");
            foreach (var project in root.GetProperty("projects").EnumerateArray())
            {
                var projectPath = project.GetProperty("path").GetString()!;
                var arguments = project.GetProperty("arguments").EnumerateArray().Select(x => x.GetString()!).ToArray();
                var opts = Call(getOptions, checker, projectPath, arguments);
                var refType = opts.GetType().GetProperty("ReferencedProjects")!.PropertyType.GetElementType()!;
                var references = project.GetProperty("references").EnumerateArray().ToArray();
                var refs = Array.CreateInstance(refType, references.Length);
                for (var i = 0; i < references.Length; i++)
                    refs.SetValue(Call(refType.GetMethod("NewFSharpReference")!, null,
                        references[i].GetProperty("output").GetString()!, options[references[i].GetProperty("path").GetString()!]), i);
                var ctor = opts.GetType().GetConstructors().Single();
                var ctorArgs = ctor.GetParameters().Select(p =>
                    opts.GetType().GetProperty(p.Name!, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)!.GetValue(opts)).ToArray();
                ctorArgs[Array.FindIndex(ctor.GetParameters(), p => p.Name!.Equals("referencedProjects", StringComparison.OrdinalIgnoreCase))] = refs;
                opts = ctor.Invoke(ctorArgs);
                options.Add(projectPath, opts);
                var result = Run(Call(check, checker, opts));
                results.Add(result);
                foreach (var diagnostic in (IEnumerable)result.GetType().GetProperty("Diagnostics")!.GetValue(result)!)
                    if (diagnostic.GetType().GetProperty("Severity")!.GetValue(diagnostic)!.ToString() == "Error")
                        errors.Add(diagnostic);
            }
            record["project_count"] = results.Count;
        }
        else throw new ArgumentException("Unknown operation.");
        var end = Snapshot(process, monitor);
        record["allocated_bytes"] = monitor ? (object)(end.allocated - start.allocated) : null;
        record["wall_ns"] = (end.time - start.time) * (1e9 / Stopwatch.Frequency);
        record["cpu_user_ns"] = (end.user - start.user) * 100;
        record["cpu_kernel_ns"] = (end.kernel - start.kernel) * 100;
        record["gen0"] = end.g0 - start.g0; record["gen1"] = end.g1 - start.g1; record["gen2"] = end.g2 - start.g2;
        record["peak_working_set_bytes"] = end.peakRam; record["peak_private_commit_bytes"] = end.peakCommit;
        record["working_set_end_bytes"] = end.ram; record["private_commit_end_bytes"] = end.commit;
        record["exit_code"] = exitCode; record["error_count"] = errors.Count;
        record["status"] = exitCode == 0 && errors.Count == 0 ? "accepted" : "failed";
        if (args[3] is "check" or "calibrate")
        {
            Thread.Sleep(10000);
            record["idle_10s_working_set_bytes"] = Snapshot(process, monitor).ram;
            FullGc();
            record["retained_heap_bytes"] = GC.GetTotalMemory(false);
            record["post_gc_working_set_bytes"] = Snapshot(process, monitor).ram;
        }
        foreach (var error in errors) Console.Error.WriteLine(error);
        GC.KeepAlive(checker); GC.KeepAlive(results); GC.KeepAlive(options);
        record["loaded_assemblies"] = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic).Select(a => new { name = a.GetName().Name, path = a.Location }).ToArray();
        record["fsharp_modules"] = process.Modules.Cast<ProcessModule>()
            .Where(m => m.ModuleName.StartsWith("FSharp.", StringComparison.OrdinalIgnoreCase))
            .Select(m => new { name = m.ModuleName, path = m.FileName }).ToArray();
        File.WriteAllText(args[2], JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true }));
        return exitCode != 0 ? exitCode : errors.Count > 0 ? 1 : 0;
    }
}
