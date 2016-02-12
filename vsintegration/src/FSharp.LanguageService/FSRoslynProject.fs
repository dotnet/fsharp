// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open Microsoft.VisualStudio
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Internal.Utilities.Collections
open Internal.Utilities.Debug
open System
open System.IO
open System.Diagnostics
open System.Collections.Generic
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.LanguageServices.Implementation
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem

type FSRoslynProject(hierarchy: IVsHierarchy, serviceProvider: System.IServiceProvider, visualStudioWorkspace: VisualStudioWorkspaceImpl, projectName: string) =
    inherit AbstractProject(visualStudioWorkspace.ProjectTracker, null, projectName, hierarchy, "F#", serviceProvider, null, visualStudioWorkspace, null)

    let mutable checkOptions : FSharpProjectOptions option = None

    member internal this.CheckOptions = 
            match checkOptions with 
            | Some options -> options
            | None -> failwith "Options haven't been computed yet."

    member internal this.Initialize(hier: IVsHierarchy, site : IProjectSite) =
        this.ProjectTracker.AddProject(this)

        site.AdviseProjectSiteChanges(KnownAdviseProjectSiteChangesCallbackOwners.LanguageService,
                                        new AdviseProjectSiteChanges(fun () -> this.OnProjectSettingsChanged(hier, site)))

        site.AdviseProjectSiteClosed(KnownAdviseProjectSiteChangesCallbackOwners.LanguageService, 
                                        new AdviseProjectSiteChanges(fun () -> this.Disconnect()))

        // Add files and references.
        site.SourceFilesOnDisk() |> 
            Seq.iter(fun file -> this.AddDocument(hier, file))
        
        this.GetReferences(site.CompilerFlags()) |> 
            Seq.iter(fun ref -> this.AddReference(ref) |> ignore)
        
        // Capture the F# specific options that we'll pass to the type checker.
        checkOptions <- Some(ProjectSitesAndFiles.GetProjectOptionsForProjectSite(site, site.ProjectFileName()))

    member this.GetReferences(flags : string[]) =
        let (|Reference|_|) (f : string) = if f.StartsWith("-r:") then Some (f.Replace("-r:", "")) else None

        let references = flags |> 
                         Seq.map(fun flag -> match flag with 
                                             | Reference ref -> ref
                                             | _ -> "") |>
                         Seq.where(fun s -> s <> "")
        references

    member this.AddReference(filePath : string) = 
        this.AddMetadataReferenceAndTryConvertingToProjectReferenceIfPossible(filePath, new MetadataReferenceProperties(), VSConstants.S_FALSE)

    member this.RemoveReference(filePath: string) =
        this.RemoveMetadataReference(filePath)

    member internal this.AddDocument(hier: IVsHierarchy, file : string) = 
        let itemid = 
            match hier.ParseCanonicalName(file) with
            | (VSConstants.S_OK, id) -> id
            | _ -> uint32 VSConstants.VSITEMID.Nil

        let document = this.ProjectTracker.DocumentProvider.TryGetDocumentForFile(this, itemid, file, SourceCodeKind.Regular, fun x -> true)
        this.AddDocument(document, true)

    member internal this.OnProjectSettingsChanged(hier: IVsHierarchy, site : IProjectSite) = 
        let sourceFiles = site.SourceFilesOnDisk()
        // Added files.
        sourceFiles |> 
            Seq.where(fun file -> not(this.ContainsFile(file))) |> 
            Seq.iter(fun file -> this.AddDocument(hier, file))
        // Removed files.
        this.GetCurrentDocuments() |> 
            Seq.where(fun doc -> not(sourceFiles |> Seq.contains(doc.FilePath))) |>
            Seq.iter(fun doc -> this.RemoveDocument(doc))

        let references = this.GetReferences(site.CompilerFlags())
        // Added references
        references |> 
            Seq.where(fun ref -> not(this.HasMetadataReference(ref))) |>
            Seq.iter(fun ref -> this.AddReference(ref) |> ignore)
        // Removed references
        this.GetCurrentMetadataReferences() |>
            Seq.where(fun ref -> not(references |> Seq.contains(ref.FilePath))) |>
            Seq.iter(fun ref -> this.RemoveReference(ref.FilePath))

        // If the order of files changed, that'll be captured in the checkOptions.
        checkOptions <- Some(ProjectSitesAndFiles.GetProjectOptionsForProjectSite(site, site.ProjectFileName()))
