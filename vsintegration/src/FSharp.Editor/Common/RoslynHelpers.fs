// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Diagnostics
open FSharp.Compiler
open FSharp.Compiler.Layout
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Range
open Microsoft.VisualStudio.FSharp.Editor.Logging
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

[<RequireQualifiedAccess>]
module internal RoslynHelpers =

    let FSharpRangeToTextSpan(sourceText: SourceText, range: range) =
        // Roslyn TextLineCollection is zero-based, F# range lines are one-based
        let startPosition = sourceText.Lines.[max 0 (range.StartLine - 1)].Start + range.StartColumn
        let endPosition = sourceText.Lines.[min (range.EndLine - 1) (sourceText.Lines.Count - 1)].Start + range.EndColumn
        TextSpan(startPosition, endPosition - startPosition)

    let TryFSharpRangeToTextSpan(sourceText: SourceText, range: range) : TextSpan option =
        try Some(FSharpRangeToTextSpan(sourceText, range))
        with e -> 
            //Assert.Exception(e)
            None

    let TextSpanToFSharpRange(fileName: string, textSpan: TextSpan, sourceText: SourceText) : range =
        let startLine = sourceText.Lines.GetLineFromPosition textSpan.Start
        let endLine = sourceText.Lines.GetLineFromPosition textSpan.End
        mkRange 
            fileName 
            (Pos.fromZ startLine.LineNumber (textSpan.Start - startLine.Start))
            (Pos.fromZ endLine.LineNumber (textSpan.End - endLine.Start))

    let GetCompletedTaskResult(task: Task<'TResult>) =
        if task.Status = TaskStatus.RanToCompletion then
            task.Result
        else
            Assert.Exception(task.Exception.GetBaseException())
            raise(task.Exception.GetBaseException())



    /// maps from `LayoutTag` of the F# Compiler to Roslyn `TextTags` for use in tooltips
    let roslynTag = function
    | LayoutTag.ActivePatternCase
    | LayoutTag.ActivePatternResult
    | LayoutTag.UnionCase
    | LayoutTag.Enum -> TextTags.Enum
    | LayoutTag.Alias
    | LayoutTag.Class
    | LayoutTag.Union
    | LayoutTag.Record
    | LayoutTag.UnknownType -> TextTags.Class
    | LayoutTag.Delegate -> TextTags.Delegate
    | LayoutTag.Event -> TextTags.Event
    | LayoutTag.Field -> TextTags.Field
    | LayoutTag.Interface -> TextTags.Interface
    | LayoutTag.Struct -> TextTags.Struct
    | LayoutTag.Keyword -> TextTags.Keyword
    | LayoutTag.Local -> TextTags.Local
    | LayoutTag.Member
    | LayoutTag.ModuleBinding
    | LayoutTag.RecordField
    | LayoutTag.Property -> TextTags.Property
    | LayoutTag.Method -> TextTags.Method
    | LayoutTag.Namespace -> TextTags.Namespace
    | LayoutTag.Module -> TextTags.Module
    | LayoutTag.LineBreak -> TextTags.LineBreak
    | LayoutTag.Space -> TextTags.Space
    | LayoutTag.NumericLiteral -> TextTags.NumericLiteral
    | LayoutTag.Operator -> TextTags.Operator
    | LayoutTag.Parameter -> TextTags.Parameter
    | LayoutTag.TypeParameter -> TextTags.TypeParameter
    | LayoutTag.Punctuation -> TextTags.Punctuation
    | LayoutTag.StringLiteral -> TextTags.StringLiteral
    | LayoutTag.Text
    | LayoutTag.UnknownEntity -> TextTags.Text

    let CollectTaggedText (list: List<_>) (t:TaggedText) = list.Add(TaggedText(roslynTag t.Tag, t.Text))

    type VolatileBarrier() =
        [<VolatileField>]
        let mutable isStopped = false
        member __.Proceed = not isStopped
        member __.Stop() = isStopped <- true

    // This is like Async.StartAsTask, but
    //  1. if cancellation occurs we explicitly associate the cancellation with cancellationToken
    //  2. if exception occurs then set result to Unchecked.defaultof<_>, i.e. swallow exceptions
    //     and hope that Roslyn copes with the null
    let StartAsyncAsTask (cancellationToken: CancellationToken) computation =
        let tcs = new TaskCompletionSource<_>(TaskCreationOptions.None)
        let barrier = VolatileBarrier()
        let reg = cancellationToken.Register(fun _ -> if barrier.Proceed then tcs.TrySetCanceled(cancellationToken) |> ignore)
        let task = tcs.Task
        let disposeReg() = barrier.Stop(); if not task.IsCanceled then reg.Dispose()
        Async.StartWithContinuations(
                  async { do! Async.SwitchToThreadPool()
                          return! computation }, 
                  continuation=(fun result -> 
                      disposeReg()
                      tcs.TrySetResult(result) |> ignore
                  ), 
                  exceptionContinuation=(fun exn -> 
                      disposeReg()
                      match exn with 
                      | :? OperationCanceledException -> 
                          tcs.TrySetCanceled(cancellationToken)  |> ignore
                      | exn ->
                          System.Diagnostics.Trace.WriteLine("Visual F# Tools: exception swallowed and not passed to Roslyn: {0}", exn.Message)
                          let res = Unchecked.defaultof<_>
                          tcs.TrySetResult(res) |> ignore
                  ),
                  cancellationContinuation=(fun _oce -> 
                      disposeReg()
                      tcs.TrySetCanceled(cancellationToken) |> ignore
                  ),
                  cancellationToken=cancellationToken)
        task

    let StartAsyncUnitAsTask cancellationToken (computation:Async<unit>) = 
        StartAsyncAsTask cancellationToken computation  :> Task

    let private TheSupportedDiagnostics =
        // We are constructing our own descriptors at run-time. Compiler service is already doing error formatting and localization.
        let dummyDescriptors = 
           [| for i in 0 .. 10000 -> DiagnosticDescriptor(sprintf "FS%04d" i, String.Empty, String.Empty, String.Empty, DiagnosticSeverity.Error, true, null, null) |]
        ImmutableArray.Create<DiagnosticDescriptor>(dummyDescriptors)

    let SupportedDiagnostics() = TheSupportedDiagnostics

    let ConvertError(error: FSharpErrorInfo, location: Location) =
        // Normalize the error message into the same format that we will receive it from the compiler.
        // This ensures that IntelliSense and Compiler errors in the 'Error List' are de-duplicated.
        // (i.e the same error does not appear twice, where the only difference is the line endings.)
        let normalizedMessage = error.Message |> ErrorLogger.NormalizeErrorString |> ErrorLogger.NewlineifyErrorString

        let id = "FS" + error.ErrorNumber.ToString("0000")
        let emptyString = LocalizableString.op_Implicit("")
        let description = LocalizableString.op_Implicit(normalizedMessage)
        let severity = if error.Severity = FSharpErrorSeverity.Error then DiagnosticSeverity.Error else DiagnosticSeverity.Warning
        let customTags = 
            match error.ErrorNumber with
            | 1182 -> FSharpDiagnosticCustomTags.Unnecessary
            | _ -> null
        let descriptor = new DiagnosticDescriptor(id, emptyString, description, error.Subcategory, severity, true, emptyString, String.Empty, customTags)
        Diagnostic.Create(descriptor, location)


    let RangeToLocation (r: range, sourceText: SourceText, filePath: string) : Location =
        let linePositionSpan = LinePositionSpan(LinePosition(Line.toZ r.StartLine, r.StartColumn), LinePosition(Line.toZ r.EndLine, r.EndColumn))
        let textSpan = sourceText.Lines.GetTextSpan linePositionSpan
        Location.Create(filePath, textSpan, linePositionSpan)

    let StartAsyncSafe cancellationToken context computation =
        let computation =
            async {
                try
                    return! computation
                with e ->
                    logExceptionWithContext(e, context)
                    return Unchecked.defaultof<_>
            }
        Async.Start (computation, cancellationToken)

module internal OpenDeclarationHelper =
    /// <summary>
    /// Inserts open declaration into `SourceText`. 
    /// </summary>
    /// <param name="sourceText">SourceText.</param>
    /// <param name="ctx">Insertion context. Typically returned from tryGetInsertionContext</param>
    /// <param name="ns">Namespace to open.</param>
    let insertOpenDeclaration (sourceText: SourceText) (ctx: InsertContext) (ns: string) : SourceText * int =
        let mutable minPos = None

        let insert line lineStr (sourceText: SourceText) : SourceText =
            let ln = sourceText.Lines.[line]
            let pos = ln.Start
            minPos <- match minPos with None -> Some pos | Some oldPos -> Some (min oldPos pos)

            // find the line break characters on the previous line to use, Environment.NewLine should not be used
            // as it makes assumptions on the line endings in the source.
            let lineBreak = ln.Text.ToString(TextSpan(ln.End, ln.EndIncludingLineBreak - ln.End))

            sourceText.WithChanges(TextChange(TextSpan(pos, 0), lineStr + lineBreak))

        let getLineStr line = sourceText.Lines.[line].ToString().Trim()
        let pos = ParsedInput.adjustInsertionPoint getLineStr ctx
        let docLine = pos.Line - 1
        let lineStr = (String.replicate pos.Column " ") + "open " + ns
        let sourceText = sourceText |> insert docLine lineStr
        // if there's no a blank line between open declaration block and the rest of the code, we add one
        let sourceText = 
            if sourceText.Lines.[docLine + 1].ToString().Trim() <> "" then 
                sourceText |> insert (docLine + 1) ""
            else sourceText
        let sourceText =
            // for top level module we add a blank line between the module declaration and first open statement
            if (pos.Column = 0 || ctx.ScopeKind = ScopeKind.Namespace) && docLine > 0
                && not (sourceText.Lines.[docLine - 1].ToString().Trim().StartsWith "open") then
                    sourceText |> insert docLine ""
            else sourceText
        sourceText, minPos |> Option.defaultValue 0

