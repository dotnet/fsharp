// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.VisualStudio
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Internal.Utilities.Collections
open System
open System.IO
open System.Diagnostics
open System.Collections.Generic
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.LanguageServices.Implementation
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem

type internal FSharpProjectSite(hierarchy: IVsHierarchy, serviceProvider: System.IServiceProvider, visualStudioWorkspace: VisualStudioWorkspaceImpl, projectName: string, projectGuid: Guid) =
    inherit AbstractProject(
        projectTracker = visualStudioWorkspace.ProjectTracker,
        reportExternalErrorCreatorOpt = null,
        projectSystemName = projectName,
        projectFilePath = projectName,
        hierarchy = hierarchy,
        language = FSharpCommonConstants.FSharpLanguageName,
        projectGuid = projectGuid,
        serviceProvider = serviceProvider,
        visualStudioWorkspaceOpt = visualStudioWorkspace,
        hostDiagnosticUpdateSourceOpt = null,
        commandLineParserServiceOpt = null)

    override this.LastDesignTimeBuildSucceeded with get() = false

    member internal this.Initialize(hier: IVsHierarchy, site : IProjectSite) =
        this.ProjectTracker.AddProject(this)

        site.AdviseProjectSiteChanges(FSharpCommonConstants.FSharpLanguageServiceCallbackName,
                                        new AdviseProjectSiteChanges(fun () -> this.OnProjectSettingsChanged(hier, site)))

        site.AdviseProjectSiteClosed(FSharpCommonConstants.FSharpLanguageServiceCallbackName, 
                                        new AdviseProjectSiteChanges(fun () -> this.Disconnect()))

        // Add files and references
        for file in site.SourceFilesOnDisk() do this.AddDocumentAux(hier, file)
        for ref in this.GetReferences(site.CompilerFlags()) do this.AddReference(ref)

    member this.GetReferences(flags : string[]) =
        flags |> Array.choose(fun flag -> if flag.StartsWith("-r:") then Some(flag.Substring(3)) else None)

    member this.AddReference(filePath : string) = 
        this.AddMetadataReferenceAndTryConvertingToProjectReferenceIfPossible(filePath, new MetadataReferenceProperties()) |> ignore

    member this.RemoveReference(filePath: string) =
        this.RemoveMetadataReference(filePath)

    member internal this.AddDocumentAux(_: IVsHierarchy, file :string) = 
        let document = this.ProjectTracker.DocumentProvider.TryGetDocumentForFile(this, null, file, SourceCodeKind.Regular, fun x -> true)
        this.AddDocument(document, true, true)

    member internal this.OnProjectSettingsChanged(hier: IVsHierarchy, site : IProjectSite) = 
        let sourceFiles = site.SourceFilesOnDisk()

        // Added files
        for file in sourceFiles do if not(this.ContainsFile(file)) then this.AddDocumentAux(hier, file)
        // Removed files
        let removedDocuments = this.GetCurrentDocuments() |> Seq.where(fun doc -> not(sourceFiles |> Seq.contains(doc.FilePath))) |> Seq.toList
        for doc in removedDocuments do this.RemoveDocument(doc)
        
        let references = this.GetReferences(site.CompilerFlags())

        // Added references
        for ref in references do if not(this.HasMetadataReference(ref)) then this.AddReference(ref)
        // Removed references
        for ref in this.GetCurrentMetadataReferences() do if not(references |> Seq.contains(ref.FilePath)) then this.RemoveReference(ref.FilePath)
