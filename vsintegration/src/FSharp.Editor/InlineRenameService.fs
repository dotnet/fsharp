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
open Microsoft.CodeAnalysis.Rename.ConflictEngine

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

type internal InlineRenameInfo
    (
        checker: FSharpChecker,
        options: FSharpProjectOptions, 
        document: Document,
        sourceText: SourceText, 
        symbolUse: FSharpSymbolUse,
        checkFileResults: FSharpCheckFileResults
    ) =
    
    let renamedSpansTracker = RenamedSpansTracker()

    let getSpanText(document: Document, triggerSpan: TextSpan, cancellationToken: CancellationToken) =
        async {
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            return sourceText.ToString(triggerSpan)
        }

    let triggerSpan = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)
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
                                fun (_, xs) -> 
                                    xs 
                                    |> Seq.map snd 
                                    |> Seq.sortBy (fun (x: FSharpSymbolUse) -> x.RangeAlternate.StartLine, x.RangeAlternate.StartColumn) 
                                    |> Seq.toArray)
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
            let searchName = symbolUse.Symbol.DisplayName
            let spanText = getSpanText(location.Document, location.TextSpan, cancellationToken) |> Async.RunSynchronously
            let _index = spanText.LastIndexOf(searchName, StringComparison.Ordinal)
            //if index < 0 then
                // Couldn't even find the search text at this reference location.  This might happen
                // if the user used things like unicode escapes. In that case, we'll have to rename the entire identifier.
            location.TextSpan
            //else TextSpan(location.TextSpan.Start + index, searchName.Length)
        
        /// Returns the actual span that should be edited in the buffer for a given rename conflict
        member __.GetConflictEditSpan(location, replacementText, cancellationToken) =
            let spanText = getSpanText(location.Document, location.TextSpan, cancellationToken) |> Async.RunSynchronously
            let _position = spanText.LastIndexOf(replacementText, StringComparison.Ordinal)
 
            //if position < 0 then Nullable()
            //let position = renamedSpansTracker.GetAdjustedPosition(location.TextSpan.Start, location.Document.Id)
            Nullable(TextSpan(location.TextSpan.Start, replacementText.Length))
        
        /// Determine the set of locations to rename given the provided options. May be called 
        /// multiple times.  For example, this can be called one time for the initial set of
        /// locations to rename, as well as any time the rename options are changed by the user.
        member __.FindRenameLocationsAsync(_optionSet, cancellationToken) =
            async {
                let! symbolUsesByDocumentId = getSymbolUses cancellationToken
                return
                    { new IInlineRenameLocationSet with
                        /// The set of locations that need to be updated with the replacement text that the user has entered in the inline rename session.  
                        /// These are the locations are all relative to the solution when the inline rename session began.
                        member __.Locations : IList<InlineRenameLocation> =
                            symbolUsesByDocumentId
                            |> Seq.collect (fun (KeyValue(documentId, symbolUses)) -> 
                                symbolUses
                                |> Seq.map (fun symbolUse -> 
                                    let sourceText = document.Project.Solution.GetDocument(documentId).GetTextAsync(cancellationToken).Result // !!!!!!!!!!!!!!!!!
                                    InlineRenameLocation(
                                        document.Project.Solution.GetDocument(documentId), 
                                        CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate))))
                            |> Seq.toArray :> _
                        
                        /// Returns the set of replacements and their possible resolutions if the user enters the
                        /// provided replacement text and options.  Replacements are keyed by their document id
                        /// and TextSpan in the original solution, and specify their new span and possible conflict resolution.
                        member __.GetReplacementsAsync(replacementText, optionSet, cancellationToken) : Task<IInlineRenameReplacementInfo> =
                            async {
                                return 
                                    { new IInlineRenameReplacementInfo with
                                        /// The solution obtained after resolving all conflicts.
                                        member __.NewSolution = document.Project.Solution // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                        /// Whether or not the replacement text entered by the user is valid.
                                        member __.ReplacementTextValid = true
                                        /// The documents that need to be updated.
                                        member __.DocumentIds = symbolUsesByDocumentId.Keys
                                        /// Returns all the replacements that need to be performed for the specified document.
                                        member __.GetReplacements(documentId) =
                                            match symbolUsesByDocumentId.TryGetValue documentId with
                                            | false, _ -> Seq.empty
                                            | true, symbolUses ->
                                                renamedSpansTracker.ClearDocuments [documentId]
                                                symbolUses
                                                |> Array.map (fun symbolUse ->
                                                    let sourceText = document.Project.Solution.GetDocument(documentId).GetTextAsync().Result // !!!!!!!!!!!!!!!!!!
                                                    let originalSpan = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)
                                                    let startPosition = renamedSpansTracker.GetAdjustedPosition(originalSpan.Start, documentId)
                                                    let newSpan = TextSpan(startPosition, replacementText.Length)
                                                    renamedSpansTracker.AddModifiedSpan(documentId, originalSpan, newSpan)
                                                    InlineRenameReplacement(InlineRenameReplacementKind.NoConflict, originalSpan, newSpan))
                                                |> Array.toSeq
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
        
                let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, islandColumn, textLine.Text.ToString(), qualifiers)
                
                match symbolUse with
                | Some symbolUse ->
                    match symbolUse.Symbol.DeclarationLocation with
                    | Some _ -> return InlineRenameInfo(checker, options, document, sourceText, symbolUse, checkFileResults) :> _
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