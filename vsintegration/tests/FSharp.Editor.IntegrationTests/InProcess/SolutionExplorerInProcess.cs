// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.OperationProgress;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Threading;
using NuGet.SolutionRestoreManager;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.Extensibility.Testing;

internal partial class SolutionExplorerInProcess
{
    public const string ExistingProjectTemplate = "__fsharp_existing_project__";

    public async Task CreateSingleProjectSolutionAsync(string name, string template, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        if (string.Equals(template, ExistingProjectTemplate, StringComparison.Ordinal))
        {
            await CreateSolutionAsync(name, cancellationToken);

            var solutionDirectory = await GetDirectoryNameAsync(cancellationToken);
            var projectDirectory = Path.Combine(solutionDirectory, name);
            Directory.CreateDirectory(projectDirectory);

            var projectFilePath = Path.Combine(projectDirectory, $"{name}.fsproj");
            var programFilePath = Path.Combine(projectDirectory, "Program.fs");
            File.WriteAllText(projectFilePath, CreateStandaloneProjectFile());
            File.WriteAllText(programFilePath, "printfn \"placeholder\"");

            var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
            var solution = (EnvDTE80.Solution2)dte.Solution;
            _ = solution.AddFromFile(projectFilePath, false);
            return;
        }

        await CreateSolutionAsync(name, cancellationToken);
        await AddProjectAsync(name, template, cancellationToken);
    }

    // RepoRoot, LocalCompilerConfiguration: derived at runtime from the test assembly's location,
    // NOT from [CallerFilePath]. Arcade builds with deterministic source-root mapping that rewrites
    // CallerFilePath to "/_/..." (for symbol-server reproducibility); using it at runtime gives
    // "D:\_" on Windows CI agents, which then breaks Process.Start(WorkingDirectory) and fsc.dll
    // path resolution. Assembly.Location IS the real post-build path on disk in all environments.
    //
    // Layout: <RepoRoot>/artifacts/bin/FSharp.Editor.IntegrationTests/<Configuration>/<tfm>/FSharp.Editor.IntegrationTests.dll
    private static readonly string AssemblyDir =
        Path.GetDirectoryName(typeof(SolutionExplorerInProcess).Assembly.Location)!;

    private static readonly string LocalCompilerConfiguration =
        new DirectoryInfo(AssemblyDir).Parent!.Name;

    private static readonly string RepoRoot = Path.GetFullPath(
        Path.Combine(AssemblyDir, "..", "..", "..", "..", ".."));

    // Repo-pinned dotnet host installed by Arcade. Falls back to PATH lookup if not present
    // (developer scenarios that build the integration project outside the repo's eng infra).
    private static readonly string DotnetExe =
        File.Exists(Path.Combine(RepoRoot, ".dotnet", "dotnet.exe"))
            ? Path.Combine(RepoRoot, ".dotnet", "dotnet.exe")
            : "dotnet";

    private static string CreateStandaloneProjectFile()
    {
        var propsPath = Path.Combine(RepoRoot, "UseLocalCompiler.Directory.Build.props");

        // Sanity-check the inferred configuration: matching fsc must exist before VS tries to build.
        // Validated here (not in the static initializer) so tests that don't use the standalone path
        // can still load this type when fsc hasn't been built locally.
        var fscRoot = Path.Combine(RepoRoot, "artifacts", "bin", "fsc", LocalCompilerConfiguration);
        if (!Directory.Exists(fscRoot))
        {
            throw new InvalidOperationException(
                $"Inferred LocalCompilerConfiguration='{LocalCompilerConfiguration}' but no built fsc found at '{fscRoot}'. " +
                $"The synthesized standalone fsproj would fail to load fsc.dll -- build the F# compiler in this configuration first.");
        }

        return $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <LocalFSharpCompilerConfiguration>{LocalCompilerConfiguration}</LocalFSharpCompilerConfiguration>
    <LocalFSharpCompilerPath>{RepoRoot}</LocalFSharpCompilerPath>
  </PropertyGroup>
  <Import Project=""{propsPath}"" />
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <!-- Must match a runtime available in the dotnet host VS launches the debug target with.
         Arcade pins DOTNET_ROOT to the repo-local .dotnet with DOTNET_MULTILEVEL_LOOKUP=0; that
         install only ships the SDK's bundled runtime (10.x). Targeting net8.0 here makes
         DebuggingScenarios.exe fail to start under VS (no 8.0 runtime found), and every line
         breakpoint stays unbound (children=0,hits=0). -->
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include=""Program.fs"" />
  </ItemGroup>
</Project>";
    }

    public async Task CreateSolutionAsync(string solutionName, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var solutionPath = CreateTemporaryPath();
        await CreateSolutionAsync(solutionPath, solutionName, cancellationToken);
    }

    private async Task CreateSolutionAsync(string solutionPath, string solutionName, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        await CloseSolutionAsync(cancellationToken);

        var solutionFileName = Path.ChangeExtension(solutionName, ".sln");
        Directory.CreateDirectory(solutionPath);

        var solution = await GetRequiredGlobalServiceAsync<SVsSolution, IVsSolution>(cancellationToken);
        ErrorHandler.ThrowOnFailure(solution.CreateSolution(solutionPath, solutionFileName, (uint)__VSCREATESOLUTIONFLAGS.CSF_SILENT));
        ErrorHandler.ThrowOnFailure(solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0));
    }

    private async Task<string> GetDirectoryNameAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var solution = await GetRequiredGlobalServiceAsync<SVsSolution, IVsSolution>(cancellationToken);
        ErrorHandler.ThrowOnFailure(solution.GetSolutionInfo(out _, out var solutionFileFullPath, out _));
        if (string.IsNullOrEmpty(solutionFileFullPath))
        {
            throw new InvalidOperationException();
        }

        return Path.GetDirectoryName(solutionFileFullPath);
    }

    public async Task AddProjectAsync(string projectName, string projectTemplate, CancellationToken cancellationToken)
    {
        // dotnet new must run off the UI thread (it's a synchronous shell-out).
        var solutionDir = await GetDirectoryNameAsync(cancellationToken);
        var projectDir = Path.Combine(solutionDir, projectName);

        await TaskScheduler.Default;
        await RunDotnetNewAsync(projectTemplate, projectName, projectDir, cancellationToken);

        var projectFilePath = Path.Combine(projectDir, $"{projectName}.fsproj");
        if (!File.Exists(projectFilePath))
        {
            throw new InvalidOperationException(
                $"'dotnet new {projectTemplate}' completed but did not produce '{projectFilePath}'.");
        }

        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        var solution = (EnvDTE80.Solution2)dte.Solution;
        _ = solution.AddFromFile(projectFilePath, false);

        // Auto-open the project's main .fs file. The previous AddNewProjectFromTemplate path opened
        // the file marked OpenInEditor="true" in the .vstemplate; tests rely on this implicit open
        // (e.g. CodeActionTests calls Editor.SetTextAsync immediately afterwards).
        // SDK templates currently produce exactly one .fs file per project (Library.fs / Program.fs / Tests.fs).
        // If that ever changes (e.g. xunit template growing a Program.fs), fail loudly rather than
        // silently opening the wrong file and producing confusing content-diff failures downstream.
        var fsFiles = Directory.EnumerateFiles(projectDir, "*.fs", SearchOption.TopDirectoryOnly)
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (fsFiles.Count != 1)
        {
            throw new InvalidOperationException(
                $"Expected exactly one *.fs file in '{projectDir}' produced by 'dotnet new {projectTemplate}', " +
                $"found {fsFiles.Count}: [{string.Join(", ", fsFiles.Select(Path.GetFileName))}]. " +
                $"Update AddProjectAsync's auto-open logic to pick the correct main file for this template.");
        }
        await OpenFileAsync(projectName, Path.GetFileName(fsFiles[0]), cancellationToken);
    }

    // Shells out to `dotnet new <template> -lang F# -o <dir> --name <name>` for project creation.
    // Replaces solution.GetProjectTemplate(...) + AddNewProjectFromTemplate which required VS-side
    // .vstemplate registration; the SDK templates are the source of truth in modern .NET F# workflows
    // and avoid coupling tests to VS template-cache state in the experimental hive.
    //
    // Hermeticity:
    //   * WorkingDirectory=RepoRoot ensures global.json is found and the repo-pinned SDK is used
    //     (otherwise dotnet new walks up from VS's cwd and may pick up a different machine SDK,
    //     which can produce subtly different template output and break content-equality assertions).
    //   * DotnetExe prefers the Arcade-installed dotnet at $RepoRoot/.dotnet/dotnet.exe.
    //   * --no-restore: dotnet new for xunit triggers an implicit restore by default; tests that
    //     need restore call SolutionExplorer.RestoreNuGetPackagesAsync via VS's restore service afterwards.
    private static async Task RunDotnetNewAsync(string template, string name, string outputDir, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(outputDir);

        var psi = new ProcessStartInfo(DotnetExe, $"new {template} -lang F# -o \"{outputDir}\" --name \"{name}\" --no-restore")
        {
            WorkingDirectory = RepoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException($"Failed to start '{DotnetExe} new {template}'.");

        // Hook cancellation to actually terminate the child process. On net472 we don't have
        // Process.WaitForExitAsync(CancellationToken); a Task.Run+WaitForExit-only approach would
        // leak the process and let a hung 'dotnet new' burn the 120-min job timeout.
        using var registration = cancellationToken.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            catch
            {
                // Process already exited or kill races; nothing actionable.
            }
        });

        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();
        await Task.Run(() => process.WaitForExit(), CancellationToken.None);

        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        if (process.ExitCode != 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var message = new StringBuilder()
                .AppendLine($"'{DotnetExe} new {template} -lang F# -o \"{outputDir}\" --name \"{name}\" --no-restore' failed with exit code {process.ExitCode}.")
                .AppendLine("Standard Output:").AppendLine(stdout)
                .AppendLine("Standard Error:").AppendLine(stderr)
                .ToString();
            throw new InvalidOperationException(message);
        }
    }

    public async Task RestoreNuGetPackagesAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        var solution = (EnvDTE80.Solution2)dte.Solution;
        foreach (var project in solution.Projects.OfType<EnvDTE.Project>())
        {
            await RestoreNuGetPackagesAsync(project.FullName, cancellationToken);
        }
    }

    public async Task RestoreNuGetPackagesAsync(string projectName, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var operationProgressStatus = await GetRequiredGlobalServiceAsync<SVsOperationProgress, IVsOperationProgressStatusService>(cancellationToken);
        var stageStatus = operationProgressStatus.GetStageStatus(CommonOperationProgressStageIds.Intellisense);
        await stageStatus.WaitForCompletionAsync();

        var solutionRestoreService = await GetComponentModelServiceAsync<IVsSolutionRestoreService>(cancellationToken);
        await solutionRestoreService.CurrentRestoreOperation;

        var projectFullPath = (await GetProjectAsync(projectName, cancellationToken)).FullName;
        var solutionRestoreStatusProvider = await GetComponentModelServiceAsync<IVsSolutionRestoreStatusProvider>(cancellationToken);
        if (await solutionRestoreStatusProvider.IsRestoreCompleteAsync(cancellationToken))
        {
            return;
        }

        var solutionRestoreService2 = (IVsSolutionRestoreService2)solutionRestoreService;
        await solutionRestoreService2.NominateProjectAsync(projectFullPath, cancellationToken);

        while (true)
        {
            if (await solutionRestoreStatusProvider.IsRestoreCompleteAsync(cancellationToken))
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
        }
    }

    public async Task<IEnumerable<string>?> BuildSolutionAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var buildOutputWindowPane = await GetBuildOutputWindowPaneAsync(cancellationToken);
        buildOutputWindowPane.Clear();

        await TestServices.Shell.ExecuteCommandAsync(VSConstants.VSStd97CmdID.BuildSln, cancellationToken);
        return await WaitForBuildToFinishAsync(buildOutputWindowPane, cancellationToken);
    }

    public async Task SetStartupProjectAsync(string projectName, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        var solution = (EnvDTE80.Solution2)dte.Solution;
        var project = await GetProjectAsync(projectName, cancellationToken);
        solution.SolutionBuild.StartupProjects = project.UniqueName;
    }

    public async Task<IVsOutputWindowPane> GetBuildOutputWindowPaneAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var outputWindow = await GetRequiredGlobalServiceAsync<SVsOutputWindow, IVsOutputWindow>(cancellationToken);
        ErrorHandler.ThrowOnFailure(outputWindow.GetPane(VSConstants.OutputWindowPaneGuid.BuildOutputPane_guid, out var pane));
        return pane;
    }

    private async Task<IEnumerable<string>> WaitForBuildToFinishAsync(IVsOutputWindowPane buildOutputWindowPane, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var buildManager = await GetRequiredGlobalServiceAsync<SVsSolutionBuildManager, IVsSolutionBuildManager2>(cancellationToken);
        using var semaphore = new SemaphoreSlim(1);
        using var solutionEvents = new UpdateSolutionEvents(buildManager);

        await semaphore.WaitAsync();

        void HandleUpdateSolutionDone(bool succeeded, bool modified, bool canceled) => semaphore.Release();
        solutionEvents.OnUpdateSolutionDone += HandleUpdateSolutionDone;
        try
        {
            await semaphore.WaitAsync();
        }
        finally
        {
            solutionEvents.OnUpdateSolutionDone -= HandleUpdateSolutionDone;
        }

        // Force the error list to update
        ErrorHandler.ThrowOnFailure(buildOutputWindowPane.FlushToTaskList());

        var textView = (IVsTextView)buildOutputWindowPane;
        var wpfTextViewHost = await textView.GetTextViewHostAsync(JoinableTaskFactory, cancellationToken);
        var lines = wpfTextViewHost.TextView.TextViewLines;
        if (lines.Count < 1)
        {
            return Enumerable.Empty<string>();
        }

        return lines.Select(line => line.Extent.GetText());
    }

    private string CreateTemporaryPath()
    {
        return Path.Combine(Path.GetTempPath(), "fsharp-test", Path.GetRandomFileName());
    }

    private async Task<EnvDTE.Project> GetProjectAsync(string nameOrFileName, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        var solution = (EnvDTE80.Solution2)dte.Solution;
        return solution.Projects.OfType<EnvDTE.Project>().First(
            project =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return string.Equals(project.FileName, nameOrFileName, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(project.Name, nameOrFileName, StringComparison.OrdinalIgnoreCase);
            });
    }

    internal sealed class UpdateSolutionEvents : IVsUpdateSolutionEvents, IVsUpdateSolutionEvents2, IDisposable
    {
        private uint _cookie;
        private IVsSolutionBuildManager2 _solutionBuildManager;

        internal delegate void UpdateSolutionDoneEvent(bool succeeded, bool modified, bool canceled);

        internal delegate void UpdateSolutionBeginEvent(ref bool cancel);

        internal delegate void UpdateSolutionStartUpdateEvent(ref bool cancel);

        internal delegate void UpdateProjectConfigDoneEvent(IVsHierarchy projectHierarchy, IVsCfg projectConfig, int success);

        internal delegate void UpdateProjectConfigBeginEvent(IVsHierarchy projectHierarchy, IVsCfg projectConfig);

        public event UpdateSolutionDoneEvent? OnUpdateSolutionDone;

        public event UpdateSolutionBeginEvent? OnUpdateSolutionBegin;

        public event UpdateSolutionStartUpdateEvent? OnUpdateSolutionStartUpdate;

        public event Action? OnActiveProjectConfigurationChange;

        public event Action? OnUpdateSolutionCancel;

        public event UpdateProjectConfigDoneEvent? OnUpdateProjectConfigDone;

        public event UpdateProjectConfigBeginEvent? OnUpdateProjectConfigBegin;

        internal UpdateSolutionEvents(IVsSolutionBuildManager2 solutionBuildManager)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _solutionBuildManager = solutionBuildManager;
            ErrorHandler.ThrowOnFailure(solutionBuildManager.AdviseUpdateSolutionEvents(this, out _cookie));
        }

        int IVsUpdateSolutionEvents.UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            var cancel = false;
            OnUpdateSolutionBegin?.Invoke(ref cancel);
            if (cancel)
            {
                pfCancelUpdate = 1;
            }

            return 0;
        }

        int IVsUpdateSolutionEvents.UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            OnUpdateSolutionDone?.Invoke(fSucceeded != 0, fModified != 0, fCancelCommand != 0);
            return 0;
        }

        int IVsUpdateSolutionEvents.UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            return UpdateSolution_StartUpdate(ref pfCancelUpdate);
        }

        int IVsUpdateSolutionEvents.UpdateSolution_Cancel()
        {
            OnUpdateSolutionCancel?.Invoke();
            return 0;
        }

        int IVsUpdateSolutionEvents.OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            return OnActiveProjectCfgChange(pIVsHierarchy);
        }

        int IVsUpdateSolutionEvents2.UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            var cancel = false;
            OnUpdateSolutionBegin?.Invoke(ref cancel);
            if (cancel)
            {
                pfCancelUpdate = 1;
            }

            return 0;
        }

        int IVsUpdateSolutionEvents2.UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            OnUpdateSolutionDone?.Invoke(fSucceeded != 0, fModified != 0, fCancelCommand != 0);
            return 0;
        }

        int IVsUpdateSolutionEvents2.UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            return UpdateSolution_StartUpdate(ref pfCancelUpdate);
        }

        int IVsUpdateSolutionEvents2.UpdateSolution_Cancel()
        {
            OnUpdateSolutionCancel?.Invoke();
            return 0;
        }

        int IVsUpdateSolutionEvents2.OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            return OnActiveProjectCfgChange(pIVsHierarchy);
        }

        int IVsUpdateSolutionEvents2.UpdateProjectCfg_Begin(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
            OnUpdateProjectConfigBegin?.Invoke(pHierProj, pCfgProj);
            return 0;
        }

        int IVsUpdateSolutionEvents2.UpdateProjectCfg_Done(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            OnUpdateProjectConfigDone?.Invoke(pHierProj, pCfgProj, fSuccess);
            return 0;
        }

        private int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            var cancel = false;
            OnUpdateSolutionStartUpdate?.Invoke(ref cancel);
            if (cancel)
            {
                pfCancelUpdate = 1;
            }

            return 0;
        }

        private int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            OnActiveProjectConfigurationChange?.Invoke();
            return 0;
        }

        void IDisposable.Dispose()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            OnUpdateSolutionDone = null;
            OnUpdateSolutionBegin = null;
            OnUpdateSolutionStartUpdate = null;
            OnActiveProjectConfigurationChange = null;
            OnUpdateSolutionCancel = null;
            OnUpdateProjectConfigDone = null;
            OnUpdateProjectConfigBegin = null;

            if (_cookie != 0)
            {
                var tempCookie = _cookie;
                _cookie = 0;
                ErrorHandler.ThrowOnFailure(_solutionBuildManager.UnadviseUpdateSolutionEvents(tempCookie));
            }
        }
    }
}
