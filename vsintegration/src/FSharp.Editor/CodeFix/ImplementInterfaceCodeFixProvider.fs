// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

[<NoEquality; NoComparison>]
type internal InterfaceState =
    { InterfaceData: InterfaceData 
      EndPosOfWith: pos option
      Tokens: FSharpTokenInfo list }

[<ExportCodeFixProvider(FSharpCommonConstants.FSharpLanguageName, Name = "ImplementInterface"); Shared>]
type internal FSharpImplementInterfaceCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: ProjectInfoManager
    ) =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = ["FS0366"]
    let checker = checkerProvider.Checker

    let queryInterfaceState (point: LinePosition) (ast: Ast.ParsedInput) =
        async {
            let line = point.Line
            let column = point.Character
            let pos = Pos.fromZ line column
            match InterfaceStubGenerator.tryFindInterfaceDeclaration pos ast with
            | None -> return None
            | Some iface ->
                let tokens = []
                let endPosOfWidth =
                    tokens 
                    |> List.tryPick (fun (t: FSharpTokenInfo) ->
                            if t.CharClass = FSharpTokenCharKind.Keyword && t.LeftColumn >= column && t.TokenName = "WITH" then
                                Some (Pos.fromZ line (t.RightColumn + 1))
                            else None)
                return Some { InterfaceData = iface; EndPosOfWith = endPosOfWidth; Tokens = tokens } 
        }
        
    let getLineIdent (lineStr: string) =
        lineStr.Length - lineStr.TrimStart(' ').Length

    let inferStartColumn indentSize state (sourceText: SourceText) = 
        match InterfaceStubGenerator.getMemberNameAndRanges state.InterfaceData with
        | (_, range) :: _ ->
            let lineStr = sourceText.Lines.[range.StartLine-1].ToString()
            getLineIdent lineStr
        | [] ->
            match state.InterfaceData with
            | InterfaceData.Interface _ as iface ->
                // 'interface ISomething with' is often in a new line, we use the indentation of that line
                let lineStr = sourceText.Lines.[iface.Range.StartLine-1].ToString()
                getLineIdent lineStr + indentSize
            | InterfaceData.ObjExpr _ as iface ->
                state.Tokens 
                |> List.tryPick (fun (t: FSharpTokenInfo) ->
                            if t.CharClass = FSharpTokenCharKind.Keyword && t.TokenName = "NEW" then
                                Some (t.LeftColumn + indentSize)
                            else None)
                // There is no reference point, we indent the content at the start column of the interface
                |> Option.defaultValue iface.Range.StartColumn

    let handleImplementInterface (sourceText: SourceText) state displayContext implementedMemberSignatures entity verboseMode =         
        let indentSize = 4
        let startColumn = inferStartColumn indentSize state sourceText
        let objectIdentifier = "this"
        let defaultBody = "raise (System.NotImplementedException())"
        let typeParams = state.InterfaceData.TypeParameters
        let stub = InterfaceStubGenerator.formatInterface 
                       startColumn indentSize typeParams objectIdentifier defaultBody
                       displayContext implementedMemberSignatures entity verboseMode
        match state.EndPosOfWith with
        | Some pos -> 
            let currentPos = sourceText.Lines.[pos.Line-1].Start + pos.Column
            TextChange(TextSpan(currentPos, 0), stub)
        | None ->
            let range = state.InterfaceData.Range
            let currentPos = sourceText.Lines.[range.EndLine-1].Start + range.EndColumn
            TextChange(TextSpan(currentPos, 0), " with" + stub)

    let createCodeFix (title: string, context: CodeFixContext, textChange: TextChange) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                async {
                    let! sourceText = context.Document.GetTextAsync() |> Async.AwaitTask
                    return context.Document.WithText(sourceText.WithChanges(textChange))
                } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)),
            title)

    let getSuggestions (context, sourceText, state: InterfaceState, displayContext, entity, results: FSharpCheckFileResults) =
        if InterfaceStubGenerator.hasNoInterfaceMember entity then []
        else
            let membersAndRanges = InterfaceStubGenerator.getMemberNameAndRanges state.InterfaceData
            let interfaceMembers = InterfaceStubGenerator.getInterfaceMembers entity
            let hasTypeCheckError = results.Errors |> Array.exists (fun e -> e.Severity = FSharpErrorSeverity.Error)                
            // This comparison is a bit expensive
            if hasTypeCheckError && List.length membersAndRanges <> Seq.length interfaceMembers then
                let implementedMemberSignatures = set []
                [ createCodeFix("Implement Interface Explicitly", context, handleImplementInterface sourceText state displayContext implementedMemberSignatures entity false)
                  createCodeFix("Implement Interface Explicitly (verbose)", context, handleImplementInterface sourceText state displayContext implementedMemberSignatures entity true) ]
            else []
            
    override __.FixableDiagnosticIds = fixableDiagnosticIds.ToImmutableArray()

    override __.RegisterCodeFixesAsync context : Task =
        async {
            match projectInfoManager.TryGetOptionsForEditingDocumentOrProject context.Document with 
            | Some options ->
                let! sourceText = context.Document.GetTextAsync(context.CancellationToken) |> Async.AwaitTask
                let! textVersion = context.Document.GetTextVersionAsync(context.CancellationToken) |> Async.AwaitTask
                let! parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(context.Document.FilePath, textVersion.GetHashCode(), sourceText.ToString(), options)
                match parseResults.ParseTree, checkFileAnswer with
                | None, _
                | _, FSharpCheckFileAnswer.Aborted -> ()
                | Some parsedInput, FSharpCheckFileAnswer.Succeeded checkFileResults ->
                    let textLinePos = sourceText.Lines.GetLinePosition context.Span.End
                    let textLine = sourceText.Lines.[textLinePos.Line]
                    let defines = CompilerEnvironment.GetCompilationDefinesForEditing(context.Document.FilePath, options.OtherOptions |> Seq.toList)
                    // On long identifiers (e.g. System.IDisposable), we would like to inspect the last identifier
                    let symbol = CommonHelpers.getSymbolAtPosition(context.Document.Id, sourceText, context.Span.End, context.Document.FilePath, defines, SymbolLookupKind.Fuzzy)
                    match symbol with
                    | Some symbol ->
                        match symbol.Kind with
                        | LexerSymbolKind.Ident ->
                            let! interfaceState = queryInterfaceState textLinePos parsedInput
                            match interfaceState with
                            | Some state ->                                
                                let fcsTextLineNumber = textLinePos.Line + 1
                                let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.RightColumn, textLine.ToString(), [symbol.Text])
                                symbolUse
                                |> Option.bind (fun symbolUse ->
                                    match symbolUse.Symbol with
                                    | :? FSharpEntity as entity -> 
                                        if InterfaceStubGenerator.isInterface entity && entity.DisplayName = symbol.Text then
                                            Some (entity, symbolUse.DisplayContext)
                                        else None
                                    | _ -> None)
                                |> Option.iter (fun (entity, displayContext) ->
                                    let diagnostics = (context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id)).ToImmutableArray()
                                    getSuggestions (context, sourceText, state, displayContext, entity, checkFileResults)
                                    |> List.iter (fun codeFix -> context.RegisterCodeFix(codeFix, diagnostics)))
                            | None -> ()
                        | _ -> ()
                    | None -> ()
            | None -> ()
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
