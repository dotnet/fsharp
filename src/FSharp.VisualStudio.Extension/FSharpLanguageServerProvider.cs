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
using FSharp.Compiler.LanguageServer;
using FSharp.Compiler.LanguageServer.Common;

using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Extensibility.LanguageServer;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.ProjectSystem.Query;
using Microsoft.VisualStudio.RpcContracts.LanguageServerProvider;
using Nerdbank.Streams;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;
using static FSharp.Compiler.CodeAnalysis.ProjectSnapshot;

/// <inheritdoc/>
#pragma warning disable VSEXTPREVIEW_LSP // Type is for evaluation purposes only and is subject to change or removal in future updates.

#pragma warning disable VSEXTPREVIEW_PROJECTQUERY_PROPERTIES_BUILDPROPERTIES // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


internal static class Extensions
{
    public static List<IQueryResultItem<T>> Please<T>(this IAsyncQueryable<T> x) => x.QueryAsync(CancellationToken.None).ToBlockingEnumerable().ToList();
}


internal class VsServerCapabilitiesOverride : IServerCapabilitiesOverride
{
    public ServerCapabilities OverrideServerCapabilities(ServerCapabilities value)
    {
        var capabilities = new VSInternalServerCapabilities
        {
            TextDocumentSync = value.TextDocumentSync,
            SupportsDiagnosticRequests = true,
            ProjectContextProvider = true,
            DiagnosticProvider = new()
            {
                SupportsMultipleContextsDiagnostics = true,
                DiagnosticKinds = [
                        // Support a specialized requests dedicated to task-list items.  This way the client can ask just
                        // for these, independently of other diagnostics.  They can also throttle themselves to not ask if
                        // the task list would not be visible.
                        //VSInternalDiagnosticKind.Task,
                        // Dedicated request for workspace-diagnostics only.  We will only respond to these if FSA is on.
                        VSInternalDiagnosticKind.Syntax,
                        // Fine-grained diagnostics requests.  Importantly, this separates out syntactic vs semantic
                        // requests, allowing the former to quickly reach the user without blocking on the latter.  In a
                        // similar vein, compiler diagnostics are explicitly distinct from analyzer-diagnostics, allowing
                        // the former to appear as soon as possible as they are much more critical for the user and should
                        // not be delayed by a slow analyzer.
                        //new("Semantic"),
                        //new(PullDiagnosticCategories.DocumentAnalyzerSyntax),
                        //new(PullDiagnosticCategories.DocumentAnalyzerSemantic),
                    ]
            },
            SemanticTokensOptions = new()
            {
                Legend = new()
                {
                    TokenTypes = [ ..SemanticTokenTypes.AllTypes], // XXX should be extended
                    TokenModifiers = [ ..SemanticTokenModifiers.AllModifiers]
                },
                Full = new SemanticTokensFullOptions()
                {
                    Delta = false
                },
                Range = false
            },
            HoverProvider = new HoverOptions()
            { 
                WorkDoneProgress = true 
            }
        };
        return capabilities;
    }
}

internal class SemanticTokensHandler
    : IRequestHandler<SemanticTokensParams, SemanticTokens, FSharpRequestContext>
{
    public bool MutatesSolutionState => false;

    [LanguageServerEndpoint("textDocument/semanticTokens/full")]
    public async Task<SemanticTokens> HandleRequestAsync(
        SemanticTokensParams request,
        FSharpRequestContext context,
        CancellationToken cancellationToken)
    {
        var tokens = await context.GetSemanticTokensForFile(request!.TextDocument!.Uri).Please(cancellationToken);

        return new SemanticTokens { Data = tokens };
    }
}


internal class VsDiagnosticsHandler
    : IRequestHandler<VSInternalDocumentDiagnosticsParams, VSInternalDiagnosticReport[], FSharpRequestContext>,
      IRequestHandler<VSGetProjectContextsParams, VSProjectContextList, FSharpRequestContext>
{
    public bool MutatesSolutionState => false;

    [LanguageServerEndpoint(VSInternalMethods.DocumentPullDiagnosticName)]
    public async Task<VSInternalDiagnosticReport[]> HandleRequestAsync(VSInternalDocumentDiagnosticsParams request, FSharpRequestContext context, CancellationToken cancellationToken)
    {
        var result = await context.GetDiagnosticsForFile(request!.TextDocument!.Uri).Please(cancellationToken);

        var rep = new VSInternalDiagnosticReport
        {
            ResultId = "potato1", // Has to be present for diagnostic to show up
            //Identifier = 69,
            //Version = 1,
            Diagnostics =
                 result.Select(d =>

                 new Diagnostic
                 {
                     Range = new Microsoft.VisualStudio.LanguageServer.Protocol.Range
                     {
                         // F# uses 1-based indexing for lines, need to adjust
                         Start = new Position { Line = d.StartLine-1, Character = d.StartColumn },
                         End = new Position { Line = d.EndLine-1, Character = d.EndColumn }
                     },
                     Severity = DiagnosticSeverity.Error,
                     Message = $"LSP: {d.Message}",
                     //Source = "Intellisense",
                     Code = d.ErrorNumberText
                 }
             ).ToArray()
        };

        return [rep];
    }

    [LanguageServerEndpoint("textDocument/_vs_getProjectContexts")]
    public Task<VSProjectContextList> HandleRequestAsync(VSGetProjectContextsParams request, FSharpRequestContext context, CancellationToken cancellationToken)
    {
        return Task.FromResult(new VSProjectContextList()
        {
            DefaultIndex = 0,
            ProjectContexts = [
                //new() {
                //    Id = "potato",
                //    Label = "Potato",
                //    // PR for F# project kind: https://devdiv.visualstudio.com/DevDiv/_git/VSLanguageServerClient/pullrequest/529882
                //    Kind = VSProjectKind.FSharp
                //},
                //new () {
                //    Id = "potato2",
                //    Label = "Potato2",
                //    Kind = VSProjectKind.FSharp
                //}

            ]
        });
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

    /// <inheritdoc/>
    public override async Task<IDuplexPipe?> CreateServerConnectionAsync(CancellationToken cancellationToken)
    {
        var ws = this.Extensibility.Workspaces();

        IQueryResults<IProjectSnapshot>? result = await ws.QueryProjectsAsync(project => project
            .With(p => p.ActiveConfigurations
                .With(c => c.RuleResultsByRuleName("CompilerCommandLineArgs")
                    .With(r => r.RuleName)
                    .With(r => r.Items)))
            .With(p => new { p.ActiveConfigurations, p.Id, p.Guid }), cancellationToken);


        List<(string, string)> projectsAndCommandLineArgs = [];
        foreach (var project in result)
        {
            project.Id.TryGetValue("ProjectPath", out var projectPath);

            List<string> commandLineArgs = [];
            if (projectPath != null)
            {
                // There can be multiple Active Configurations, e.g. one for net8.0 and one for net472
                // TODO For now taking any single one of them, but we might actually want to pick specific one
                var config = project.ActiveConfigurations.FirstOrDefault();
                if (config != null)
                {
                    foreach (var ruleResults in config.RuleResults)
                    {
                        // XXX Idk why `.Where` does not work with these IAsyncQuerable type
                        if (ruleResults?.RuleName == "CompilerCommandLineArgs")
                        {
                            // XXX Not sure why there would be more than one item for this rule result
                            // Taking first one, ignoring the rest
                            var args = ruleResults?.Items?.FirstOrDefault()?.Name;
                            if (args != null) commandLineArgs.Add(args);
                        }
                    }
                }
                if (commandLineArgs.Count > 0)
                {
                    projectsAndCommandLineArgs.Add((projectPath, commandLineArgs[0]));
                }
            }

            try
            {
                this.ProcessProject(project);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        FSharpWorkspace workspace;

        try
        {
            List<FSharpProjectSnapshot> snapshots = [];
            foreach(var args in projectsAndCommandLineArgs)
            {
                var lines = args.Item2.Split(';'); // XXX Probably not robust enough
                var path = args.Item1;

                var snapshot = FSharpProjectSnapshot.FromCommandLineArgs(
                    lines, Path.GetDirectoryName(path), Path.GetFileName(path));
                snapshots.Add(snapshot);
            }
            workspace = FSharpWorkspace.Create(snapshots);
        }
        catch
        {
            workspace = FSharpWorkspace.Create([]);
        }

        var ((clientStream, serverStream), _server) = FSharpLanguageServer.Create(workspace, (serviceCollection) =>
        {
            serviceCollection.AddSingleton<IServerCapabilitiesOverride, VsServerCapabilitiesOverride>();
            serviceCollection.AddSingleton<IMethodHandler, VsDiagnosticsHandler>();
            serviceCollection.AddSingleton<IMethodHandler, SemanticTokensHandler>();
        });

        return new DuplexPipe(
            PipeReader.Create(clientStream),
            PipeWriter.Create(serverStream));
    }

    private void ProcessProject(IProjectSnapshot project)
    {
        List<IQueryResultItem<IFileSnapshot>>? files = project.Files.Please();
        var references = project.ProjectReferences.Please();

        var properties = project.Properties.Please();
        var id = project.Id;

        var configurationDimensions = project.ConfigurationDimensions.Please();
        var configurations = project.Configurations.Please();

        foreach (var configuration in configurations)
        {
            this.ProcessConfiguration(configuration.Value);
        }
    }

    private void ProcessConfiguration(IProjectConfigurationSnapshot configuration)
    {
        var properties = configuration.Properties.Please();
        var packageReferences = configuration.PackageReferences.Please();
        var assemblyReferences = configuration.AssemblyReferences.Please();
        var refNames = assemblyReferences.Select(r => r.Value.Name).ToList();
        var dimensions = configuration.ConfigurationDimensions.Please();
        var outputGroups = configuration.OutputGroups.Please();
        var buildProperties = configuration.BuildProperties.Please();
        var buildPropDictionary = buildProperties.Select(p => (p.Value.Name, p.Value.Value)).ToList();
        return;
    }


    /// <inheritdoc/>
    public override Task OnServerInitializationResultAsync(ServerInitializationResult serverInitializationResult, LanguageServerInitializationFailureInfo? initializationFailureInfo, CancellationToken cancellationToken)
    {
        if (serverInitializationResult == ServerInitializationResult.Failed)
        {
            // Log telemetry for failure and disable the server from being activated again.
            this.Enabled = false;
        }

        return base.OnServerInitializationResultAsync(serverInitializationResult, initializationFailureInfo, cancellationToken);
    }
}
#pragma warning restore VSEXTPREVIEW_LSP // Type is for evaluation purposes only and is subject to change or removal in future updates.
