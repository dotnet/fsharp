// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open Microsoft.VisualStudio.FSharp.Editor.DebugHelpers
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

type RoslynTaggedText = Microsoft.CodeAnalysis.TaggedText

[<RequireQualifiedAccess>]
module internal RoslynHelpers =
    let joinWithLineBreaks segments =
        let lineBreak = TaggedText.lineBreak

        match segments |> List.filter (Seq.isEmpty >> not) with
        | [] -> Seq.empty
        | xs ->
            xs
            |> List.reduce (fun acc elem ->
                seq {
                    yield! acc
                    yield lineBreak
                    yield! elem
                })

    let FSharpRangeToTextSpan (sourceText: SourceText, range: range) =
        // Roslyn TextLineCollection is zero-based, F# range lines are one-based
        let startPosition =
            sourceText.Lines.[max 0 (range.StartLine - 1)].Start + range.StartColumn

        let endPosition =
            sourceText.Lines.[min (range.EndLine - 1) (sourceText.Lines.Count - 1)].Start
            + range.EndColumn

        TextSpan(startPosition, endPosition - startPosition)

    let TryFSharpRangeToTextSpan (sourceText: SourceText, range: range) : TextSpan voption =
        try
            ValueSome(FSharpRangeToTextSpan(sourceText, range))
        with e ->
            //Assert.Exception(e)
            ValueNone

    let TextSpanToFSharpRange (fileName: string, textSpan: TextSpan, sourceText: SourceText) : range =
        let startLine = sourceText.Lines.GetLineFromPosition textSpan.Start
        let endLine = sourceText.Lines.GetLineFromPosition textSpan.End

        mkRange
            fileName
            (Position.fromZ startLine.LineNumber (textSpan.Start - startLine.Start))
            (Position.fromZ endLine.LineNumber (textSpan.End - endLine.Start))

    /// maps from `TextTag` of the F# Compiler to Roslyn `TextTags` for use in tooltips
    let roslynTag =
        function
        | TextTag.ActivePatternCase
        | TextTag.ActivePatternResult
        | TextTag.UnionCase
        | TextTag.Enum -> TextTags.Enum
        | TextTag.Struct -> TextTags.Struct
        | TextTag.TypeParameter -> TextTags.TypeParameter
        | TextTag.Alias
        | TextTag.Class
        | TextTag.Union
        | TextTag.Record
        | TextTag.UnknownType // Default to class until/unless we use classification data
        | TextTag.Module -> TextTags.Class
        | TextTag.Interface -> TextTags.Interface
        | TextTag.Keyword -> TextTags.Keyword
        | TextTag.Member
        | TextTag.Function
        | TextTag.Method -> TextTags.Method
        | TextTag.RecordField
        | TextTag.Property -> TextTags.Property
        | TextTag.Parameter // parameter?
        | TextTag.Local -> TextTags.Local
        | TextTag.Namespace -> TextTags.Namespace
        | TextTag.Delegate -> TextTags.Delegate
        | TextTag.Event -> TextTags.Event
        | TextTag.Field -> TextTags.Field
        | TextTag.LineBreak -> TextTags.LineBreak
        | TextTag.Space -> TextTags.Space
        | TextTag.NumericLiteral -> TextTags.NumericLiteral
        | TextTag.Operator -> TextTags.Operator
        | TextTag.StringLiteral -> TextTags.StringLiteral
        | TextTag.Punctuation -> TextTags.Punctuation
        | TextTag.Text
        | TextTag.ModuleBinding // why no 'Identifier'? Does it matter?
        | TextTag.UnresolvedName
        | TextTag.UnknownEntity -> TextTags.Text

    let CollectTaggedText (list: List<_>) (t: TaggedText) =
        list.Add(RoslynTaggedText(roslynTag t.Tag, t.Text))

    type VolatileBarrier() =
        [<VolatileField>]
        let mutable isStopped = false

        member _.Proceed = not isStopped
        member _.Stop() = isStopped <- true

    // This is like Async.StartAsTask, but
    //  1. If cancellation occurs we explicitly associate the cancellation with cancellationToken
    //  2. If exception occurs then set result to Unchecked.defaultof<_>, i.e. swallow exceptions
    //     and hope that Roslyn copes with the null
    //  3. Never, ever run the computation on the UI thread - switch to thread pool if necessary
    //     This is because Roslyn makes blocking invocations of tasks from the UI thread at
    //     several points, and relies on those tasks completing in the thread pool if necessary.
    //     See for example https://github.com/dotnet/fsharp/issues/11946#issuecomment-896071454
    //     Note that no async { ... } code in FSharp.Editor is ever intended to run on the UI
    //     thread in any form. That is, our async code in FSharp.Editor is always "backgroundAsync"
    //     in the sense that it is valid switch away from the UI thread if it is ever started on
    //     the UI thread, e.g. see the corresponding backgroundTask { ... } in RFC FS-1097

    let StartAsyncAsTask (cancellationToken: CancellationToken) computation =
        // Protect against blocking the UI thread by switching to thread pool
        let computation =
            match SynchronizationContext.Current with
            | null -> computation
            | _ ->
                async {
                    do! Async.SwitchToThreadPool()
                    return! computation
                }

        let tcs = new TaskCompletionSource<_>(TaskCreationOptions.None)
        let barrier = VolatileBarrier()

        let reg =
            cancellationToken.Register(fun _ ->
                if barrier.Proceed then
                    tcs.TrySetCanceled(cancellationToken) |> ignore)

        let task = tcs.Task

        let disposeReg () =
            barrier.Stop()

            if not task.IsCanceled then
                reg.Dispose()

        Async.StartWithContinuations(
            computation,
            continuation =
                (fun result ->
                    disposeReg ()
                    tcs.TrySetResult(result) |> ignore),
            exceptionContinuation =
                (fun exn ->
                    disposeReg ()

                    match exn with
                    | :? OperationCanceledException -> tcs.TrySetCanceled(cancellationToken) |> ignore
                    | exn ->
                        System.Diagnostics.Trace.TraceError("Visual F# Tools: exception swallowed and not passed to Roslyn: {0}", exn)
                        let res = Unchecked.defaultof<_>
                        tcs.TrySetResult(res) |> ignore),
            cancellationContinuation =
                (fun _oce ->
                    disposeReg ()
                    tcs.TrySetCanceled(cancellationToken) |> ignore),
            cancellationToken = cancellationToken
        )

        task

    let StartAsyncUnitAsTask cancellationToken (computation: Async<unit>) =
        StartAsyncAsTask cancellationToken computation :> Task

    let ConvertError (error: FSharpDiagnostic, location: Location) =
        // Normalize the error message into the same format that we will receive it from the compiler.
        // This ensures that IntelliSense and Compiler errors in the 'Error List' are de-duplicated.
        // (i.e the same error does not appear twice, where the only difference is the line endings.)
        let normalizedMessage =
            error.Message
            |> FSharpDiagnostic.NormalizeErrorString
            |> FSharpDiagnostic.NewlineifyErrorString

        let id = error.ErrorNumberText
        let emptyString = LocalizableString.op_Implicit ("")
        let description = LocalizableString.op_Implicit (normalizedMessage)

        let severity =
            match error.Severity with
            | FSharpDiagnosticSeverity.Error -> DiagnosticSeverity.Error
            | FSharpDiagnosticSeverity.Warning -> DiagnosticSeverity.Warning
            | FSharpDiagnosticSeverity.Info -> DiagnosticSeverity.Info
            | FSharpDiagnosticSeverity.Hidden -> DiagnosticSeverity.Hidden

        let customTags =
            match error.ErrorNumber with
            | 1182 -> FSharpDiagnosticCustomTags.Unnecessary
            | _ -> null

        let descriptor =
            new DiagnosticDescriptor(id, emptyString, description, error.Subcategory, severity, true, emptyString, String.Empty, customTags)

        Diagnostic.Create(descriptor, location)

    let RangeToLocation (r: range, sourceText: SourceText, filePath: string) : Location =
        let linePositionSpan =
            LinePositionSpan(LinePosition(Line.toZ r.StartLine, r.StartColumn), LinePosition(Line.toZ r.EndLine, r.EndColumn))

        let textSpan = sourceText.Lines.GetTextSpan linePositionSpan
        Location.Create(filePath, textSpan, linePositionSpan)

    let StartAsyncSafe cancellationToken context computation =
        let computation =
            async {
                try
                    return! computation
                with e ->
                    FSharpOutputPane.logExceptionWithContext (e, context)
                    return Unchecked.defaultof<_>
            }

        Async.Start(computation, cancellationToken)

module internal OpenDeclarationHelper =
    /// <summary>
    /// The change that adds an open declaration at the point the insertion context names.
    /// </summary>
    /// <param name="sourceText">SourceText.</param>
    /// <param name="ctx">Insertion context. Typically returned from tryGetInsertionContext</param>
    /// <param name="declaration">The declaration to add, `open Foo` or `open type Foo`.</param>
    let getOpenDeclarationChange (sourceText: SourceText) (ctx: InsertionContext) (declaration: string) : TextChange =
        let getLineStr line =
            if line >= 0 && line < sourceText.Lines.Count then
                sourceText.Lines[line].ToString().Trim()
            else
                ""

        let pos = ParsedInput.AdjustInsertionPoint getLineStr ctx
        let line = sourceText.Lines[min (Line.toZ pos.Line) (sourceText.Lines.Count - 1)]

        // Follow the line endings the file itself uses rather than assuming the host's.
        let lineBreak =
            match line.Text.ToString(TextSpan(line.End, line.EndIncludingLineBreak - line.End)) with
            | "" -> Environment.NewLine
            | breakChars -> breakChars

        let isHeaderScope =
            match ctx.ScopeKind with
            | ScopeKind.TopModule
            | ScopeKind.Namespace
            | ScopeKind.NestedModule -> true
            | _ -> false

        // A declaration header sitting directly above becomes its own paragraph, so the open block
        // below it reads as a block rather than as part of the header.
        let separatorAbove =
            if isHeaderScope && getLineStr (line.LineNumber - 1) <> "" then
                lineBreak
            else
                ""

        let separatorBelow =
            match ctx.ScopeKind with
            // The open joins the block of opens right above it.
            | ScopeKind.OpenDeclaration -> lineBreak
            | _ when getLineStr line.LineNumber = "" -> lineBreak
            | _ -> lineBreak + lineBreak

        let margin = String(' ', pos.Column)
        let column = min pos.Column (line.End - line.Start)
        let trivia = sourceText.ToString(TextSpan(line.Start, column)).TrimEnd()

        // Anything but whitespace before the insertion point is trivia the scope's first declaration
        // follows on its line - a block comment closing there, say. Break the line at the declaration
        // rather than write the open into the middle of what precedes it.
        if trivia.Length > 0 then
            TextChange(
                TextSpan(line.Start + trivia.Length, column - trivia.Length),
                lineBreak + margin + declaration + lineBreak + lineBreak + margin
            )
        else
            TextChange(TextSpan(line.Start, 0), separatorAbove + margin + declaration + separatorBelow)

    /// <summary>
    /// Inserts open declaration into `SourceText`.
    /// </summary>
    /// <param name="sourceText">SourceText.</param>
    /// <param name="ctx">Insertion context. Typically returned from tryGetInsertionContext</param>
    /// <param name="ns">Namespace to open.</param>
    let insertOpenDeclaration (sourceText: SourceText) (ctx: InsertionContext) (ns: string) : SourceText * int =
        let change = getOpenDeclarationChange sourceText ctx ("open " + ns)
        sourceText.WithChanges change, change.Span.Start

// http://www.fssnip.net/7S3/title/Intersperse-a-list
module List =
    /// The intersperse function takes an element and a list and
    /// 'intersperses' that element between the elements of the list.
    let intersperse sep ls =
        List.foldBack
            (fun x ->
                function
                | [] -> [ x ]
                | xs -> x :: sep :: xs)
            ls
            []
