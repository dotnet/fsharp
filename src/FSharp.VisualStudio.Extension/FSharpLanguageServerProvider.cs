// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace FSharp.VisualStudio.Extension;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using FSharp.Compiler.LanguageServer;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Extensibility.LanguageServer;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.ProjectSystem.Query;
using Microsoft.VisualStudio.RpcContracts.LanguageServerProvider;
using Nerdbank.Streams;
using static FSharp.Compiler.CodeAnalysis.ProjectSnapshot;
using static FSharp.Compiler.LanguageServer.Workspace;

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
                        VSInternalDiagnosticKind.Task,
                        // Dedicated request for workspace-diagnostics only.  We will only respond to these if FSA is on.
                        VSInternalDiagnosticKind.Syntax,
                        // Fine-grained diagnostics requests.  Importantly, this separates out syntactic vs semantic
                        // requests, allowing the former to quickly reach the user without blocking on the latter.  In a
                        // similar vein, compiler diagnostics are explicitly distinct from analyzer-diagnostics, allowing
                        // the former to appear as soon as possible as they are much more critical for the user and should
                        // not be delayed by a slow analyzer.
                        new("Semantic"),
                        //new(PullDiagnosticCategories.DocumentAnalyzerSyntax),
                        //new(PullDiagnosticCategories.DocumentAnalyzerSemantic),
                    ]
            }
        };
        return capabilities;
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
                         Start = new Position { Line = d.StartLine, Character = d.StartColumn },
                         End = new Position { Line = d.EndLine, Character = d.EndColumn }
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
                new() {
                    Id = "potato",
                    Label = "Potato",
                    // PR for F# project kind: https://devdiv.visualstudio.com/DevDiv/_git/VSLanguageServerClient/pullrequest/529882
                    Kind = VSProjectKind.VisualBasic
                },
                new () {
                    Id = "potato2",
                    Label = "Potato2",
                    Kind = VSProjectKind.VisualBasic
                }

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
        //var result = what.QueryProjectsAsync(project => project.With(p => p.Kind == "6EC3EE1D-3C4E-46DD-8F32-0CC8E7565705"), cancellationToken).Result;
        IQueryResults<IProjectSnapshot>? result = await ws.QueryProjectsAsync(project => project.With(p => new { p.ActiveConfigurations, p.Id, p.Guid }), cancellationToken);

        var x = await ws.QuerySolutionAsync(solution => solution.With(s => new { s.Path, s.Guid, s.ActiveConfiguration, s.ActivePlatform }), cancellationToken);

        foreach (var project in result)
        {
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
            // Some hardcoded projects before we create them from the ProjectQuery
            var projectsRoot = @"D:\code";
            var giraffe = FSharpProjectSnapshot.FromResponseFile(
                new FileInfo(Path.Combine(projectsRoot, @"Giraffe\src\Giraffe\Giraffe.rsp")),
                Path.Combine(projectsRoot, @"Giraffe\src\Giraffe\Giraffe.fsproj"));
            var giraffeTests = FSharpProjectSnapshot.FromResponseFile(
                new FileInfo(Path.Combine(projectsRoot, @"Giraffe\tests\Giraffe.Tests\Giraffe.Tests.rsp")),
                Path.Combine(projectsRoot, @"Giraffe\tests\Giraffe.Tests\Giraffe.Tests.fsproj"));
            workspace = FSharpWorkspace.Create([giraffe, giraffeTests]);
        }
        catch
        {
            workspace = FSharpWorkspace.Create([]);
        }

        var ((clientStream, serverStream), _server) = FSharpLanguageServer.Create(workspace, (serviceCollection) =>
        {
            serviceCollection.AddSingleton<IServerCapabilitiesOverride, VsServerCapabilitiesOverride>();
            serviceCollection.AddSingleton<IMethodHandler, VsDiagnosticsHandler>();
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
