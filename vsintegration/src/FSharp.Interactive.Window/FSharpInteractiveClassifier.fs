// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Collections.Generic
open System.ComponentModel.Composition

open Microsoft.VisualStudio.InteractiveWindow
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Utilities

open FSharp.Compiler.Tokenization

/// Lexical colour for the window's input, which is not part of any project the editor's semantic
/// classification could see. A submission is at most a screenful, so each request tokenizes the
/// buffer from the top rather than keeping token state.
type internal FSharpInteractiveClassifier(buffer: ITextBuffer, registry: IClassificationTypeRegistryService) =

    let tokenizer = FSharpSourceTokenizer([], Some "stdin.fsx", None)

    let keyword = registry.GetClassificationType "keyword"
    let comment = registry.GetClassificationType "comment"
    let string' = registry.GetClassificationType "string"
    let number = registry.GetClassificationType "number"
    let operator = registry.GetClassificationType "operator"
    let identifier = registry.GetClassificationType "identifier"
    let preprocessor = registry.GetClassificationType "preprocessor keyword"
    let excluded = registry.GetClassificationType "excluded code"

    let classificationFor kind =
        match kind with
        | FSharpTokenColorKind.Keyword -> ValueSome keyword
        | FSharpTokenColorKind.Comment -> ValueSome comment
        | FSharpTokenColorKind.String -> ValueSome string'
        | FSharpTokenColorKind.Number -> ValueSome number
        | FSharpTokenColorKind.Operator -> ValueSome operator
        | FSharpTokenColorKind.Identifier
        | FSharpTokenColorKind.UpperIdentifier -> ValueSome identifier
        | FSharpTokenColorKind.PreprocessorKeyword -> ValueSome preprocessor
        | FSharpTokenColorKind.InactiveCode -> ValueSome excluded
        | _ -> ValueNone

    let changed = Event<EventHandler<ClassificationChangedEventArgs>, ClassificationChangedEventArgs>()

    // An edit can open or close a string or comment, changing the colour of everything after it.
    do
        buffer.Changed.Add(fun args ->
            if args.Changes.Count > 0 then
                let snapshot = args.After
                let start = snapshot.GetLineFromPosition(args.Changes[0].NewPosition).Start
                let invalidated = SnapshotSpan(start, SnapshotPoint(snapshot, snapshot.Length))
                changed.Trigger(null, ClassificationChangedEventArgs invalidated))

    interface IClassifier with

        [<CLIEvent>]
        member _.ClassificationChanged = changed.Publish

        member _.GetClassificationSpans(span: SnapshotSpan) =
            let snapshot = span.Snapshot
            let result = List<ClassificationSpan>()
            let mutable state = FSharpTokenizerLexState.Initial

            for lineNumber in 0 .. snapshot.LineCount - 1 do
                let line = snapshot.GetLineFromLineNumber lineNumber
                let text = line.GetText()
                let lineTokenizer = tokenizer.CreateLineTokenizer text
                let mutable scanning = true

                while scanning do
                    match lineTokenizer.ScanToken state with
                    | Some token, nextState ->
                        state <- nextState

                        if token.LeftColumn >= 0 && token.LeftColumn + token.FullMatchedLength <= text.Length then
                            let tokenSpan = SnapshotSpan(snapshot, line.Start.Position + token.LeftColumn, token.FullMatchedLength)

                            if tokenSpan.IntersectsWith span then
                                match classificationFor token.ColorClass with
                                | ValueSome classification -> result.Add(ClassificationSpan(tokenSpan, classification))
                                | ValueNone -> ()
                    | None, nextState ->
                        state <- nextState
                        scanning <- false

            result :> IList<_>

/// Serves the classifier for the interactive window's own buffers and no others: a buffer in a
/// document gets its colour from the project the document belongs to.
[<Export(typeof<IClassifierProvider>)>]
[<ContentType(InteractiveWindowGuids.FSharpContentTypeName)>]
type internal FSharpInteractiveClassifierProvider [<ImportingConstructor>] (registry: IClassificationTypeRegistryService) =

    interface IClassifierProvider with
        member _.GetClassifier(buffer: ITextBuffer) =
            match InteractiveWindowExtensions.GetInteractiveWindow buffer with
            | null -> null
            | _ ->
                buffer.Properties.GetOrCreateSingletonProperty(fun () -> FSharpInteractiveClassifier(buffer, registry))
                :> IClassifier | null
