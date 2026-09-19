using System.Text.RegularExpressions;
using Microsoft.Build.Logging.StructuredLogger;
using BuildTask = Microsoft.Build.Logging.StructuredLogger.Task;

namespace ReproEvidence;

internal static class BinlogEvidence
{
    private const string SnapshotPrefix = "REPRO_EVIDENCE_V1|";

    internal static object Read(string file)
    {
        var build = BinaryLog.ReadBuild(file);
        var cells = new List<Cell>();
        build.VisitAllChildren<Project>(project =>
        {
            if (!project.ProjectFile.EndsWith(".fsproj", StringComparison.OrdinalIgnoreCase) || project.IsOuterProject)
                return;
            var tasks = Descendants<BuildTask>(project).Where(t => t.Name == "Fsc" && Owner(t) == project).ToArray();
            var snapshots = Descendants<Message>(project)
                .Where(m => Owner(m) == project && m.Text.StartsWith(SnapshotPrefix, StringComparison.Ordinal))
                .Select(m => m.Text[SnapshotPrefix.Length..].Split('|', 2))
                .Where(parts => parts.Length == 2).ToArray();
            if (tasks.Length == 0 && snapshots.Length == 0 && !project.Children.OfType<Target>().Any(t => t.Name == "CoreCompile"))
                return;

            var evaluation = build.FindEvaluation(project.EvaluationId);
            var properties = evaluation?.GetProperties() ?? new Dictionary<string, string>();
            var snapshot = snapshots.Where(p => p[0] != "ProjectReference")
                .GroupBy(p => p[0]).ToDictionary(g => g.Key, g => g.Last()[1]);
            string? Value(string name) => snapshot.GetValueOrDefault(name) ?? properties.GetValueOrDefault(name);
            var imports = evaluation == null ? [] : Descendants<Import>(evaluation)
                .Select(i => i.ImportedProjectFilePath).Where(p => !string.IsNullOrEmpty(p))
                .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            var fscTasks = tasks.Select(t => ReadTask(t, project.ProjectDirectory)).ToArray();
            var diagnostics = new List<string>();
            if (evaluation == null) diagnostics.Add("Matching project evaluation is missing.");
            if (!imports.Any(p => Path.GetFileName(p).Contains("FSharp", StringComparison.OrdinalIgnoreCase)
                               && p.EndsWith(".props", StringComparison.OrdinalIgnoreCase))
                || !imports.Any(p => Path.GetFileName(p).Contains("FSharp", StringComparison.OrdinalIgnoreCase)
                                  && p.EndsWith(".targets", StringComparison.OrdinalIgnoreCase)))
                diagnostics.Add("Evaluated F# props/targets imports are missing.");
            if (fscTasks.Length == 0) diagnostics.Add("No Fsc task executed (possibly an incremental or design-time build).");
            if (fscTasks.Any(t => !t.Complete)) diagnostics.Add("At least one Fsc task has incomplete execution evidence.");
            if (string.Equals(Value("DesignTimeBuild"), "true", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Value("BuildingInsideVisualStudio"), "true", StringComparison.OrdinalIgnoreCase))
                diagnostics.Add("IDE/host-object compilation cannot prove external compiler execution.");
            if (!snapshot.ContainsKey("OtherFlags"))
                diagnostics.Add("The pre-compilation OtherFlags snapshot is missing.");
            var expected = ReadFlags(
                SplitArguments(snapshot.GetValueOrDefault("OtherFlags") ?? "", diagnostics),
                string.IsNullOrWhiteSpace(snapshot.GetValueOrDefault("LangVersion")) ? "default" : snapshot["LangVersion"],
                bool.TryParse(snapshot.GetValueOrDefault("Optimize"), out bool optimize) ? optimize : null,
                snapshot.GetValueOrDefault("DefineConstants")?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

            var cell = new Cell
            {
                Project = project.ProjectFile,
                EvaluationId = project.EvaluationId,
                ProjectInstanceId = project.ProjectInstanceId,
                TargetFramework = Value("TargetFramework") ?? project.TargetFramework,
                Configuration = Value("Configuration") ?? project.Configuration,
                LanguageVersion = fscTasks.LastOrDefault()?.LanguageVersion,
                Optimize = fscTasks.LastOrDefault()?.Optimize,
                Defines = fscTasks.LastOrDefault()?.Defines ?? [],
                ExpectedLanguageVersion = expected.LanguageVersion,
                ExpectedOptimize = expected.Optimize,
                ExpectedDefines = expected.Defines,
                SdkVersion = Value("NETCoreSdkVersion"),
                HostPath = fscTasks.LastOrDefault()?.Executable,
                AssetsFile = Value("ProjectAssetsFile"),
                TargetPath = Value("TargetPath"),
                Imports = imports,
                ProjectReferences = snapshots.Where(p => p[0] == "ProjectReference")
                    .Select(p => p[1]).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                Snapshot = snapshot,
                FscTasks = fscTasks,
                Diagnostics = diagnostics
            };
            if (string.IsNullOrEmpty(cell.TargetFramework) || string.IsNullOrEmpty(cell.Configuration)
                || string.IsNullOrEmpty(cell.SdkVersion) || string.IsNullOrEmpty(cell.AssetsFile)
                || string.IsNullOrEmpty(cell.TargetPath) || string.IsNullOrEmpty(cell.HostPath))
                diagnostics.Add("Project/TFM/configuration/SDK/host/assets/output context is incomplete.");
            cell.Complete = diagnostics.Count == 0;
            cells.Add(cell);
        });
        return new
        {
            schemaVersion = 1,
            binlog = Path.GetFullPath(file),
            buildSucceeded = build.Succeeded,
            complete = cells.Count > 0 && cells.All(c => c.Complete),
            diagnostics = Descendants<AbstractDiagnostic>(build).Select(Diagnostic).ToArray(),
            cells
        };
    }

    private static FscRecord ReadTask(BuildTask task, string directory)
    {
        var parameters = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        var folder = task.Children.OfType<Folder>().FirstOrDefault(f => f.Name == "Parameters");
        if (folder != null)
            foreach (var node in folder.Children)
                if (node is Property property)
                    parameters[property.Name] = [property.Value];
                else if (node is Parameter parameter)
                    parameters[parameter.ParameterName ?? parameter.Name] = parameter.Children.OfType<Item>().Select(i => i.Text).ToArray();

        var record = new FscRecord
        {
            TaskAssembly = task.FromAssembly,
            TargetsPath = task.SourceFilePath,
            CommandLine = task.CommandLineArguments,
            Parameters = parameters,
            Diagnostics = Descendants<AbstractDiagnostic>(task).Select(Diagnostic).ToArray()
        };
        string? toolPath = record.Parameter("ToolPath"), toolExe = record.Parameter("ToolExe");
        string? expectedTool = toolPath != null && toolExe != null ? Path.Combine(toolPath.Trim('"'), toolExe.Trim('"')) : null;
        // Fsc overrides ToolTask.LogToolCommand with a regular Message event rather
        // than TaskCommandLineEventArgs. Accept only a message naming the actual tool.
        if (string.IsNullOrEmpty(record.CommandLine) && expectedTool != null)
            record.CommandLine = Descendants<Message>(task).Select(m => m.Text)
                .FirstOrDefault(m => m.StartsWith(expectedTool + " ", StringComparison.OrdinalIgnoreCase)
                                  || m.StartsWith('"' + expectedTool + "\" ", StringComparison.OrdinalIgnoreCase));
        string commandPattern = expectedTool != null
            ? "^\\s*(\"?" + Regex.Escape(expectedTool) + "\"?)\\s+([\\s\\S]*)$"
            : "^\\s*(\"[^\"]+\"|\\S+)\\s+([\\s\\S]*)$";
        var command = Regex.Match(record.CommandLine ?? "", commandPattern, RegexOptions.IgnoreCase);
        if (command.Success)
        {
            record.Executable = command.Groups[1].Value.Trim('"');
            record.Arguments = command.Groups[2].Value;
        }
        record.CompilerPath = record.Parameter("DotnetFscCompilerPath")?.Trim('"');
        if (string.IsNullOrEmpty(record.CompilerPath) && Path.GetFileNameWithoutExtension(record.Executable) == "fsc")
            record.CompilerPath = record.Executable;
        string arguments = record.Arguments ?? "";
        if (record.CompilerPath != null && record.CompilerPath != record.Executable)
        {
            var compiler = Regex.Match(arguments, "^\"?" + Regex.Escape(record.CompilerPath) + "\"?\\s+", RegexOptions.IgnoreCase);
            if (compiler.Success) arguments = arguments[compiler.Length..];
            else record.MissingEvidence.Add("Compiler task parameter does not match the executed command.");
        }
        // Fsc logs its expanded response file one argument per line, including spaces
        // in unquoted paths. Older single-line commands instead need quote-aware splitting.
        string[] tokens;
        if (arguments.Contains('\n'))
            tokens = arguments.Split('\n').Select(t => t.Trim()).Where(t => t.Length > 0).ToArray();
        else
            tokens = SplitArguments(arguments, record.MissingEvidence);
        (record.LanguageVersion, record.Optimize, record.Defines) = ReadFlags(tokens);
        var references = new List<string>();
        foreach (string token in tokens)
        {
            if (token.StartsWith('@') || Regex.IsMatch(token, "^(?:--lib|-I|/lib|/I)(:|$)"))
                record.MissingEvidence.Add("A response file or library-search override prevents reference resolution.");
            var reference = Regex.Match(token, "^(?:--reference|-r|/reference|/r)(?::(.*))?$");
            if (!reference.Success) continue;
            string value = reference.Groups[1].Value;
            try
            {
                if (value.StartsWith('"') && value.EndsWith('"')) value = value[1..^1];
                if (string.IsNullOrWhiteSpace(value) || value.Contains('"'))
                    throw new ArgumentException("Reference path is empty or has unsupported quoting.");
                string path = Path.GetFullPath(value, directory);
                if (!Path.IsPathFullyQualified(value) && !File.Exists(path))
                    throw new ArgumentException("Relative reference does not resolve in the project directory.");
                references.Add(path);
            }
            catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
            {
                record.MissingEvidence.Add($"Cannot resolve compiler reference '{value}': {ex.Message}");
            }
        }
        record.References = references.ToArray();
        foreach (string reference in parameters.GetValueOrDefault("References", []))
            if (!references.Contains(Path.GetFullPath(reference.Trim('"'), directory), StringComparer.OrdinalIgnoreCase))
                record.MissingEvidence.Add($"Typed task reference is absent from the compiler command: {reference}");
        if (!Path.IsPathFullyQualified(record.TaskAssembly ?? "")) record.MissingEvidence.Add("Actual Fsc task assembly path is missing.");
        if (!Path.IsPathFullyQualified(record.TargetsPath ?? "")) record.MissingEvidence.Add("Executed Fsc task definition path is missing.");
        if (!Path.IsPathFullyQualified(record.CompilerPath ?? "")) record.MissingEvidence.Add("Actual compiler path is missing.");
        if (string.IsNullOrEmpty(record.CommandLine) || record.Executable == null)
            record.MissingEvidence.Add("Actual Fsc tool command message is missing.");
        if (parameters.Count == 0) record.MissingEvidence.Add("Fsc task inputs are missing.");
        if (!record.References.Any(p => Path.GetFileName(p).Equals("FSharp.Core.dll", StringComparison.OrdinalIgnoreCase)))
            record.MissingEvidence.Add("FSharp.Core compile reference is missing.");
        if (string.Equals(record.Parameter("SkipCompilerExecution"), "true", StringComparison.OrdinalIgnoreCase))
            record.MissingEvidence.Add("Fsc explicitly skipped compiler execution.");
        if (task.EndTime == default) record.MissingEvidence.Add("Fsc task completion is missing.");
        record.Complete = record.MissingEvidence.Count == 0;
        return record;
    }

    private static string[] SplitArguments(string arguments, List<string> diagnostics)
    {
        var matches = Regex.Matches(arguments, "\\G\\s*((?:[^\\s\"]+|\"[^\"]*\")+)");
        int consumed = matches.LastOrDefault() is { } last ? last.Index + last.Length : 0;
        if (!string.IsNullOrWhiteSpace(arguments[consumed..]) || arguments.Contains("\\\"", StringComparison.Ordinal))
            diagnostics.Add("Compiler arguments contain unsupported or unterminated quoting.");
        return matches.Select(m => m.Groups[1].Value.Replace("\"", "")).ToArray();
    }

    private static (string LanguageVersion, bool? Optimize, string[] Defines) ReadFlags(
        string[] tokens, string languageVersion = "default", bool? optimize = null, string[]? defines = null)
    {
        var languageFlag = tokens.Select(t => Regex.Match(t, "^(?:--|/)langversion:(.*)$")).LastOrDefault(m => m.Success);
        var optimizeFlag = tokens.Select(t => Regex.Match(t, "^(?:--optimize|/optimize|-O|/O)([+-]?)$")).LastOrDefault(m => m.Success);
        var defineFlags = tokens.Select(t => Regex.Match(t, "^(?:--define|-d|/define|/d):(.*)$"))
            .Where(m => m.Success).Select(m => m.Groups[1].Value);
        return (
            languageFlag?.Groups[1].Value.Trim('"') ?? languageVersion,
            optimizeFlag == null ? optimize : optimizeFlag.Groups[1].Value != "-",
            (defines ?? []).Concat(defineFlags).Distinct(StringComparer.Ordinal).ToArray());
    }

    private static Project? Owner(BaseNode node)
    {
        for (var parent = node.Parent; parent != null; parent = parent.Parent)
            if (parent is Project project) return project;
        return null;
    }

    private static List<T> Descendants<T>(TreeNode node) where T : BaseNode
    {
        var result = new List<T>();
        node.VisitAllChildren<T>(result.Add);
        return result;
    }

    private static object Diagnostic(AbstractDiagnostic diagnostic) => new
    {
        severity = diagnostic is Error ? "error" : "warning",
        code = diagnostic.Code,
        message = diagnostic.Text,
        file = diagnostic.File,
        line = diagnostic.LineNumber,
        column = diagnostic.ColumnNumber,
        endLine = diagnostic.EndLineNumber,
        endColumn = diagnostic.EndColumnNumber
    };

    private sealed class Cell
    {
        public required string Project { get; init; }
        public int EvaluationId { get; init; }
        public int ProjectInstanceId { get; init; }
        public string? TargetFramework { get; init; }
        public string? Configuration { get; init; }
        public string? LanguageVersion { get; init; }
        public bool? Optimize { get; init; }
        public string[] Defines { get; init; } = [];
        public string? ExpectedLanguageVersion { get; init; }
        public bool? ExpectedOptimize { get; init; }
        public string[] ExpectedDefines { get; init; } = [];
        public string? SdkVersion { get; init; }
        public string? HostPath { get; init; }
        public string? AssetsFile { get; init; }
        public string? TargetPath { get; init; }
        public string[] Imports { get; init; } = [];
        public string[] ProjectReferences { get; init; } = [];
        public required Dictionary<string, string> Snapshot { get; init; }
        public required FscRecord[] FscTasks { get; init; }
        public required List<string> Diagnostics { get; init; }
        public bool Complete { get; set; }
    }

    private sealed class FscRecord
    {
        public string? TaskAssembly { get; init; }
        public string? TargetsPath { get; init; }
        public string? CompilerPath { get; set; }
        public string? CommandLine { get; set; }
        public string? Executable { get; set; }
        public string? HostPath => Executable;
        public string? Arguments { get; set; }
        public string? LanguageVersion { get; set; }
        public bool? Optimize { get; set; }
        public string[] Defines { get; set; } = [];
        public required Dictionary<string, string[]> Parameters { get; init; }
        public string[] References { get; set; } = [];
        public object[] Diagnostics { get; init; } = [];
        public List<string> MissingEvidence { get; } = [];
        public bool Complete { get; set; }
        internal string? Parameter(string name) => Parameters.GetValueOrDefault(name)?.LastOrDefault();
    }
}
