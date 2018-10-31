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
type internal SingleFileWorkspaceMap(workspace: Workspace,
                                     miscFilesWorkspace: MiscellaneousFilesWorkspace,
                                     optionsManager: FSharpProjectOptionsManager, 
                                     projectContextFactory: IWorkspaceProjectContextFactory) =

    let files = ConcurrentDictionary(StringComparer.OrdinalIgnoreCase)
    let projectContext = projectContextFactory.CreateProjectContext(FSharpConstants.FSharpLanguageName, FSharpConstants.FSharpMiscellaneousFiles, null, Guid.NewGuid(), null, null)

    do
        miscFilesWorkspace.DocumentOpened.Add(fun args ->
            let document = args.Document
            if document.Project.Language = FSharpConstants.FSharpLanguageName && workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Length = 0 then
                projectContext.AddSourceFile(document.FilePath)
                files.[document.FilePath] <- ()
        )

        workspace.DocumentClosed.Add(fun args ->
            match files.TryRemove(args.Document.FilePath) with
            | true, _->
                projectContext.RemoveSourceFile(args.Document.FilePath)
                optionsManager.ClearSingleFileOptionsCache(args.Document.Id)
            | _ -> ()
        )