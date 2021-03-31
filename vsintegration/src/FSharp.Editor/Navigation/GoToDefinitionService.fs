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
open FSharp.Compiler.Symbols

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
                    | FSharpGoToDefinitionResult.ExternalAssembly(targetSymbolUse, _targetExternalSymbol, metadataReferences) ->
                        let textOpt =
                            match targetSymbolUse.Symbol with
                            | :? FSharpEntity as symbol ->
                                symbol.TryGenerateSignatureText()
                                |> Option.map (fun text -> text, symbol.DisplayName)
                            | :? FSharpMemberOrFunctionOrValue as symbol ->
                                symbol.ApparentEnclosingEntity.TryGenerateSignatureText()
                                |> Option.map (fun text -> text, symbol.ApparentEnclosingEntity.DisplayName)
                            | _ ->
                                None

                        match textOpt with
                        | Some (text, fileName) ->
                            let tmpProjInfo, tmpDocInfo = 
                                MetadataAsSource.generateTemporaryDocument(
                                    AssemblyIdentity(targetSymbolUse.Symbol.Assembly.QualifiedName), 
                                    fileName, 
                                    metadataReferences)
                            let tmpShownDocOpt = metadataAsSourceService.ShowDocument(tmpProjInfo, tmpDocInfo.FilePath, SourceText.From(text.ToString()))
                            match tmpShownDocOpt with
                            | Some tmpShownDoc ->
                                let goToAsync =
                                    asyncMaybe {
                                        let userOpName = "TryGoToDefinition"
                                        let! _, _, projectOptions = projectInfoManager.TryGetOptionsForDocumentOrProject (tmpShownDoc, cancellationToken, userOpName)
                                        let! _, _, checkResults = checkerProvider.Checker.ParseAndCheckDocument(tmpShownDoc, projectOptions, userOpName)
                                        let! r =
                                            let rec areTypesEqual (ty1: FSharpType) (ty2: FSharpType) =
                                                let ty1 = ty1.StripAbbreviations()
                                                let ty2 = ty2.StripAbbreviations()
                                                ty1.TypeDefinition.DisplayName = ty2.TypeDefinition.DisplayName &&
                                                ty1.TypeDefinition.AccessPath = ty2.TypeDefinition.AccessPath &&
                                                ty1.GenericArguments.Count = ty2.GenericArguments.Count &&
                                                (
                                                    (ty1.GenericArguments, ty2.GenericArguments)
                                                    ||> Seq.forall2 areTypesEqual
                                                )

                                            // This tries to find the best possible location of the target symbol's location in the metadata source.
                                            // We really should rely on symbol equality within FCS instead of doing it here, 
                                            //     but the generated metadata as source isn't perfect for symbol equality.
                                            checkResults.GetAllUsesOfAllSymbolsInFile(cancellationToken)
                                            |> Seq.tryFind (fun x ->
                                                match x.Symbol, targetSymbolUse.Symbol with
                                                | (:? FSharpEntity as symbol1), (:? FSharpEntity as symbol2) when x.IsFromDefinition ->
                                                    symbol1.DisplayName = symbol2.DisplayName
                                                | (:? FSharpMemberOrFunctionOrValue as symbol1), (:? FSharpMemberOrFunctionOrValue as symbol2) ->
                                                    symbol1.DisplayName = symbol2.DisplayName &&
                                                    symbol1.GenericParameters.Count = symbol2.GenericParameters.Count &&
                                                    symbol1.CurriedParameterGroups.Count = symbol2.CurriedParameterGroups.Count &&
                                                    (
                                                        (symbol1.CurriedParameterGroups, symbol2.CurriedParameterGroups)
                                                        ||> Seq.forall2 (fun pg1 pg2 ->
                                                            pg1.Count = pg2.Count &&
                                                            (
                                                                (pg1, pg2)
                                                                ||> Seq.forall2 (fun p1 p2 ->
                                                                    areTypesEqual p1.Type p2.Type
                                                                )
                                                            )
                                                        )
                                                    ) &&
                                                    areTypesEqual symbol1.ReturnParameter.Type symbol2.ReturnParameter.Type
                                                | _ ->
                                                    false
                                            )
                                            |> Option.map (fun x -> x.Range)

                                        let span =
                                            match RoslynHelpers.TryFSharpRangeToTextSpan(tmpShownDoc.GetTextAsync(cancellationToken).Result, r) with
                                            | Some span -> span
                                            | _ -> TextSpan()

                                        return span                             
                                    }

                                let span =
                                    match Async.RunSynchronously(goToAsync, cancellationToken = cancellationToken) with
                                    | Some span -> span
                                    | _ -> TextSpan()

                                let navItem = FSharpGoToDefinitionNavigableItem(tmpShownDoc, span)                               
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