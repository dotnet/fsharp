// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

#nowarn "40"

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Linq
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Editor
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.LanguageServices

[<Sealed>]
type internal SingleFileWorkspaceMap(workspace: VisualStudioWorkspace,
                                     miscFilesWorkspace: MiscellaneousFilesWorkspace,
                                     optionsManager: FSharpProjectOptionsManager, 
                                     projectContextFactory: IWorkspaceProjectContextFactory,
                                     rdt: IVsRunningDocumentTable) as this =

    let files = ConcurrentDictionary(StringComparer.OrdinalIgnoreCase)
    let projectContext = projectContextFactory.CreateProjectContext(FSharpConstants.FSharpLanguageName, FSharpConstants.FSharpMiscellaneousFilesName, null, Guid.NewGuid(), null, null)

    let createSourceCodeKind (filePath: string) =
        if isScriptFile filePath then
            SourceCodeKind.Script
        else
            SourceCodeKind.Regular

    do
        miscFilesWorkspace.DocumentOpened.Add(fun args ->
            let document = args.Document
            if document.Project.Language = FSharpConstants.FSharpLanguageName && workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Length = 0 then
                projectContext.AddSourceFile(document.FilePath, sourceCodeKind = createSourceCodeKind document.FilePath)
        )

        workspace.DocumentOpened.Add(fun args ->
            let document = args.Document
            if document.Project.Name = FSharpConstants.FSharpMiscellaneousFilesName then
                files.[document.FilePath] <- document
            elif document.Project.Language = FSharpConstants.FSharpLanguageName then
                match files.TryRemove(document.FilePath) with
                | true, _ ->
                    optionsManager.ClearSingleFileOptionsCache(document.Id)
                    projectContext.RemoveSourceFile(document.FilePath)
                | _ -> ()
        )

        workspace.DocumentClosed.Add(fun args ->
            let document = args.Document
            match files.TryRemove(document.FilePath) with
            | true, _ ->
                optionsManager.ClearSingleFileOptionsCache(document.Id)
                projectContext.RemoveSourceFile(document.FilePath)
            | _ -> ()
        )

        do
            rdt.AdviseRunningDocTableEvents(this) |> ignore

    interface IVsRunningDocTableEvents with

        member __.OnAfterAttributeChange(_, _) = VSConstants.S_OK

        member __.OnAfterDocumentWindowHide(_, _) = VSConstants.S_OK

        member __.OnAfterFirstDocumentLock(_, _, _, _) = VSConstants.S_OK

        member __.OnAfterSave(_) = VSConstants.S_OK

        member __.OnBeforeDocumentWindowShow(_, _, _) = VSConstants.S_OK

        member __.OnBeforeLastDocumentUnlock(_, _, _, _) = VSConstants.S_OK

    interface IVsRunningDocTableEvents2 with

        member __.OnAfterAttributeChange(_, _) = VSConstants.S_OK

        member __.OnAfterAttributeChangeEx(_, grfAttribs, _, _, pszMkDocumentOld, _, _, pszMkDocumentNew) = 
            // Handles renaming of a misc file
            if (grfAttribs &&& (uint32 __VSRDTATTRIB.RDTA_MkDocument)) <> 0u && files.ContainsKey(pszMkDocumentOld) then
                match files.TryRemove(pszMkDocumentOld) with
                | true, document ->
                    optionsManager.ClearSingleFileOptionsCache(document.Id)
                    projectContext.RemoveSourceFile(pszMkDocumentOld)
                    projectContext.AddSourceFile(pszMkDocumentNew, sourceCodeKind = createSourceCodeKind pszMkDocumentNew)
                | _ -> ()
            VSConstants.S_OK

        member __.OnAfterDocumentWindowHide(_, _) = VSConstants.S_OK

        member __.OnAfterFirstDocumentLock(_, _, _, _) = VSConstants.S_OK

        member __.OnAfterSave(_) = VSConstants.S_OK

        member __.OnBeforeDocumentWindowShow(_, _, _) = VSConstants.S_OK

        member __.OnBeforeLastDocumentUnlock(_, _, _, _) = VSConstants.S_OK