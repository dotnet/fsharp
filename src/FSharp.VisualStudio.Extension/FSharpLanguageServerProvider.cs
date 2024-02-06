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
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Extensibility.LanguageServer;
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
        [DocumentFilter.FromDocumentType(FSharpDocumentType)]);

    /// <inheritdoc/>
    public override async Task<IDuplexPipe?> CreateServerConnectionAsync(CancellationToken cancellationToken)
    {
        var what = this.Extensibility.Workspaces();
        //var result = what.QueryProjectsAsync(project => project.With(p => p.Kind == "6EC3EE1D-3C4E-46DD-8F32-0CC8E7565705"), cancellationToken).Result;
        IQueryResults<IProjectSnapshot>? result = await what.QueryProjectsAsync(project => project, cancellationToken);

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


        var ((clientStream, serverStream), _server, _trace) = FSharpLanguageServer.Create();

        return new DuplexPipe(
            PipeReader.Create(clientStream),
            PipeWriter.Create(serverStream));
    }

    private void ProcessProject(IProjectSnapshot project)
    {
        List<IQueryResultItem<IFileSnapshot>>? files = project.Files.Please();
        var references = project.ProjectReferences.Please();

        var properties = project.Properties.Please();

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
