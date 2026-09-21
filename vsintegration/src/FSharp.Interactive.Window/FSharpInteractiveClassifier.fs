// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Collections.Generic
open System.ComponentModel.Composition

open Microsoft.VisualStudio.InteractiveWindow
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Utilities

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
type internal FSharpInteractiveClassifier
    (
        buffer: ITextBuffer,
        registry: IClassificationTypeRegistryService,
        scanners: ILexicalScannerFactory,
        carriesStateAcrossLines: bool
    ) =

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
        | LexicalKind.Keyword -> ValueSome keyword
        | LexicalKind.Comment -> ValueSome comment
        | LexicalKind.String -> ValueSome string'
        | LexicalKind.Number -> ValueSome number
        | LexicalKind.Operator -> ValueSome operator
        | LexicalKind.Identifier -> ValueSome identifier
        | LexicalKind.PreprocessorKeyword -> ValueSome preprocessor
        | LexicalKind.InactiveCode -> ValueSome excluded
        | LexicalKind.Other -> ValueNone

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
            let tokens = ResizeArray<LexicalToken>()
            let mutable scanner = scanners.CreateScanner()

            for lineNumber in firstLine..lastLine do
                if not carriesStateAcrossLines then
                    scanner <- scanners.CreateScanner()

                let line = snapshot.GetLineFromLineNumber lineNumber
                let text = line.GetText()
                tokens.Clear()
                scanner.ScanLine(text, tokens)

                for token in tokens do
                    if token.Start >= 0 && token.Start + token.Length <= text.Length then
                        let tokenSpan = SnapshotSpan(snapshot, line.Start.Position + token.Start, token.Length)

                        if tokenSpan.IntersectsWith span then
                            match classificationFor token.Kind with
                            | ValueSome classification -> result.Add(ClassificationSpan(tokenSpan, classification))
                            | ValueNone -> ()

            result :> IList<_>

[<Export(typeof<IClassifierProvider>)>]
[<ContentType(InteractiveWindowGuids.FSharpContentTypeName)>]
type internal FSharpInteractiveInputClassifierProvider
    [<ImportingConstructor>]
    (registry: IClassificationTypeRegistryService, scanners: ILexicalScannerFactory) =

    interface IClassifierProvider with
        member _.GetClassifier buffer =
            buffer.Properties.GetOrCreateSingletonProperty(fun () -> FSharpInteractiveClassifier(buffer, registry, scanners, true))

[<Export(typeof<IClassifierProvider>)>]
[<ContentType(OutputContentTypeName)>]
type internal FSharpInteractiveOutputClassifierProvider
    [<ImportingConstructor>]
    (registry: IClassificationTypeRegistryService, scanners: ILexicalScannerFactory) =

    interface IClassifierProvider with
        member _.GetClassifier buffer =
            buffer.Properties.GetOrCreateSingletonProperty(fun () -> FSharpInteractiveClassifier(buffer, registry, scanners, false))
