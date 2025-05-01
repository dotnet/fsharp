// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization

open CancellableTasks

[<NoEquality; NoComparison>]
type internal InterfaceState =
    {
        InterfaceData: InterfaceData
        EndPosOfWith: pos option
        AppendBracketAt: int option
        Tokens: Tokenizer.SavedTokenInfo[]
    }

// state machine not statically compilable
// TODO: rewrite token arithmetics properly here
#nowarn "3511"

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ImplementInterface); Shared>]
type internal ImplementInterfaceCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    let queryInterfaceState appendBracketAt (pos: pos) (tokens: Tokenizer.SavedTokenInfo[]) (ast: ParsedInput) =
        let line = pos.Line - 1

        InterfaceStubGenerator.TryFindInterfaceDeclaration pos ast
        |> Option.map (fun iface ->
            let endPosOfWidth =
                tokens
                |> Array.tryPick (fun (t: Tokenizer.SavedTokenInfo) ->
                    if t.Tag = FSharpTokenTag.WITH || t.Tag = FSharpTokenTag.OWITH then
                        Some(Position.fromZ line (t.RightColumn + 1))
                    else
                        None)

            let appendBracketAt =
                match iface, appendBracketAt with
                | InterfaceData.ObjExpr _, Some _ -> appendBracketAt
                | _ -> None

            {
                InterfaceData = iface
                EndPosOfWith = endPosOfWidth
                AppendBracketAt = appendBracketAt
                Tokens = tokens
            })

    let getLineIdent (lineStr: string) =
        lineStr.Length - lineStr.TrimStart(' ').Length

    let inferStartColumn indentSize state (sourceText: SourceText) =
        match InterfaceStubGenerator.GetMemberNameAndRanges state.InterfaceData with
        | (_, range) :: _ ->
            let lineStr = sourceText.Lines[range.StartLine - 1].ToString()
            getLineIdent lineStr
        | [] ->
            match state.InterfaceData with
            | InterfaceData.Interface _ as iface ->
                // 'interface ISomething with' is often in a new line, we use the indentation of that line
                let lineStr = sourceText.Lines[iface.Range.StartLine - 1].ToString()
                getLineIdent lineStr + indentSize
            | InterfaceData.ObjExpr _ as iface ->
                state.Tokens
                |> Array.tryPick (fun (t: Tokenizer.SavedTokenInfo) ->
                    if t.Tag = FSharpTokenTag.NEW then
                        Some(t.LeftColumn + indentSize)
                    else
                        None)
                // There is no reference point, we indent the content at the start column of the interface
                |> Option.defaultValue iface.Range.StartColumn

    let getChanges (sourceText: SourceText) state displayContext implementedMemberSignatures entity indentSize verboseMode =
        let startColumn = inferStartColumn indentSize state sourceText
        let objectIdentifier = "this"
        let defaultBody = "raise (System.NotImplementedException())"
        let typeParams = state.InterfaceData.TypeParameters

        let stub =
            let stub =
                InterfaceStubGenerator.FormatInterface
                    startColumn
                    indentSize
                    typeParams
                    objectIdentifier
                    defaultBody
                    displayContext
                    implementedMemberSignatures
                    entity
                    verboseMode

            stub.TrimEnd(Environment.NewLine.ToCharArray())

        let stubChange =
            match state.EndPosOfWith with
            | Some pos ->
                let currentPos = sourceText.Lines[pos.Line - 1].Start + pos.Column
                TextChange(TextSpan(currentPos, 0), stub)
            | None ->
                let range = state.InterfaceData.Range
                let currentPos = sourceText.Lines[range.EndLine - 1].Start + range.EndColumn
                TextChange(TextSpan(currentPos, 0), " with" + stub)

        match state.AppendBracketAt with
        | Some index -> [ stubChange; TextChange(TextSpan(index, 0), " }") ]
        | None -> [ stubChange ]

    let getSuggestions
        (sourceText: SourceText, results: FSharpCheckFileResults, state: InterfaceState, displayContext, entity, indentSize)
        =
        if InterfaceStubGenerator.HasNoInterfaceMember entity then
            CancellableTask.singleton Seq.empty
        else
            let membersAndRanges =
                InterfaceStubGenerator.GetMemberNameAndRanges state.InterfaceData

            let interfaceMembers = InterfaceStubGenerator.GetInterfaceMembers entity

            let hasTypeCheckError =
                results.Diagnostics
                |> Array.exists (fun e -> e.Severity = FSharpDiagnosticSeverity.Error)
            // This comparison is a bit expensive
            if hasTypeCheckError && List.length membersAndRanges <> Seq.length interfaceMembers then

                let getMemberByLocation (name, range: range) =
                    let lineStr = sourceText.Lines[range.EndLine - 1].ToString()
                    results.GetSymbolUseAtLocation(range.EndLine, range.EndColumn, lineStr, [ name ])

                cancellableTask {
                    let! implementedMemberSignatures =
                        InterfaceStubGenerator.GetImplementedMemberSignatures getMemberByLocation displayContext state.InterfaceData

                    let getCodeFix title verboseMode =
                        let changes =
                            getChanges sourceText state displayContext implementedMemberSignatures entity indentSize verboseMode

                        {
                            Name = CodeFix.ImplementInterface
                            Message = title
                            Changes = changes
                        }

                    return
                        seq {
                            getCodeFix (SR.ImplementInterface()) true
                            getCodeFix (SR.ImplementInterfaceWithoutTypeAnnotation()) false
                        }
                }

            else
                CancellableTask.singleton Seq.empty

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0366"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFixes this

    interface IFSharpMultiCodeFixProvider with
        member _.GetCodeFixesAsync context =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCancellationToken ()

                let! parseResults, checkFileResults =
                    context.Document.GetFSharpParseAndCheckResultsAsync(nameof ImplementInterfaceCodeFixProvider)

                let! sourceText = context.GetSourceTextAsync()

                let textLine = sourceText.Lines.GetLineFromPosition context.Span.Start

                let! _, _, parsingOptions, _ = context.Document.GetFSharpCompilationOptionsAsync(nameof ImplementInterfaceCodeFixProvider)

                let defines = CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions
                let langVersionOpt = Some parsingOptions.LangVersionText
                // Notice that context.Span doesn't return reliable ranges to find tokens at exact positions.
                // That's why we tokenize the line and try to find the last successive identifier token
                let tokens =
                    Tokenizer.tokenizeLine (
                        context.Document.Id,
                        sourceText,
                        context.Span.Start,
                        context.Document.FilePath,
                        defines,
                        langVersionOpt,
                        parsingOptions.StrictIndentation,
                        cancellationToken
                    )

                let startLeftColumn = context.Span.Start - textLine.Start

                let rec tryFindIdentifierToken acc i =
                    if i >= tokens.Length then
                        acc
                    else
                        match tokens[i] with
                        | t when t.LeftColumn < startLeftColumn ->
                            // Skip all the tokens starting before the context
                            tryFindIdentifierToken acc (i + 1)
                        | t when t.Tag = FSharpTokenTag.Identifier -> tryFindIdentifierToken (Some t) (i + 1)
                        | t when t.Tag = FSharpTokenTag.DOT || Option.isNone acc -> tryFindIdentifierToken acc (i + 1)
                        | _ -> acc

                let token = tryFindIdentifierToken None 0

                match token with
                | None -> return Seq.empty
                | Some token ->
                    let fixupPosition = textLine.Start + token.RightColumn
                    let interfacePos = Position.fromZ textLine.LineNumber token.RightColumn
                    // We rely on the observation that the lastChar of the context should be '}' if that character is present
                    let appendBracketAt =
                        match sourceText[context.Span.End - 1] with
                        | '}' -> None
                        | _ -> Some context.Span.End

                    let interfaceState =
                        queryInterfaceState appendBracketAt interfacePos tokens parseResults.ParseTree

                    match interfaceState with
                    | None -> return Seq.empty
                    | Some interfaceState ->
                        let symbol =
                            Tokenizer.getSymbolAtPosition (
                                context.Document.Id,
                                sourceText,
                                fixupPosition,
                                context.Document.FilePath,
                                defines,
                                SymbolLookupKind.Greedy,
                                false,
                                false,
                                langVersionOpt,
                                parsingOptions.StrictIndentation,
                                cancellationToken
                            )

                        match symbol with
                        | None -> return Seq.empty
                        | Some symbol ->
                            let fcsTextLineNumber = textLine.LineNumber + 1
                            let lineContents = textLine.ToString()
                            let! options = context.Document.GetOptionsAsync(cancellationToken)

                            let tabSize =
                                options.GetOption(FormattingOptions.TabSize, FSharpConstants.FSharpLanguageName)

                            let symbolUse =
                                checkFileResults.GetSymbolUseAtLocation(
                                    fcsTextLineNumber,
                                    symbol.Ident.idRange.EndColumn,
                                    lineContents,
                                    symbol.FullIsland
                                )

                            match symbolUse with
                            | None -> return Seq.empty
                            | Some symbolUse ->
                                match symbolUse.Symbol with
                                | :? FSharpEntity as entity when
                                    // Things get complicated with interface inheritance: https://github.com/dotnet/fsharp/issues/5813
                                    // With enough enthusiasm this probably can be handled though,
                                    // in that case change the check to `InterfaceStubGenerator.IsInterface entity`
                                    entity.AllInterfaces.Count = 1
                                    ->

                                    return!
                                        getSuggestions (
                                            sourceText,
                                            checkFileResults,
                                            interfaceState,
                                            symbolUse.DisplayContext,
                                            entity,
                                            tabSize
                                        )
                                | _ -> return Seq.empty
            }
