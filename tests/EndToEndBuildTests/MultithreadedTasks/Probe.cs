using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

// This unmarked control must use a TaskHost in MT builds. It verifies that routing diagnostics are enabled.
public sealed class SerialControl : Task
{
    public override bool Execute() => true;
}

[MSBuildMultiThreadableTask]
public sealed class ProjectEnvironment : Task, IMultiThreadableTask
{
    public TaskEnvironment TaskEnvironment { get; set; } = TaskEnvironment.Fallback;
    [Required] public string Marker { get; set; }
    public string LoadedTasks { get; set; }
    public override bool Execute()
    {
        var environment = typeof(ToolTask).GetProperty("TaskEnvironment");
        if (environment?.PropertyType != typeof(TaskEnvironment) || !typeof(IMultiThreadableTask).IsAssignableFrom(typeof(ToolTask)))
            throw new InvalidOperationException("This MSBuild host does not expose the required ToolTask.TaskEnvironment API");
        if (!string.IsNullOrEmpty(LoadedTasks))
        {
            var framework = typeof(IMultiThreadableTask).Assembly.Location;
            var utilities = typeof(ToolTask).Assembly.Location;
            File.WriteAllText(TaskEnvironment.GetAbsolutePath("host.txt").ToString(),
                $"{framework}\t{FileVersionInfo.GetVersionInfo(framework).FileVersion}\t{utilities}\t{FileVersionInfo.GetVersionInfo(utilities).FileVersion}");
            bool found = false;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (assembly.GetName().Name == "FSharp.Build")
                {
                    if (!string.Equals(Path.GetFullPath(assembly.Location), Path.GetFullPath(LoadedTasks), StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Loaded SDK tasks instead of local tasks: " + assembly.Location);
                    found = true;
                }
            if (!found) throw new InvalidOperationException("No FSharp.Build assembly loaded in the project process");
        }
        TaskEnvironment.SetEnvironmentVariable("FSHARP_MT_MARKER", Marker);
        File.WriteAllText(TaskEnvironment.GetAbsolutePath("environment.txt").ToString(),
            $"{Environment.ProcessId}\t{TaskEnvironment.ProjectDirectory}\t{Marker}\t{LoadedTasks}");
        return true;
    }
}

public sealed class EvidenceLogger : ILogger
{
    public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Diagnostic;
    public string Parameters { get; set; }
    private StreamWriter writer;
    private readonly object gate = new object();

    public void Initialize(IEventSource source)
    {
        writer = new StreamWriter(Parameters);
        source.TaskStarted += (_, e) => Write("start", e.Timestamp, e.BuildEventContext, e.TaskName, e.ProjectFile, e.TaskAssemblyLocation);
        source.TaskFinished += (_, e) => Write("finish", e.Timestamp, e.BuildEventContext, e.TaskName, e.ProjectFile, e.Succeeded.ToString());
        source.ErrorRaised += (_, e) => Write("error", e.Timestamp, e.BuildEventContext, e.Code, e.ProjectFile, $"{e.File}:{e.LineNumber}:{e.ColumnNumber} {e.Message}");
        source.MessageRaised += (_, e) =>
        {
            // Fsc and Fsi emit ordinary messages instead of ToolTask's typed command events.
            if (e.Importance == MessageImportance.Normal && (e.Message.Contains("fsc.dll") || e.Message.Contains("fsi.dll")))
                Write("command", e.Timestamp, e.BuildEventContext, "", "", e.Message);
            if (e.Message.Contains("ran in TaskHost process"))
                Write("route", e.Timestamp, e.BuildEventContext, "", "", e.Message);
        };
    }

    private void Write(string kind, DateTime time, BuildEventContext context, string task, string project, string detail)
    {
        lock (gate)
            writer.WriteLine($"{kind}\t{time.Ticks}\t{context.NodeId}:{context.ProjectContextId}:{context.TaskId}\t{task}\t{project}\t{detail?.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ')}");
    }

    public void Shutdown() => writer?.Dispose();
}
