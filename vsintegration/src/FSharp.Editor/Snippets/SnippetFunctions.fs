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
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
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

    let private tryGetSpan (session: IVsExpansionSession) (getSpan: IVsExpansionSession -> VsTextSpan[] -> int) =
        match session with
        | null -> ValueNone
        | session ->
            let spans = Array.zeroCreate<VsTextSpan> 1

            if Com.Succeeded(getSpan session spans) then
                ValueSome spans[0]
            else
                ValueNone

    let tryGetSnippetSpan session =
        tryGetSpan session (fun session spans -> session.GetSnippetSpan spans)

    let tryGetFieldSpan session field =
        tryGetSpan session (fun session spans -> session.GetFieldSpan(field, spans))

    /// The expansion engine calls `IVsExpansionFunction` synchronously on the UI thread while the
    /// session is live, so there is nowhere to await.
    let runSynchronously millisecondsTimeout (work: CancellableTask<'T voption>) =
        use cts = new CancellationTokenSource(millisecondsTimeout: int)

        try
            ThreadHelper.JoinableTaskFactory.Run(fun () -> work cts.Token)
        with
        | :? OperationCanceledException when cts.IsCancellationRequested -> ValueNone
        // This runs inside a COM callback, so an exception that escapes unwinds into native Visual
        // Studio code.
        | e ->
            FSharpOutputPane.logException e
            ValueNone

    let tryGetDocument (subjectBuffer: ITextBuffer) =
        match subjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges() with
        | null -> ValueNone
        | document when document.Project.IsFSharp -> ValueSome document
        | _ -> ValueNone

    let tryGetContainingTypeName (document: Document) pos =
        cancellableTask {
            let! parseResults = document.GetFSharpParseResultsAsync userOpName

            return
                (pos, parseResults.ParseTree)
                ||> ParsedInput.tryPickLast (fun _ node ->
                    match node with
                    // The walk offers every type of a `type … and …` group, and the nearest declaration
                    // to the left when none contains `pos`.
                    | SyntaxNode.SynTypeDefn(SynTypeDefn(typeInfo = typeInfo; range = m))
                    | SyntaxNode.SynTypeDefnSig(SynTypeDefnSig(typeInfo = typeInfo; range = m)) when Range.rangeContainsPos m pos ->
                        typeInfo.LongIdent |> List.tryLast |> Option.map _.idText
                    | _ -> None)
                |> ValueOption.ofOption
        }

    let private necessaryQualifier (checkResults: FSharpCheckFileResults) position (entity: FSharpEntity) (symbol: FSharpSymbol) =
        let path =
            match entity.TryGetFullDisplayName() with
            | Some fullName -> List.ofArray (fullName.Split '.')
            | None -> [ entity.DisplayName ]

        let rec widen remaining qualifier =
            if checkResults.IsRelativeNameResolvableFromSymbol(position, qualifier, symbol) then
                qualifier
            else
                match remaining with
                | [] -> qualifier
                | next :: rest -> widen rest (next :: qualifier)

        widen (List.rev path) []

    let private qualifierPrefix checkResults position entity symbol =
        match necessaryQualifier checkResults position entity symbol with
        | [] -> ""
        | qualifier -> String.Join(".", qualifier) + "."

    /// A case is declared as an identifier, so a name that is not a plain one is quoted. `DisplayName` would
    /// spell a case named `op_Nil` as the operator it encodes.
    let private patternNameOf (name: string) =
        if PrettyNaming.IsIdentifierName name then
            name
        else
            $"``%s{name}``"

    let private matchRulesFor checkResults position (entity: FSharpEntity) =
        if entity.IsFSharpUnion then
            let prefix =
                match Seq.tryHeadV entity.UnionCases with
                | ValueSome first -> qualifierPrefix checkResults position entity first
                | ValueNone -> ""

            entity.UnionCases
            |> Seq.map (fun case ->
                if case.HasFields then
                    $"| %s{prefix}%s{patternNameOf case.Name} _ -> ()"
                else
                    $"| %s{prefix}%s{patternNameOf case.Name} -> ()")
        elif entity.IsEnum then
            let literals =
                entity.FSharpFields |> Seq.filter (fun field -> field.LiteralValue.IsSome)

            let prefix =
                match Seq.tryHeadV literals with
                | ValueSome first -> qualifierPrefix checkResults position entity first
                | ValueNone -> ""

            seq {
                for field in literals do
                    $"| %s{prefix}%s{patternNameOf field.Name} -> ()"

                // An enum value need not be one of the declared literals, so the wildcard is not optional.
                "| _ -> ()"
            }
        else
            Seq.empty

    let tryGetMatchRulesAt (document: Document) (range: range) =
        cancellableTask {
            let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync userOpName
            let! ct = CancellableTask.getCancellationToken ()
            let! sourceText = document.GetTextAsync ct

            let rules =
                match checkResults.TryGetCapturedType range |> Option.map _.StripAbbreviations() with
                | Some fsharpType when fsharpType.HasTypeDefinition -> matchRulesFor checkResults range.Start fsharpType.TypeDefinition
                | _ -> Seq.empty

            let lineBreak =
                sourceText.LineBreakAt(RoslynHelpers.FSharpRangeToTextSpan(sourceText, range).End)

            return
                match String.Join(lineBreak, rules) with
                | "" -> ValueNone
                | rules -> ValueSome rules
        }

    let tryGetMatchRules (document: Document) (span: VsTextSpan) =
        Range.mkRange document.FilePath (Position.fromZ span.iStartLine span.iStartIndex) (Position.fromZ span.iEndLine span.iEndIndex)
        |> tryGetMatchRulesAt document

[<AbstractClass>]
type internal FSharpSnippetFunction(getSession: unit -> IVsExpansionSession, arguments: string[]) =

    /// The engine can build a function before it opens the session, so this is read per call.
    member _.Session = getSession ()

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

type internal SnippetFunctionClassName(getSession, subjectBuffer: ITextBuffer, arguments) =
    inherit FSharpSnippetFunction(getSession, arguments)

    override this.TryGetValue() =
        match tryGetDocument subjectBuffer, tryGetSnippetSpan this.Session with
        | ValueSome document, ValueSome span ->
            runSynchronously parseTimeout (tryGetContainingTypeName document (Position.fromZ span.iStartLine span.iStartIndex))
        | _ -> ValueNone

type internal SnippetFunctionGenerateMatchCases(getSession, subjectBuffer: ITextBuffer, arguments: string[]) =
    inherit FSharpSnippetFunction(getSession, arguments)

    let matchedField =
        match arguments with
        | [| argument |] when argument.StartsWith("$", StringComparison.Ordinal) -> ValueSome(argument.Trim '$')
        | _ -> ValueNone

    override this.TryGetValue() =
        match tryGetDocument subjectBuffer, matchedField |> ValueOption.bind (tryGetFieldSpan this.Session) with
        | ValueSome document, ValueSome span ->
            runSynchronously document.Project.FSharpTimeUntilStaleCompletion (tryGetMatchRules document span)
        | _ -> ValueNone
