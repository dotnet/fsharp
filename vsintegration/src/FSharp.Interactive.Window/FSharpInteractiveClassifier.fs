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

[<AutoOpen>]
module private InteractiveContentTypes =

    /// The window package names its output buffers this, whatever language the session speaks.
    [<Literal>]
    let OutputContentTypeName = "Interactive Output"

/// Lexical colour for text the editor's semantic classification never sees: the window's input,
/// which belongs to no project, and its output, where the value printer speaks F# signature syntax.
///
/// Input carries lexer state across lines, because a submission is one fragment of code and small.
/// Output is neither: it grows for the life of the session and interleaves printed values with
/// whatever the code wrote to the console, so each line is coloured on its own.
type internal FSharpInteractiveClassifier(buffer: ITextBuffer, registry: IClassificationTypeRegistryService, carriesStateAcrossLines: bool) =

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

    // Both interactive content types are shared with every language the window package hosts, and
    // ownership cannot be settled when the classifier is built: the buffer is handed to us before
    // the window has finished claiming it. So it is asked again on each request until it is known.
    let mutable ours = ValueNone

    let isOurs () =
        match ours with
        | ValueSome known -> known
        | ValueNone ->
            let known =
                match InteractiveWindowExtensions.GetInteractiveWindow buffer with
                | null -> FSharpInteractiveWindows.ownsOutputBuffer buffer
                | window -> window.Evaluator :? FSharpInteractiveEvaluator

            if known then ours <- ValueSome true
            known

    let changed = Event<EventHandler<ClassificationChangedEventArgs>, ClassificationChangedEventArgs>()

    // When lines are coloured independently an edit invalidates only the lines it touched; when
    // state is carried, an edit can open or close a string or comment and recolour everything after.
    do
        buffer.Changed.Add(fun args ->
            if args.Changes.Count > 0 && isOurs () then
                let snapshot = args.After
                let start = snapshot.GetLineFromPosition(args.Changes[0].NewPosition).Start

                let last =
                    if carriesStateAcrossLines then
                        SnapshotPoint(snapshot, snapshot.Length)
                    else
                        snapshot.GetLineFromPosition(min args.Changes[args.Changes.Count - 1].NewEnd snapshot.Length).End

                changed.Trigger(null, ClassificationChangedEventArgs(SnapshotSpan(start, last))))

    interface IClassifier with

        [<CLIEvent>]
        member _.ClassificationChanged = changed.Publish

        member _.GetClassificationSpans(span: SnapshotSpan) =
            let snapshot = span.Snapshot
            let result = List<ClassificationSpan>()

            if not (isOurs ()) then
                result :> IList<_>
            else

            let firstLine =
                if carriesStateAcrossLines then
                    0
                else
                    snapshot.GetLineNumberFromPosition span.Start.Position

            let lastLine = snapshot.GetLineNumberFromPosition span.End.Position
            let mutable state = FSharpTokenizerLexState.Initial

            for lineNumber in firstLine..lastLine do
                if not carriesStateAcrossLines then
                    state <- FSharpTokenizerLexState.Initial

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

[<Export(typeof<IClassifierProvider>)>]
[<ContentType(InteractiveWindowGuids.FSharpContentTypeName)>]
type internal FSharpInteractiveInputClassifierProvider [<ImportingConstructor>] (registry: IClassificationTypeRegistryService) =

    interface IClassifierProvider with
        member _.GetClassifier buffer =
            buffer.Properties.GetOrCreateSingletonProperty(fun () -> FSharpInteractiveClassifier(buffer, registry, true))

[<Export(typeof<IClassifierProvider>)>]
[<ContentType(OutputContentTypeName)>]
type internal FSharpInteractiveOutputClassifierProvider [<ImportingConstructor>] (registry: IClassificationTypeRegistryService) =

    interface IClassifierProvider with
        member _.GetClassifier buffer =
            buffer.Properties.GetOrCreateSingletonProperty(fun () -> FSharpInteractiveClassifier(buffer, registry, false))
