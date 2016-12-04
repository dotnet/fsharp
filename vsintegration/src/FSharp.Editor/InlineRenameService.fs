// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Linq
open System.Runtime.CompilerServices
open System.Windows
open System.Windows.Controls
open System.Windows.Media

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Editor.Implementation.InlineRename

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Text.Formatting
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

type internal FailureInlineRenameInfo() =
    interface IInlineRenameInfo with
        member __.CanRename = false
        member __.LocalizedErrorMessage = "Cannot rename symbol because it's not defined in a F# project in current solution"
        member __.TriggerSpan = Unchecked.defaultof<_>
        member __.HasOverloads = false
        member __.ForceRenameOverloads = true
        member __.DisplayName = ""
        member __.FullDisplayName = ""
        member __.Glyph = Glyph.MethodPublic
        member __.GetFinalSymbolName _replacementText = ""
        member __.GetReferenceEditSpan(_location, _cancellationToken) = Unchecked.defaultof<_>
        member __.GetConflictEditSpan(_location, _replacementText, _cancellationToken) = Nullable()
        member __.FindRenameLocationsAsync(_optionSet, _cancellationToken) = Task<IInlineRenameLocationSet>.FromResult null
        member __.TryOnBeforeGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = false
        member __.TryOnAfterGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = false

type internal InlineRenameInfo(checker: FSharpChecker, options: FSharpProjectOptions, solution: Solution, sourceText: SourceText, symbolUse: FSharpSymbolUse) =
    let gate = obj()
    let mutable underlyingFindRenameLocationsTask: Task<ImmutableDictionary<DocumentId, FSharpSymbolUse[]>> = null

    let getSpanText(document: Document, triggerSpan: TextSpan, cancellationToken: CancellationToken) =
        async {
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            return sourceText.ToString(triggerSpan)
        }

    interface IInlineRenameInfo with
        member __.CanRename = true
        member __.LocalizedErrorMessage = null
        member __.TriggerSpan = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)
        member __.HasOverloads = false
        member __.ForceRenameOverloads = true
        member __.DisplayName = symbolUse.Symbol.DisplayName
        member __.FullDisplayName = try symbolUse.Symbol.FullName with _ -> symbolUse.Symbol.DisplayName
        member __.Glyph = Glyph.MethodPublic
        member __.GetFinalSymbolName replacementText = replacementText
        
        member __.GetReferenceEditSpan(location, cancellationToken) =
            let searchName = symbolUse.Symbol.DisplayName
            let spanText = getSpanText(location.Document, location.TextSpan, cancellationToken) |> Async.RunSynchronously
            let index = spanText.LastIndexOf(searchName, StringComparison.Ordinal)
 
            if index < 0 then
                // Couldn't even find the search text at this reference location.  This might happen
                // if the user used things like unicode escapes.  IN that case, we'll have to rename
                // the entire identifier.
                location.TextSpan
            else TextSpan(location.TextSpan.Start + index, searchName.Length)

        member __.GetConflictEditSpan(location, replacementText, cancellationToken) =
            let spanText = getSpanText(location.Document, location.TextSpan, cancellationToken) |> Async.RunSynchronously
            let position = spanText.LastIndexOf(replacementText, StringComparison.Ordinal)
 
            if position < 0 then Nullable()
            else Nullable(TextSpan(location.TextSpan.Start + position, replacementText.Length))

        member __.FindRenameLocationsAsync(_optionSet, cancellationToken) =
            let renameTask =
                lock gate <| fun _ ->
                    if isNull underlyingFindRenameLocationsTask then
                        // If this is the first call, then just start finding the initial set of rename locations.
                        underlyingFindRenameLocationsTask <- 
                            async {
                                let! projectCheckResults = checker.ParseAndCheckProject(options)
                                let! symbolUses = projectCheckResults.GetUsesOfSymbol(symbolUse.Symbol)
                                
                                return
                                    (symbolUses 
                                     |> Seq.collect (fun symbolUse -> solution.GetDocumentIdsWithFilePath(symbolUse.FileName) |> Seq.map (fun id -> id, symbolUse))
                                     |> Seq.groupBy fst
                                    ).ToImmutableDictionary((fun (id, _) -> id), fun (_, xs) -> xs |> Seq.map snd |> Seq.toArray)
                            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)

                        underlyingFindRenameLocationsTask
                    else
                        // We already have a task to figure out the set of rename locations.  Let it
                        // finish, then ask it to get the rename locations with the updated options.
                        underlyingFindRenameLocationsTask
 
            async {
                let! symbolUsesByDocumentId = Async.AwaitTask renameTask

                return
                    { new IInlineRenameLocationSet with
                        /// The set of locations that need to be updated with the replacement text that the user
                        /// has entered in the inline rename session.  These are the locations are all relative
                        /// to the solution when the inline rename session began.
                        member __.Locations : IList<InlineRenameLocation> =
                            symbolUsesByDocumentId
                            |> Seq.collect (fun (KeyValue(documentId, symbolUses)) -> 
                                symbolUses
                                |> Seq.map (fun symbolUse -> 
                                    InlineRenameLocation(
                                        solution.GetDocument(documentId), 
                                        CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate))))
                            |> Seq.toArray :> _
                        
                        /// Returns the set of replacements and their possible resolutions if the user enters the
                        /// provided replacement text and options.  Replacements are keyed by their document id
                        /// and TextSpan in the original solution, and specify their new span and possible conflict
                        /// resolution.
                        member __.GetReplacementsAsync(replacementText, optionSet, cancellationToken) : Task<IInlineRenameReplacementInfo> =
                            async {
                                return 
                                    { new IInlineRenameReplacementInfo with
                                        /// The solution obtained after resolving all conflicts.
                                        member __.NewSolution = solution
                                        /// Whether or not the replacement text entered by the user is valid.
                                        member __.ReplacementTextValid = true
                                        /// The documents that need to be updated.
                                        member __.DocumentIds = symbolUsesByDocumentId.Keys
                                        /// Returns all the replacements that need to be performed for the specified document.
                                        member __.GetReplacements(documentId) =
                                            match symbolUsesByDocumentId.TryGetValue documentId with
                                            | false, _ -> Seq.empty
                                            | true, symbolUses ->
                                                symbolUses
                                                |> Array.map (fun symbolUse ->
                                                    let originalSpan = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)
                                                    let newSpan = TextSpan(originalSpan.Start, replacementText.Length)
                                                    InlineRenameReplacement(InlineRenameReplacementKind.NoConflict, originalSpan, newSpan))
                                                |> Array.toSeq
                                    }
                            }
                            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
                      }
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
            
        member __.TryOnBeforeGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = true
        member __.TryOnAfterGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = true

[<ExportLanguageService(typeof<IEditorInlineRenameService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal InlineRenameService 
    [<ImportingConstructor>]
    (
        projectInfoManager: ProjectInfoManager,
        checkerProvider: FSharpCheckerProvider,
        [<ImportMany>] _refactorNotifyServices: seq<IRefactorNotifyService>
    ) =

    static member GetInlineRenameInfo(checker: FSharpChecker, document: Document, sourceText: SourceText, filePath: string, position: int, defines: string list, options: FSharpProjectOptions, textVersionHash: int, cancellationToken: CancellationToken) : Async<IInlineRenameInfo> = 
        async {
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = textLinePos.Line + 1 // Roslyn line numbers are zero-based, FSharp.Compiler.Service line numbers are 1-based
            
            match CommonHelpers.tryClassifyAtPosition(document.Id, sourceText, filePath, defines, position, cancellationToken) with 
            | Some (islandColumn, qualifiers, _) -> 
                let! _parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText.ToString(), options)
                match checkFileAnswer with
                | FSharpCheckFileAnswer.Aborted -> return FailureInlineRenameInfo() :> _
                | FSharpCheckFileAnswer.Succeeded(checkFileResults) -> 
        
                //let! declarations = checkFileResults.GetDeclarationLocationAlternate (fcsTextLineNumber, islandColumn, textLine.ToString(), qualifiers, false)
                let! symbol = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, islandColumn, textLine.Text.ToString(), qualifiers)
                
                match symbol with
                | Some symbol ->
                    match symbol.Symbol.DeclarationLocation with
                    | Some _ -> return InlineRenameInfo(checker, options, document.Project.Solution, sourceText, symbol) :> _
                    | _ -> return FailureInlineRenameInfo() :> _
                | _ -> return FailureInlineRenameInfo() :> _
            | None -> return FailureInlineRenameInfo() :> _
        }
    
    interface IEditorInlineRenameService with
        member __.GetRenameInfoAsync(document: Document, position: int, cancellationToken: CancellationToken) : Task<IInlineRenameInfo> =
            async {
                match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)  with 
                | Some options ->
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                    let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                    return! InlineRenameService.GetInlineRenameInfo(checkerProvider.Checker, document, sourceText, document.FilePath, position, defines, options, textVersion.GetHashCode(), cancellationToken)
                | None -> return FailureInlineRenameInfo() :> _
            }
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)