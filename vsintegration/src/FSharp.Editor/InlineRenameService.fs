// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Editor.Implementation.InlineRename

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

type internal FailureInlineRenameInfo private () =
    interface IInlineRenameInfo with
        member __.CanRename = false
        member __.LocalizedErrorMessage = EditorFeaturesResources.You_cannot_rename_this_element
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
    static member Instance = FailureInlineRenameInfo()

type internal InlineRenameInfo
    (
        checker: FSharpChecker,
        options: FSharpProjectOptions, 
        document: Document,
        sourceText: SourceText, 
        symbolUse: FSharpSymbolUse,
        checkFileResults: FSharpCheckFileResults
    ) =

    let getDocumentText (document: Document) cancellationToken =
        match document.TryGetText() with
        | true, text -> text
        | _ -> document.GetTextAsync(cancellationToken).Result

    // for property access cases FSC incorrectly reports ranges including prefix
    // try to trim the range to a name
    // NOTE won't work if prefix already contains string that matches property name, i.e. `aaa.aaa`
    let fixupSpan (sourceText: SourceText) span =
        // TODO: search in-place to avoid ToString call
        let useText = sourceText.ToString(span)
        let index = useText.IndexOf(symbolUse.Symbol.DisplayName)
        if index <= 0 then span
        else TextSpan(span.Start + index, symbolUse.Symbol.DisplayName.Length)

    let triggerSpan =
        CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)
        |> fixupSpan sourceText

    let underlyingGetSymbolUsesTaskLock = obj()
    let mutable underlyingGetSymbolUsesTask: Task<ImmutableDictionary<DocumentId, FSharpSymbolUse[]>> = null
    
    let getSymbolUses cancellationToken =
        lock underlyingGetSymbolUsesTaskLock <| fun _ ->
            if isNull underlyingGetSymbolUsesTask then
                // If this is the first call, then just start finding the initial set of rename locations.
                underlyingGetSymbolUsesTask <- 
                    async {
                        let! symbolUses =
                            if symbolUse.Symbol.IsPrivateToFile then
                                checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol)
                            else
                                async {
                                    let! projectCheckResults = checker.ParseAndCheckProject(options)
                                    return! projectCheckResults.GetUsesOfSymbol(symbolUse.Symbol)
                                }
                        
                        return
                            (symbolUses 
                             |> Seq.collect (fun symbolUse -> 
                                  document.Project.Solution.GetDocumentIdsWithFilePath(symbolUse.FileName) |> Seq.map (fun id -> id, symbolUse))
                             |> Seq.groupBy fst
                            ).ToImmutableDictionary(
                                (fun (id, _) -> id), 
                                fun (_, xs) -> xs |> Seq.map snd |> Seq.toArray)
                    } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
        
                underlyingGetSymbolUsesTask
            else
                // We already have a task to figure out the set of rename locations.
                // Let it finish, then ask it to get the rename locations with the updated options.
                underlyingGetSymbolUsesTask
        |> Async.AwaitTask

    interface IInlineRenameInfo with
        /// Whether or not the entity at the selected location can be renamed.
        member __.CanRename = true
        /// Provides the reason that can be displayed to the user if the entity at the selected location cannot be renamed.
        member __.LocalizedErrorMessage = null
        /// The span of the entity that is being renamed.
        member __.TriggerSpan = triggerSpan
        /// Whether or not this entity has overloads that can also be renamed if the user wants.
        member __.HasOverloads = false
        /// Whether the Rename Overloads option should be forced to true. Used if rename is invoked from within a nameof expression.
        member __.ForceRenameOverloads = true
        /// The short name of the symbol being renamed, for use in displaying information to the user.
        member __.DisplayName = symbolUse.Symbol.DisplayName
        /// The full name of the symbol being renamed, for use in displaying information to the user.
        member __.FullDisplayName = try symbolUse.Symbol.FullName with _ -> symbolUse.Symbol.DisplayName
        /// The glyph for the symbol being renamed, for use in displaying information to the user.
        member __.Glyph = Glyph.MethodPublic
        /// Gets the final name of the symbol if the user has typed the provided replacement text in the editor.  
        /// Normally, the final name will be same as the replacement text. However, that may not always be the same.  
        /// For example, when renaming an attribute the replacement text may be "NewName" while the final symbol name might be "NewNameAttribute".
        member __.GetFinalSymbolName replacementText = replacementText

        /// Returns the actual span that should be edited in the buffer for a given rename reference
        member __.GetReferenceEditSpan(location, cancellationToken) =
            let text = getDocumentText location.Document cancellationToken
            fixupSpan text location.TextSpan 
        
        /// Returns the actual span that should be edited in the buffer for a given rename conflict
        member __.GetConflictEditSpan(location, _replacementText, _cancellationToken) = Nullable(location.TextSpan)
        
        /// Determine the set of locations to rename given the provided options. May be called 
        /// multiple times.  For example, this can be called one time for the initial set of
        /// locations to rename, as well as any time the rename options are changed by the user.
        member __.FindRenameLocationsAsync(_optionSet, cancellationToken) =
            async {
                let! symbolUsesByDocumentId = getSymbolUses cancellationToken
                let! locationsByDocument =
                    symbolUsesByDocumentId
                    |> Seq.map (fun (KeyValue(documentId, symbolUses)) ->
                        async {
                            let document = document.Project.Solution.GetDocument(documentId)
                            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                            let locations =
                                symbolUses
                                |> Array.map (fun symbolUse ->
                                    let textSpan = fixupSpan sourceText (CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate))
                                    InlineRenameLocation(document, textSpan))
                            return document, locations
                        })
                    |> Async.Parallel
                return
                    { new IInlineRenameLocationSet with
                        /// The set of locations that need to be updated with the replacement text that the user has entered in the inline rename session.  
                        /// These are the locations are all relative to the solution when the inline rename session began.
                        member __.Locations : IList<InlineRenameLocation> = 
                            [| for _, locs in locationsByDocument do yield! locs |] :> _

                        /// Returns the set of replacements and their possible resolutions if the user enters the
                        /// provided replacement text and options.  Replacements are keyed by their document id
                        /// and TextSpan in the original solution, and specify their new span and possible conflict resolution.
                        member this.GetReplacementsAsync(replacementText, optionSet, cancellationToken) : Task<IInlineRenameReplacementInfo> =
                            let rec applyChanges i (solution: Solution) =
                                async {
                                    if i = locationsByDocument.Length then 
                                        return solution
                                    else
                                        let document, locations = locationsByDocument.[i]
                                        let! oldSourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                                        let changes = locations |> Seq.map (fun loc -> TextChange(loc.TextSpan, replacementText))
                                        let newSource = oldSourceText.WithChanges(changes)
                                        return! applyChanges (i + 1) (solution.WithDocumentText(document.Id, newSource))
                                }

                            async {
                                let! newSolution = applyChanges 0 document.Project.Solution
                                return 
                                    { new IInlineRenameReplacementInfo with
                                        /// The solution obtained after resolving all conflicts.
                                        member __.NewSolution = newSolution
                                        /// Whether or not the replacement text entered by the user is valid.
                                        member __.ReplacementTextValid = true
                                        /// The documents that need to be updated.
                                        member __.DocumentIds = symbolUsesByDocumentId.Keys
                                        /// Returns all the replacements that need to be performed for the specified document.
                                        member __.GetReplacements(documentId) = Seq.empty
                                    }
                            }
                            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
                      }
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
        
        /// Called before the rename is applied to the specified documents in the workspace. 
        /// Return <code>true</code> if rename should proceed, or <code>false</code> if it should be canceled.
        member __.TryOnBeforeGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = true
        /// Called after the rename is applied to the specified documents in the workspace.  Return 
        /// <code>true</code> if this operation succeeded, or <code>false</code> if it failed.
        member __.TryOnAfterGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = true

[<ExportLanguageService(typeof<IEditorInlineRenameService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal InlineRenameService 
    [<ImportingConstructor>]
    (
        projectInfoManager: ProjectInfoManager,
        checkerProvider: FSharpCheckerProvider,
        [<ImportMany>] _refactorNotifyServices: seq<IRefactorNotifyService>
    ) =

    static member GetInlineRenameInfo(checker: FSharpChecker, document: Document, sourceText: SourceText, position: int, defines: string list, options: FSharpProjectOptions, textVersionHash: int, cancellationToken: CancellationToken) : Async<IInlineRenameInfo> = 
        async {
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = textLinePos.Line + 1 // Roslyn line numbers are zero-based, FSharp.Compiler.Service line numbers are 1-based
            
            match CommonHelpers.tryClassifyAtPosition(document.Id, sourceText, document.FilePath, defines, position, cancellationToken) with 
            | Some (islandColumn, qualifiers, _) -> 
                let! _parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(document.FilePath, textVersionHash, sourceText.ToString(), options)
                
                match checkFileAnswer with
                | FSharpCheckFileAnswer.Aborted -> return FailureInlineRenameInfo.Instance :> _
                | FSharpCheckFileAnswer.Succeeded(checkFileResults) -> 
        
                let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, islandColumn, textLine.Text.ToString(), qualifiers)
                
                match symbolUse with
                | Some symbolUse ->
                    match symbolUse.Symbol.DeclarationLocation with
                    | Some _ -> return InlineRenameInfo(checker, options, document, sourceText, symbolUse, checkFileResults) :> _
                    | _ -> return FailureInlineRenameInfo.Instance :> _
                | _ -> return FailureInlineRenameInfo.Instance :> _
            | None -> return FailureInlineRenameInfo.Instance :> _
        }
    
    interface IEditorInlineRenameService with
        member __.GetRenameInfoAsync(document: Document, position: int, cancellationToken: CancellationToken) : Task<IInlineRenameInfo> =
            async {
                match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)  with 
                | Some options ->
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                    let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                    return! InlineRenameService.GetInlineRenameInfo(checkerProvider.Checker, document, sourceText, position, defines, options, textVersion.GetHashCode(), cancellationToken)
                | None -> return FailureInlineRenameInfo.Instance :> _
            }
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)