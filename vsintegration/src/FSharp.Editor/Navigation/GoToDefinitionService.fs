﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.IO
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation

open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open System

[<Export(typeof<IFSharpGoToDefinitionService>)>]
[<Export(typeof<FSharpGoToDefinitionService>)>]
type internal FSharpGoToDefinitionService 
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =

    let gtd = GoToDefinition(checkerProvider.Checker, projectInfoManager)
    let statusBar = StatusBar(ServiceProvider.GlobalProvider.GetService<SVsStatusbar,IVsStatusbar>())  
   
    interface IFSharpGoToDefinitionService with
        /// Invoked with Peek Definition.
        member _.FindDefinitionsAsync (document: Document, position: int, cancellationToken: CancellationToken) =
            let task = gtd.FindDefinitionsForPeekTask(document, position, cancellationToken)
            task.Wait(cancellationToken)
            let results = task.Result
            results
            |> Seq.choose(fun (result, _) ->
                match result with
                | FSharpGoToDefinitionResult.NavigableItem(navItem) -> Some navItem
                | _ -> None
            )
            |> Task.FromResult

        /// Invoked with Go to Definition.
        /// Try to navigate to the definiton of the symbol at the symbolRange in the originDocument
        member _.TryGoToDefinition(document: Document, position: int, cancellationToken: CancellationToken) =
            statusBar.Message(SR.LocatingSymbol())
            use __ = statusBar.Animate()

            let gtdTask = gtd.FindDefinitionTask(document, position, cancellationToken)

            // Wrap this in a try/with as if the user clicks "Cancel" on the thread dialog, we'll be cancelled.
            // Task.Wait throws an exception if the task is cancelled, so be sure to catch it.
            try
                // This call to Wait() is fine because we want to be able to provide the error message in the status bar.
                gtdTask.Wait(cancellationToken)
                if gtdTask.Status = TaskStatus.RanToCompletion && gtdTask.Result.IsSome then
                    let result, _ = gtdTask.Result.Value
                    match result with
                    | FSharpGoToDefinitionResult.NavigableItem(navItem) ->
                        gtd.NavigateToItem(navItem, statusBar)
                        // 'true' means do it, like Sheev Palpatine would want us to.
                        true
                    | FSharpGoToDefinitionResult.ExternalAssembly(tmpProjInfo, tmpDocId, targetSymbolUse) ->
                        match targetSymbolUse.Symbol.Assembly.FileName with
                        | Some targetSymbolAssemblyFileName ->
                            let text = CSharpDecompiler.Decompile(targetSymbolUse.Symbol.FullName, targetSymbolAssemblyFileName)
                            let workspace = new AdhocWorkspace(document.Project.Solution.Workspace.Services.HostServices, WorkspaceKind.MetadataAsSource)
                            let proj = workspace.AddProject(tmpProjInfo)                         
                            let tmpDoc = proj.GetDocument(tmpDocId)
                            let _ =
                                let directoryName = Path.GetDirectoryName(tmpDoc.FilePath)
                                if Directory.Exists(directoryName) |> not then
                                    Directory.CreateDirectory(directoryName) |> ignore
                                use fileStream = new FileStream(tmpDoc.FilePath, IO.FileMode.Create)
                                use writer = new StreamWriter(fileStream)
                                text.Write(writer)
                            
                            workspace.OpenDocument(tmpDoc.Id, true)
                            let tmpShownDocOpt = CSharpDecompiler.ShowDocument(tmpDoc.FilePath, tmpDoc.Name, ServiceProvider.GlobalProvider)
                            match tmpShownDocOpt with
                            | Some tmpShownDoc ->                             
                                let navItem = FSharpGoToDefinitionNavigableItem(tmpShownDoc, Text.TextSpan())                               
                                gtd.NavigateToItem(navItem, statusBar)
                                true
                            | _ ->
                                statusBar.TempMessage (SR.CannotDetermineSymbol())
                                false
                        | _ ->
                            statusBar.TempMessage (SR.CannotDetermineSymbol())
                            false
                else 
                    statusBar.TempMessage (SR.CannotDetermineSymbol())
                    false
            with exc -> 
                statusBar.TempMessage(String.Format(SR.NavigateToFailed(), Exception.flattenMessage exc))

                // Don't show the dialog box as it's most likely that the user cancelled.
                // Don't make them click twice.
                true