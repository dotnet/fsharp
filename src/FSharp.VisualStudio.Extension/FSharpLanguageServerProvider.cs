// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace FSharp.VisualStudio.Extension;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FSharp.Compiler.CodeAnalysis.Workspace;
using FSharp.Compiler.Diagnostics;
using FSharp.Compiler.LanguageServer;
using FSharp.Compiler.LanguageServer.Common;

using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Extensibility.LanguageServer;
using Microsoft.VisualStudio.Extensibility.Settings;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.ProjectSystem.Query;
using Microsoft.VisualStudio.RpcContracts.LanguageServerProvider;
using Nerdbank.Streams;

/// <inheritdoc/>
#pragma warning disable VSEXTPREVIEW_LSP // Type is for evaluation purposes only and is subject to change or removal in future updates.

#pragma warning disable VSEXTPREVIEW_PROJECTQUERY_PROPERTIES_BUILDPROPERTIES // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


internal static class Extensions
{
    public static List<IQueryResultItem<T>> Please<T>(this IAsyncQueryable<T> x) => x.QueryAsync(CancellationToken.None).ToBlockingEnumerable().ToList();
}


internal class VsServerCapabilitiesOverride : IServerCapabilitiesOverride
{
    public ServerCapabilities OverrideServerCapabilities(FSharpLanguageServerConfig config, ServerCapabilities value, ClientCapabilities clientCapabilities)
    {
        // Only the semantic-tokens capability is advertised. Diagnostics and
        // code actions are unconditionally suppressed so the in-proc (devenv)
        // implementations remain authoritative until the LSP server's
        // diagnostics + code actions are graduated in a follow-up rollout.
        // Project contexts stay on because they're metadata for the client, not UX.
        var capabilities = new VSInternalServerCapabilities
        {
            TextDocumentSync = value.TextDocumentSync,
            SupportsDiagnosticRequests = false,
            ProjectContextProvider = true,
            SemanticTokensOptions = value.SemanticTokensOptions,
            CodeActionProvider = null,
            DiagnosticProvider = null,
        };
        return capabilities;
    }
}

internal class VsDiagnosticsHandler
    : IRequestHandler<VSInternalDiagnosticParams, VSInternalDiagnosticReport[], FSharpRequestContext>
{
    public bool MutatesSolutionState => false;

    [LanguageServerEndpoint(VSInternalMethods.DocumentPullDiagnosticName, LanguageServerConstants.DefaultLanguageName)]
    public async Task<VSInternalDiagnosticReport[]> HandleRequestAsync(VSInternalDiagnosticParams request, FSharpRequestContext context, CancellationToken cancellationToken)
    {
        var projectContext = Utils.tryGetProjectContext(request!.TextDocument!);
        var report = await context.Workspace.Query.GetDiagnosticsForFile(request.TextDocument!.Uri, projectContext).Please(cancellationToken);

        var snapshots = context.Workspace.Query.GetProjectSnapshotsForFile(request.TextDocument.Uri);
        var projects = Utils.snapshotsToProjectInfos(snapshots);

        var vsReport = new VSInternalDiagnosticReport
        {
            ResultId = report.ResultId,
            Diagnostics = [.. report.Diagnostics.Select(d => d.ToVsDiagnostic(projects))]
        };

        return [vsReport];
    }

}


internal class SolutionObserver : IObserver<IQueryResults<ISolutionSnapshot>>
{
    public void OnCompleted()
    {

    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(IQueryResults<ISolutionSnapshot> value)
    {
        Trace.TraceInformation("Solution was updated");
    }

}

internal class ProjectObserver(FSharpWorkspace workspace) : IObserver<IQueryResults<IProjectSnapshot>>
{
    private readonly FSharpWorkspace workspace = workspace;

    internal void ProcessProject(IProjectSnapshot project)
    {
        project.Id.TryGetValue("ProjectPath", out var projectPath);

        List<(string, string)> projectInfos = [];

        if (projectPath != null && projectPath.ToLower().EndsWith(".fsproj"))
        {
            var configs = project.ActiveConfigurations.ToList();

            foreach (var config in configs)
            {
                if (config != null)
                {
                    // Extract bin output path for each active config
                    var data = config.OutputGroups;

                    string? outputPath = null;
                    foreach (var group in data)
                    {
                        if (group.Name == "Built")
                        {
                            foreach (var output in group.Outputs)
                            {
                                if (output.FinalOutputPath != null && (output.FinalOutputPath.ToLower().EndsWith(".dll") || output.FinalOutputPath.ToLower().EndsWith(".exe")))
                                {
                                    outputPath = output.FinalOutputPath;
                                    break;
                                }
                            }
                            if (outputPath != null)
                            {
                                break;
                            }
                        }
                    }

                    foreach (var ruleResults in config.RuleResults)
                    {
                        // XXX Idk why `.Where` does not work with these IAsyncQueryable type
                        if (ruleResults?.RuleName == "CompilerCommandLineArgs")
                        {
                            // XXX Not sure why there would be more than one item for this rule result
                            // Taking first one, ignoring the rest
                            var args = ruleResults?.Items?.FirstOrDefault()?.Name;
                            if (args != null && outputPath != null) projectInfos.Add((outputPath, args));
                        }
                    }
                }
            }

            foreach (var projectInfo in projectInfos)
            {
                workspace.Projects.AddOrUpdate(projectPath, projectInfo.Item1, projectInfo.Item2.Split(';'));
            }

            //var graphPath = Path.Combine(Path.GetDirectoryName(projectPath) ?? ".", "..", "depGraph.md");

            //workspace.projects.Debug_DumpGraphOnEveryChange = FSharpOption<string>.Some(graphPath);

            //Trace.TraceInformation($"Auto-saving workspace graph to {graphPath}");

        }
    }

    public void OnNext(IQueryResults<IProjectSnapshot> result)
    {
        foreach (var project in result)
        {
            this.ProcessProject(project);
        }
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }
}


[VisualStudioContribution]
internal class FSharpLanguageServerProvider : LanguageServerProvider
{
    /// <summary>
    /// Gets the document type for FSharp code files.
    /// </summary>
    [VisualStudioContribution]
    public static DocumentTypeConfiguration FSharpDocumentType => new("F#")
    {
        FileExtensions = [".fs", ".fsi", ".fsx"],
        BaseDocumentType = LanguageServerBaseDocumentType,
    };

    /// <inheritdoc/>
    public override LanguageServerProviderConfiguration LanguageServerProviderConfiguration => new(
        "%FSharpLspExtension.FSharpLanguageServerProvider.DisplayName%",
        [Microsoft.VisualStudio.Extensibility.DocumentFilter.FromDocumentType(FSharpDocumentType)]);

    private VsTelemetryReporter? _telemetryReporter;

    /// <inheritdoc/>
    public override async Task<IDuplexPipe?> CreateServerConnectionAsync(CancellationToken cancellationToken)
    {
        _telemetryReporter = new VsTelemetryReporter();
        var activitySourceName = "fsc";

        FSharp.Compiler.LanguageServer.Activity.listenToSome();

#pragma warning disable VSEXTPREVIEW_SETTINGS // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        // Write default settings unless they're overridden. Otherwise users can't even find which settings exist.

        var settingsReadResult = await this.Extensibility.Settings().ReadEffectiveValuesAsync(FSharpExtensionSettings.AllStringSettings, cancellationToken);

        var settingValues = FSharpExtensionSettings.AllSettingsWithDefaults.Select(
            x => (x.Setting, x.DefaultIfUnset, value: settingsReadResult.ValueOrDefault(x.Setting, defaultValue: FSharpExtensionSettings.UNSET)));

        foreach (var (setting, defaultIfUnset, value) in settingValues.Where(x => x.value == FSharpExtensionSettings.UNSET))
        {
            await this.Extensibility.Settings().WriteAsync(batch =>
                batch.WriteSetting(setting, defaultIfUnset), "write default settings", cancellationToken);
        }

        var effectiveSettingValues = settingValues.ToDictionary(
            x => x.Setting,
            x => x.value == FSharpExtensionSettings.UNSET ? x.DefaultIfUnset : x.value);

        // Only semantic tokens are wired up. Diagnostics and code actions stay
        // false here so the capabilities override (which also hardcodes them
        // off) and the feature config agree.
        var serverConfig = new FSharpLanguageServerConfig(
            new FSharpLanguageServerFeatures(
                diagnostics: false,
                codeActions: false,
                semanticTokens: effectiveSettingValues[FSharpExtensionSettings.SemanticClassificationProvider] == FSharpExtensionSettings.LSP
                ));

        var disposeToEndSubscription =
            this.Extensibility.Settings().SubscribeAsync(
                [FSharpExtensionSettings.FSharpCategory],
                cancellationToken,
                changeHandler: result =>
                {
                    Trace.TraceInformation($"Settings update", result);
                });

#pragma warning restore VSEXTPREVIEW_SETTINGS // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


        //const string vsMajorVersion = "17.0";

        //var settings = OpenTelemetryExporterSettingsBuilder
        //    .CreateVSDefault(vsMajorVersion)
        //    .Build();

        //try
        //{
        //    var tracerProvider = Sdk.CreateTracerProviderBuilder()
        //            .AddVisualStudioDefaultTraceExporter(settings)
        //            //.AddConsoleExporter()
        //            .AddOtlpExporter()
        //            .Build();
        //}
        //catch (Exception e)
        //{
        //    Trace.TraceError($"Failed to create OpenTelemetry tracer provider: {e}");
        //}


        var activitySource = new ActivitySource(activitySourceName);
        var activity = activitySource.CreateActivity("CreateServerConnectionAsync", ActivityKind.Internal);

        if (activity != null)
        {
            activity.Start();
        }
        else
        {
            Trace.TraceWarning("Failed to start OpenTelemetry activity, there are no listeners");
        }

        var ws = this.Extensibility.Workspaces();

        var projectQuery = (IAsyncQueryable<IProjectSnapshot> project) => project
            .With(p => p.ActiveConfigurations
                .With(c => c.ConfigurationDimensions.With(d => d.Name).With(d => d.Value))
                .With(c => c.Properties.With(p => p.Name).With(p => p.Value))
                .With(c => c.OutputGroups.With(g => g.Name).With(g => g.Outputs.With(o => o.Name).With(o => o.FinalOutputPath).With(o => o.RootRelativeURL)))
                .With(c => c.RuleResultsByRuleName("CompilerCommandLineArgs")
                    .With(r => r.RuleName)
                    .With(r => r.Items)))
            .With(p => p.ProjectReferences
                .With(r => r.ReferencedProjectPath)
                .With(r => r.CanonicalName)
                .With(r => r.Id)
                .With(r => r.Name)
                .With(r => r.ProjectGuid)
                .With(r => r.ReferencedProjectId)
                .With(r => r.ReferenceType));

        IQueryResults<IProjectSnapshot>? result = await ws.QueryProjectsAsync(p => projectQuery(p).With(p => new { p.ActiveConfigurations, p.Id, p.Guid }), cancellationToken);

        var workspace = new FSharpWorkspace();

        foreach (var project in result)
        {
            var observer = new ProjectObserver(workspace);

            await projectQuery(project.AsQueryable()).SubscribeAsync(observer, CancellationToken.None);

            //observer.ProcessProject(project);
        }

        var ((inputStream, outputStream), _server) = FSharpLanguageServer.Create(workspace, serverConfig, (serviceCollection) =>
        {
            serviceCollection.AddSingleton<IServerCapabilitiesOverride, VsServerCapabilitiesOverride>();
            serviceCollection.AddSingleton<IMethodHandler, VsDiagnosticsHandler>();
            serviceCollection.AddSingleton<ILspTelemetry>(_telemetryReporter!);
        });

        var solutions = await ws.QuerySolutionAsync(
    solution => solution.With(solution => solution.FileName),
    cancellationToken);

        var singleSolution = solutions.FirstOrDefault();

        if (singleSolution != null)
        {
            var unsubscriber = await singleSolution
                .AsQueryable()
                .With(p => p.Projects.With(p => p.Files))
                .SubscribeAsync(new SolutionObserver(), CancellationToken.None);
        }

        return new DuplexPipe(
            PipeReader.Create(inputStream),
            PipeWriter.Create(outputStream));
    }

    /// <inheritdoc/>
    public override Task OnServerInitializationResultAsync(ServerInitializationResult serverInitializationResult, LanguageServerInitializationFailureInfo? initializationFailureInfo, CancellationToken cancellationToken)
    {
        if (serverInitializationResult == ServerInitializationResult.Failed)
        {
            // Log telemetry for failure and disable the server from being activated again.
            this.Enabled = false;
            _telemetryReporter?.ReportFault(
                TelemetryEvents.ServerFault,
                "LSP server initialization failed",
                null);
        }
        else
        {
            _telemetryReporter?.ReportEvent(
                TelemetryEvents.LspServerInitialized,
                []);
        }

        return base.OnServerInitializationResultAsync(serverInitializationResult, initializationFailureInfo, cancellationToken);
    }
}
#pragma warning restore VSEXTPREVIEW_LSP // Type is for evaluation purposes only and is subject to change or removal in future updates.
