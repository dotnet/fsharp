// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor.DebugHelpers
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.TextManager.Interop

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

open CancellableTasks

type internal VsTextSpan = Microsoft.VisualStudio.TextManager.Interop.TextSpan

[<AutoOpen>]
module internal SnippetFunctionHelpers =

    [<Literal>]
    let private userOpName = "FSharpSnippetFunction"

    /// Long enough for a warm parse, short enough that a cold project degrades instead of hanging.
    [<Literal>]
    let parseTimeout = 2000

    let positionOf (snapshot: ITextSnapshot) line index =
        snapshot.GetLineFromLineNumber(line).Start.Position + index

    // The engine can build a snippet function before it opens the session, so both of these have to
    // tolerate not having one yet.
    let tryGetSnippetSpan (session: IVsExpansionSession) =
        match session with
        | null -> ValueNone
        | session ->
            let spans = Array.zeroCreate<VsTextSpan> 1

            if Com.Succeeded(session.GetSnippetSpan spans) then
                ValueSome spans[0]
            else
                ValueNone

    let tryGetFieldSpan (session: IVsExpansionSession) field =
        match session with
        | null -> ValueNone
        | session ->
            let spans = Array.zeroCreate<VsTextSpan> 1

            if Com.Succeeded(session.GetFieldSpan(field, spans)) then
                ValueSome spans[0]
            else
                ValueNone

    /// The expansion engine calls `IVsExpansionFunction` synchronously on the UI thread while the
    /// session is live, so there is nowhere to await. `JoinableTaskFactory.Run` is the same blocking
    /// bridge `FSharpGraphProvider` uses for the Code Map action handler; the timeout keeps a cold
    /// project from turning that block into a hang, at the cost of falling back to the literal's
    /// declared default.
    let runSynchronously millisecondsTimeout (work: CancellableTask<'T voption>) =
        use cts = new CancellationTokenSource(millisecondsTimeout: int)

        try
            ThreadHelper.JoinableTaskFactory.Run(fun () -> work cts.Token)
        with
        | :? OperationCanceledException when cts.IsCancellationRequested -> ValueNone
        // This runs inside a COM callback, so an exception that escapes unwinds into native Visual
        // Studio code. A snippet field is not worth taking the IDE down for.
        | e ->
            FSharpOutputPane.logException e
            ValueNone

    let tryGetDocument (subjectBuffer: ITextBuffer) =
        match subjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges() with
        | null -> ValueNone
        | document when document.Project.IsFSharp -> ValueSome document
        | _ -> ValueNone

    /// The name of the innermost type declaration whose body contains `position`.
    let tryGetContainingTypeName (document: Document) position =
        cancellableTask {
            let! parseResults = document.GetFSharpParseResultsAsync userOpName
            let! ct = CancellableTask.getCancellationToken ()
            let! sourceText = document.GetTextAsync ct

            let line = sourceText.Lines.GetLineFromPosition position
            let caret = Position.mkPos (line.LineNumber + 1) (position - line.Start)

            let innermost =
                (Navigation.getNavigation parseResults.ParseTree).Declarations
                |> Array.fold
                    (fun innermost topLevel ->
                        let declaration = topLevel.Declaration

                        if
                            declaration.Kind <> NavigationItemKind.Type
                            || not (Range.rangeContainsPos declaration.BodyRange caret)
                        then
                            innermost
                        else
                            match innermost with
                            | ValueSome(previous: NavigationItem) when previous.BodyRange.StartLine >= declaration.BodyRange.StartLine ->
                                innermost
                            | _ -> ValueSome declaration)
                    ValueNone

            return innermost |> ValueOption.map _.LogicalName
        }

    /// The type an expression evaluates to: for a call, what is left once its arguments are applied.
    let rec private resultTypeOf (fsharpType: FSharpType) =
        if fsharpType.IsFunctionType then
            resultTypeOf fsharpType.GenericArguments[1]
        else
            fsharpType.StripAbbreviations()

    /// Lazy on purpose: `String.Join` is the one consumer and it materializes the text directly,
    /// so no intermediate collection of rules is ever built.
    let private matchRulesFor (entity: FSharpEntity) =
        if entity.IsFSharpUnion then
            entity.UnionCases
            |> Seq.map (fun case ->
                if case.HasFields then
                    $"| %s{case.Name} _ -> ()"
                else
                    $"| %s{case.Name} -> ()")
        elif entity.IsEnum then
            seq {
                for field in entity.FSharpFields do
                    if field.LiteralValue.IsSome then
                        $"| %s{entity.DisplayName}.%s{field.Name} -> ()"

                // An enum value need not be one of the declared literals, so the wildcard is not optional.
                "| _ -> ()"
            }
        else
            Seq.empty

    let private matchRulesForUse (symbolUse: FSharpSymbolUse) =
        match symbolUse.Symbol with
        | :? FSharpMemberOrFunctionOrValue as value ->
            let resultType = resultTypeOf value.FullType

            if resultType.HasTypeDefinition then
                matchRulesFor resultType.TypeDefinition
            else
                Seq.empty
        | _ -> Seq.empty

    /// The match rules covering the union or enum at `position`, or ValueNone for anything else.
    let tryGetMatchRules (document: Document) position =
        cancellableTask {
            let! lexerSymbol = document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, userOpName)
            let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync userOpName
            let! ct = CancellableTask.getCancellationToken ()
            let! sourceText = document.GetTextAsync ct

            let line = sourceText.Lines.GetLineFromPosition position

            let rules =
                lexerSymbol
                |> ValueOption.ofOption
                |> ValueOption.bind (fun symbol ->
                    checkResults.GetSymbolUseAtLocation(
                        line.LineNumber + 1,
                        symbol.Ident.idRange.EndColumn,
                        line.ToString(),
                        symbol.FullIsland
                    )
                    |> ValueOption.ofOption)
                |> ValueOption.map matchRulesForUse
                |> ValueOption.defaultValue Seq.empty

            return
                match String.Join(sourceText.LineBreakAt position, rules) with
                | "" -> ValueNone
                | rules -> ValueSome rules
        }

/// One `<Function>` declared by a snippet literal. `arguments` are the raw `$field$` references the
/// snippet passed, which is what tells us whether a field edit invalidates our value.
[<AbstractClass>]
type internal FSharpSnippetFunction(getSession: unit -> IVsExpansionSession, subjectBuffer: ITextBuffer, arguments: string[]) =

    /// The engine can build a function before it opens the session, so this is read per call.
    member _.Session = getSession ()
    member _.SubjectBuffer = subjectBuffer

    abstract TryGetValue: unit -> string voption

    interface IVsExpansionFunction with

        member _.GetFunctionType(pFuncType: byref<uint32>) =
            pFuncType <- uint _ExpansionFunctionType.eft_Value
            VSConstants.S_OK

        member _.GetListCount(iCount: byref<int>) =
            iCount <- 0
            VSConstants.S_OK

        member _.GetListText(_index, pbstrText: byref<string>) =
            pbstrText <- null
            VSConstants.E_NOTIMPL

        member this.GetDefaultValue(bstrValue: byref<string>, fHasDefaultValue: byref<int>) =
            match this.TryGetValue() with
            | ValueSome value ->
                bstrValue <- value
                fHasDefaultValue <- 1
            | ValueNone ->
                bstrValue <- ""
                fHasDefaultValue <- 0

            VSConstants.S_OK

        member this.GetCurrentValue(bstrValue: byref<string>, fHasCurrentValue: byref<int>) =
            (this :> IVsExpansionFunction).GetDefaultValue(&bstrValue, &fHasCurrentValue)

        member _.FieldChanged(bstrField: string, fRequeryFunction: byref<int>) =
            fRequeryFunction <-
                if arguments |> Array.contains $"$%s{bstrField}$" then
                    1
                else
                    0

            VSConstants.S_OK

        member _.ReleaseFunction() = VSConstants.S_OK

/// `ClassName()` — the F# counterpart of the C# snippet function of the same name.
type internal SnippetFunctionClassName(getSession, subjectBuffer: ITextBuffer, arguments) =
    inherit FSharpSnippetFunction(getSession, subjectBuffer, arguments)

    override this.TryGetValue() =
        match tryGetDocument subjectBuffer, tryGetSnippetSpan this.Session with
        | ValueSome document, ValueSome span ->
            let position =
                positionOf subjectBuffer.CurrentSnapshot span.iStartLine span.iStartIndex

            // Parse results are cached per document version, so the timeout only bites on the first
            // parse of a freshly opened file.
            runSynchronously parseTimeout (tryGetContainingTypeName document position)
        | _ -> ValueNone

/// `GenerateMatchCases($field$)` — the F# counterpart of C#'s `GenerateSwitchCases`, covering
/// discriminated unions as well as enums.
type internal SnippetFunctionGenerateMatchCases(getSession, subjectBuffer: ITextBuffer, arguments: string[]) =
    inherit FSharpSnippetFunction(getSession, subjectBuffer, arguments)

    /// The single argument names the field holding the expression to match on, delimited as `$name$`.
    let matchedField =
        match arguments with
        | [| argument |] when argument.StartsWith("$", StringComparison.Ordinal) -> ValueSome(argument.Trim '$')
        | _ -> ValueNone

    override this.TryGetValue() =
        match tryGetDocument subjectBuffer, matchedField |> ValueOption.bind (tryGetFieldSpan this.Session) with
        | ValueSome document, ValueSome span ->
            let position = positionOf subjectBuffer.CurrentSnapshot span.iEndLine span.iEndIndex

            // Resolving the user's expression needs a check of the text they just typed, so there is
            // no cached answer to fall back on - only the literal's declared default.
            runSynchronously document.Project.FSharpTimeUntilStaleCompletion (tryGetMatchRules document position)
        | _ -> ValueNone
