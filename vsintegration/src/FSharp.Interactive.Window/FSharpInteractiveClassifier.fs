// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Collections.Generic
open System.ComponentModel.Composition

open Microsoft.VisualStudio.InteractiveWindow
open Microsoft.VisualStudio.Language.StandardClassification
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Utilities

/// Input carries lexer state across lines: a submission is one small fragment of code. Output is
/// neither: it grows for the life of the session and mixes printed values with whatever the code
/// wrote to the console, so each of its lines is coloured on its own.
type internal FSharpInteractiveClassifier
    (buffer: ITextBuffer, registry: IClassificationTypeRegistryService, scanners: ILexicalScannerFactory, carriesStateAcrossLines: bool) =

    let classificationFor kind =
        match kind with
        | LexicalKind.Keyword -> ValueSome PredefinedClassificationTypeNames.Keyword
        | LexicalKind.Comment -> ValueSome PredefinedClassificationTypeNames.Comment
        | LexicalKind.String -> ValueSome PredefinedClassificationTypeNames.String
        | LexicalKind.Number -> ValueSome PredefinedClassificationTypeNames.Number
        | LexicalKind.Operator -> ValueSome PredefinedClassificationTypeNames.Operator
        | LexicalKind.Identifier -> ValueSome PredefinedClassificationTypeNames.Identifier
        | LexicalKind.PreprocessorKeyword -> ValueSome PredefinedClassificationTypeNames.PreprocessorKeyword
        | LexicalKind.InactiveCode -> ValueSome PredefinedClassificationTypeNames.ExcludedCode
        | LexicalKind.Other -> ValueNone
        |> ValueOption.bind (fun name ->
            match registry.GetClassificationType name with
            | null -> ValueNone
            | classification -> ValueSome classification)

    // Both interactive content types are shared with every language the window package hosts, and
    // a buffer reaches us before its window has finished claiming it, so ownership is asked again
    // on each request until it is known.
    let mutable ours = false

    let isOurs () =
        if not ours then
            ours <-
                match InteractiveWindowExtensions.GetInteractiveWindow buffer with
                | null -> FSharpInteractiveWindows.ownsOutputBuffer buffer
                | window -> window.Evaluator :? FSharpInteractiveEvaluator

        ours

    let changed =
        Event<EventHandler<ClassificationChangedEventArgs>, ClassificationChangedEventArgs>()

    do
        buffer.Changed.Add(fun args ->
            if args.Changes.Count > 0 && isOurs () then
                let snapshot = args.After
                let start = snapshot.GetLineFromPosition(args.Changes[0].NewPosition).Start

                // With carried state an edit can open or close a string or comment, which recolours
                // everything after it.
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
            let result = List<ClassificationSpan>()

            if isOurs () then
                let snapshot = span.Snapshot

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
                        if token.IsWithin text then
                            let tokenSpan =
                                SnapshotSpan(snapshot, line.Start.Position + token.Start, token.Length)

                            if tokenSpan.IntersectsWith span then
                                classificationFor token.Kind
                                |> ValueOption.iter (fun classification -> result.Add(ClassificationSpan(tokenSpan, classification)))

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
[<ContentType(PredefinedInteractiveContentTypes.InteractiveOutputContentTypeName)>]
type internal FSharpInteractiveOutputClassifierProvider
    [<ImportingConstructor>]
    (registry: IClassificationTypeRegistryService, scanners: ILexicalScannerFactory) =

    interface IClassifierProvider with
        member _.GetClassifier buffer =
            buffer.Properties.GetOrCreateSingletonProperty(fun () -> FSharpInteractiveClassifier(buffer, registry, scanners, false))
