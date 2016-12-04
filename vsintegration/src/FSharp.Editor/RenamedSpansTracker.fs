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
 
type internal Spans =
    { Old: TextSpan
      New: TextSpan }

/// Tracks the text spans that were modified as part of a rename operation
type internal RenamedSpansTracker() =
    let documentToModifiedSpansMap = Dictionary<DocumentId, ResizeArray<Spans>>()
 
    member __.IsDocumentChanged (documentId: DocumentId) = documentToModifiedSpansMap.ContainsKey(documentId)
 
    member __.AddModifiedSpan(documentId: DocumentId, oldSpan: TextSpan, newSpan: TextSpan) =
        let spans =
            match documentToModifiedSpansMap.TryGetValue(documentId) with
            | false, _ ->
                let spans = ResizeArray<Spans>()
                documentToModifiedSpansMap.[documentId] <- spans
                spans
            | true, spans -> spans
        spans.Add { Old = oldSpan; New = newSpan }
 
    // Given a position in the old solution, we get back the new adjusted position 
    member __.GetAdjustedPosition(startingPosition: int, documentId: DocumentId) =
        match documentToModifiedSpansMap.TryGetValue(documentId) with
        | true, spans -> 
            spans 
            |> Seq.filter (fun pair -> pair.Old.Start < startingPosition)
            |> Seq.fold (fun res pair -> res + (pair.New.Length - pair.Old.Length)) startingPosition
        | _ -> startingPosition
 
    member __.GetResolutionTextSpan(originalSpan: TextSpan, documentId: DocumentId) =
        match documentToModifiedSpansMap.TryGetValue(documentId) with
        | true, spans ->
            match spans |> Seq.filter (fun t -> t.Old = originalSpan) |> Seq.tryHead with
            | Some x -> x.New
            | None ->
                // The RenamedSpansTracker doesn't currently track unresolved conflicts for
                // unmodified locations.  If the document wasn't modified, we can just use the 
                // original span as the new span.
                originalSpan
        | _ -> originalSpan
 
    member __.ClearDocuments(conflictLocationDocumentIds: seq<DocumentId>) =
        for documentId in conflictLocationDocumentIds do
            documentToModifiedSpansMap.Remove(documentId) |> ignore
 
    member __.GetModifiedSpanMap(documentId: DocumentId) =
        let result = Dictionary<TextSpan, TextSpan>()
        if documentToModifiedSpansMap.ContainsKey(documentId) then
            for pair in documentToModifiedSpansMap.[documentId] do
                result.[pair.Old] <- pair.New
        result
