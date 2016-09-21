// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Linq
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Host
open Microsoft.CodeAnalysis.Editor.Navigation
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

type internal FSharpNavigableItem(document: Document, textSpan: TextSpan, displayString: string) =
    interface INavigableItem with
        member this.Glyph = Glyph.BasicFile
        member this.DisplayFileLocation = true
        member this.DisplayString = displayString
        member this.Document = document
        member this.SourceSpan = textSpan
        member this.ChildItems = ImmutableArray<INavigableItem>.Empty

[<Shared>]
[<ExportLanguageService(typeof<IGoToDefinitionService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpGoToDefinitionService [<ImportingConstructor>] ([<ImportMany>]presenters: IEnumerable<INavigableItemsPresenter>) =

    static member FindDefinition (sourceText: SourceText,
                                  filePath: string,
                                  position: int,
                                  defines: string list,
                                  options: FSharpProjectOptions,
                                  textVersionHash: int,
                                  cancellationToken: CancellationToken)
                                  : Async<Option<range>> = async {

        let textLine = sourceText.Lines.GetLineFromPosition(position)
        let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
        let textLineColumn = sourceText.Lines.GetLinePosition(position).Character
        let classifiedSpanOption =
            FSharpColorizationService.GetColorizationData(sourceText, textLine.Span, Some(filePath), defines, cancellationToken)
            |> Seq.tryFind(fun classifiedSpan -> classifiedSpan.TextSpan.Contains(position))

        let processQualifiedIdentifier(qualifiers, islandColumn) = async {
            let! parseResults = FSharpChecker.Instance.ParseFileInProject(filePath, sourceText.ToString(), options)
            let! checkFileAnswer = FSharpChecker.Instance.CheckFileInProject(parseResults, filePath, textVersionHash, sourceText.ToString(), options)
            let checkFileResults = match checkFileAnswer with
                                    | FSharpCheckFileAnswer.Aborted -> failwith "Compilation isn't complete yet"
                                    | FSharpCheckFileAnswer.Succeeded(results) -> results

            let! declarations = checkFileResults.GetDeclarationLocationAlternate (textLineNumber, islandColumn, textLine.ToString(), qualifiers, false)

            return match declarations with
                   | FSharpFindDeclResult.DeclFound(range) -> Some(range)
                   | _ -> None
        }

        return match classifiedSpanOption with
               | Some(classifiedSpan) ->
                    match classifiedSpan.ClassificationType with
                    | ClassificationTypeNames.Identifier ->
                        match QuickParse.GetCompleteIdentifierIsland true (textLine.ToString()) textLineColumn with
                        | Some(islandIdentifier, islandColumn, isQuoted) ->
                            let qualifiers = if isQuoted then [islandIdentifier] else islandIdentifier.Split '.' |> Array.toList
                            processQualifiedIdentifier(qualifiers, islandColumn) |> Async.RunSynchronously
                        | None -> None
                    | _ -> None
               | None -> None
    }
    
    // FSROSLYNTODO: Since we are not integrated with the Roslyn project system yet, the below call
    // document.Project.Solution.GetDocumentIdsWithFilePath() will only access files in the same project.
    // Either Roslyn INavigableItem needs to be extended to allow arbitary full paths, or we need to
    // fully integrate with their project system.
    member this.FindDefinitionsAsyncAux(document: Document, position: int, cancellationToken: CancellationToken) =
        let computation = async {
            let results = List<INavigableItem>()
            match FSharpLanguageService.GetOptions(document.Project.Id) with
            | Some(options) ->
                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                let! definition = FSharpGoToDefinitionService.FindDefinition(sourceText, document.FilePath, position, defines, options, textVersion.GetHashCode(), cancellationToken)

                match definition with
                | Some(range) ->
                    let refDocumentId = document.Project.Solution.GetDocumentIdsWithFilePath(range.FileName).First()
                    let refDocument = document.Project.Solution.GetDocument(refDocumentId)
                    let! refSourceText = refDocument.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let refTextSpan = CommonRoslynHelpers.FSharpRangeToTextSpan(refSourceText, range)
                    let refDisplayString = refSourceText.GetSubText(refTextSpan).ToString()
                    results.Add(FSharpNavigableItem(refDocument, refTextSpan, refDisplayString))
                | None -> ()
            | None -> ()
            return results.AsEnumerable()
        }
        
        Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken)
                .ContinueWith(CommonRoslynHelpers.GetCompletedTaskResult, cancellationToken)

    interface IGoToDefinitionService with
        member this.FindDefinitionsAsync(document: Document, position: int, cancellationToken: CancellationToken) =
            this.FindDefinitionsAsyncAux(document, position, cancellationToken)

        member this.TryGoToDefinition(document: Document, position: int, cancellationToken: CancellationToken) =
            let definitionTask = this.FindDefinitionsAsyncAux(document, position, cancellationToken)
            definitionTask.Wait(cancellationToken)
            
            if definitionTask.Status = TaskStatus.RanToCompletion then
                if definitionTask.Result.Any() then
                    let navigableItem = definitionTask.Result.First() // F# API provides only one INavigableItem
                    for presenter in presenters do
                        presenter.DisplayResult(navigableItem.DisplayString, definitionTask.Result)
                    true
                else
                    false
            else
                false
