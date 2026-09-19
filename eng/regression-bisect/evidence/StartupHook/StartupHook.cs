using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

// Startup hooks require this name in the global namespace. A runtime without startup
// hook support or a missing output is untestable, never evidence of a Core load.
public static class StartupHook
{
    private static readonly object Gate = new();
    private static readonly ConditionalWeakTable<Assembly, object> Seen = new();
    private static string? destination;

    public static void Initialize()
    {
        // The runner supplies a fresh path; JSONL preserves distinct Core load contexts.
        destination = Environment.GetEnvironmentVariable("REPRO_CORE_EVIDENCE");
        if (string.IsNullOrEmpty(destination))
            return;
        using var process = Process.GetCurrentProcess();
        destination = destination!.Replace("{pid}", process.Id.ToString(System.Globalization.CultureInfo.InvariantCulture));
        AppDomain.CurrentDomain.AssemblyLoad += (_, args) => Record(args.LoadedAssembly);
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            Record(assembly);
    }

    private static void Record(Assembly assembly)
    {
        if (assembly.GetName().Name != "FSharp.Core")
            return;
        lock (Gate)
        {
            if (Seen.TryGetValue(assembly, out _))
                return;
            Seen.Add(assembly, new object());
            try
            {
                string path = assembly.Location;
                var runtimeDirectory = Directory.GetParent(typeof(object).Assembly.Location);
                if (runtimeDirectory?.Parent?.Name != "Microsoft.NETCore.App")
                    throw new InvalidOperationException("The loaded CoreLib has no shared-framework version directory.");
                using var contents = File.OpenRead(path);
                using var hash = SHA256.Create();
                using var process = Process.GetCurrentProcess();
                File.AppendAllText(destination!, Serialize(new Dictionary<string, object?>
                {
                    ["schemaVersion"] = 1,
                    ["complete"] = true,
                    ["path"] = path,
                    ["sha256"] = BitConverter.ToString(hash.ComputeHash(contents)).Replace("-", "").ToLowerInvariant(),
                    ["assemblyName"] = assembly.GetName().Name,
                    ["assemblyVersion"] = assembly.GetName().Version?.ToString(),
                    ["fileVersion"] = FileVersionInfo.GetVersionInfo(path).FileVersion,
                    ["entryAssembly"] = Assembly.GetEntryAssembly()?.Location,
                    ["processId"] = process.Id,
                    ["architecture"] = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(),
                    ["runtimeVersion"] = runtimeDirectory.Name,
                    ["runtimePath"] = runtimeDirectory.FullName
                }) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Observation must not change application behavior. A missing/incomplete
                // report is rejected by the runner, including pathless single-file loads.
                try
                {
                    File.AppendAllText(destination!, Serialize(new Dictionary<string, object?>
                    {
                        ["schemaVersion"] = 1,
                        ["complete"] = false,
                        ["diagnostics"] = new[] { $"{ex.GetType().Name}: {ex.Message}" }
                    }) + Environment.NewLine);
                }
                catch (Exception writeError)
                {
                    var message = $"{writeError.GetType().Name}: {writeError.Message}".Replace('\r', ' ').Replace('\n', ' ');
                    Console.Error.WriteLine("REPRO_CORE_EVIDENCE_ERROR: " + message.Substring(0, Math.Min(512, message.Length)));
                }
            }
        }
    }

    private static string Serialize(Dictionary<string, object?> record)
    {
        var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, object>),
            new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true, KnownTypes = new[] { typeof(string[]) } });
        using var stream = new MemoryStream();
        serializer.WriteObject(stream, record);
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
