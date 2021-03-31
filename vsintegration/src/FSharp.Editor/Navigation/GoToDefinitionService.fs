// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.IO
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation

open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.LanguageServices

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices

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
    let metadataAsSourceService = checkerProvider.MetadataAsSource
   
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
                    | FSharpGoToDefinitionResult.ExternalAssembly(tmpProjInfo, tmpDocInfo, targetSymbolUse, _targetExternalSymbol) ->
                        let textOpt =
                            match targetSymbolUse.Symbol with
                            | :? FSharp.Compiler.Symbols.FSharpEntity as symbol ->
                                symbol.TryGenerateSignatureText()
                            | :? FSharp.Compiler.Symbols.FSharpMemberOrFunctionOrValue as symbol ->
                                symbol.ApparentEnclosingEntity.TryGenerateSignatureText()
                            | _ ->
                                None

                        match textOpt with
                        | Some text ->
                            let tmpShownDocOpt = metadataAsSourceService.ShowDocument(tmpProjInfo, tmpDocInfo.FilePath, SourceText.From(text.ToString()))
                            match tmpShownDocOpt with
                            | Some tmpShownDoc ->
                                let possibleRangesAsync =
                                    asyncMaybe {
                                        let userOpName = "TryGoToDefinition"
                                        let! _, _, projectOptions = projectInfoManager.TryGetOptionsForDocumentOrProject (tmpShownDoc, cancellationToken, userOpName)
                                        return! checkerProvider.Checker.FindBackgroundReferencesInFile(tmpShownDoc.FilePath, projectOptions, targetSymbolUse.Symbol, canInvalidateProject=false) |> liftAsync
                                    }

                                match Async.RunSynchronously(possibleRangesAsync, cancellationToken = cancellationToken) with
                                | Some possibleRanges ->
                                    let span =
                                        possibleRanges
                                        |> Seq.tryHead
                                        |> Option.map (fun r ->
                                            match RoslynHelpers.TryFSharpRangeToTextSpan(tmpShownDoc.GetTextAsync(cancellationToken).Result, r) with
                                            | Some span -> span
                                            | _ -> TextSpan()
                                        )
                                        |> Option.defaultValue (TextSpan())
                                    let navItem = FSharpGoToDefinitionNavigableItem(tmpShownDoc, span)                               
                                    gtd.NavigateToItem(navItem, statusBar)
                                    true
                                | _ ->
                                    statusBar.TempMessage (SR.CannotDetermineSymbol())
                                    false
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