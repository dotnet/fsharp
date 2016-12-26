// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Formatting
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

    let queryInterfaceState (pos: pos) tokens (ast: Ast.ParsedInput) =
        async {
            let line = pos.Line - 1
            let column = pos.Column
            match InterfaceStubGenerator.tryFindInterfaceDeclaration pos ast with
            | None -> return None
            | Some iface ->
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

    let handleImplementInterface (sourceText: SourceText) state displayContext implementedMemberSignatures entity indentSize verboseMode = 
        let startColumn = inferStartColumn indentSize state sourceText
        let objectIdentifier = "this"
        let defaultBody = "raise (System.NotImplementedException())"
        let typeParams = state.InterfaceData.TypeParameters
        let stub = 
            let stub = InterfaceStubGenerator.formatInterface 
                           startColumn indentSize typeParams objectIdentifier defaultBody
                           displayContext implementedMemberSignatures entity verboseMode
            stub.TrimEnd(Environment.NewLine.ToCharArray())
        match state.EndPosOfWith with
        | Some pos -> 
            let currentPos = sourceText.Lines.[pos.Line-1].Start + pos.Column
            TextChange(TextSpan(currentPos, 0), stub)
        | None ->
            let range = state.InterfaceData.Range
            let currentPos = sourceText.Lines.[range.EndLine-1].Start + range.EndColumn
            TextChange(TextSpan(currentPos, 0), " with" + stub)
            
    let getSuggestions (context: CodeFixContext, results: FSharpCheckFileResults, state: InterfaceState, displayContext, entity, indentSize) =
        if InterfaceStubGenerator.hasNoInterfaceMember entity then 
            ()
        else
            let membersAndRanges = InterfaceStubGenerator.getMemberNameAndRanges state.InterfaceData
            let interfaceMembers = InterfaceStubGenerator.getInterfaceMembers entity
            let hasTypeCheckError = results.Errors |> Array.exists (fun e -> e.Severity = FSharpErrorSeverity.Error)                
            // This comparison is a bit expensive
            if hasTypeCheckError && List.length membersAndRanges <> Seq.length interfaceMembers then    
                let diagnostics = (context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id)).ToImmutableArray()            
                let registerCodeFix title verboseMode =
                    let codeAction =
                        CodeAction.Create(
                            title,
                            (fun (cancellationToken: CancellationToken) ->
                                async {
                                    let! sourceText = context.Document.GetTextAsync() |> Async.AwaitTask
                                    let getMemberByLocation(name, range: range) =
                                        let lineStr = sourceText.Lines.[range.EndLine-1].ToString()
                                        results.GetSymbolUseAtLocation(range.EndLine, range.EndColumn, lineStr, [name])
                                    let! implementedMemberSignatures =
                                        InterfaceStubGenerator.getImplementedMemberSignatures getMemberByLocation displayContext state.InterfaceData    
                                    let textChange = handleImplementInterface sourceText state displayContext implementedMemberSignatures entity indentSize verboseMode
                                    return context.Document.WithText(sourceText.WithChanges textChange)
                                } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)),
                            title)                
                    context.RegisterCodeFix(codeAction, diagnostics)

                registerCodeFix "Implement Interface Explicitly" false
                registerCodeFix "Implement Interface Explicitly (verbose)" true
            else 
                ()
            
    override __.FixableDiagnosticIds = fixableDiagnosticIds.ToImmutableArray()

    override __.RegisterCodeFixesAsync context : Task =
        async {
            match projectInfoManager.TryGetOptionsForEditingDocumentOrProject context.Document with 
            | Some options ->
                let cancellationToken = context.CancellationToken
                let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                let! textVersion = context.Document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                let! parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(context.Document.FilePath, textVersion.GetHashCode(), sourceText.ToString(), options)
                match parseResults.ParseTree, checkFileAnswer with
                | None, _
                | _, FSharpCheckFileAnswer.Aborted -> ()
                | Some parsedInput, FSharpCheckFileAnswer.Succeeded checkFileResults ->
                    let textLine = sourceText.Lines.GetLineFromPosition context.Span.Start
                    let defines = CompilerEnvironment.GetCompilationDefinesForEditing(context.Document.FilePath, options.OtherOptions |> Seq.toList)
                    // Notice that context.Span doesn't return reliable ranges to find tokens at exact positions.
                    // That's why we tokenize the line and try to find the last successive identifier token
                    let tokens = CommonHelpers.tokenizeLine(context.Document.Id, sourceText, context.Span.Start, context.Document.FilePath, defines)
                    let rec tryFindIdentifierToken acc tokens =
                       match tokens with
                       | t :: remainingTokens when t.Tag = FSharpTokenTag.Identifier ->
                           tryFindIdentifierToken (Some t) remainingTokens
                       | t :: remainingTokens when t.Tag = FSharpTokenTag.DOT || Option.isNone acc ->
                           tryFindIdentifierToken acc remainingTokens
                       | _ :: _ 
                       | [] -> acc
                    match tryFindIdentifierToken None tokens with
                    | Some token ->
                        let fixupPosition = textLine.Start + token.RightColumn
                        let interfacePos = Pos.fromZ textLine.LineNumber token.RightColumn
                        let! interfaceState = queryInterfaceState interfacePos tokens parsedInput                        
                        let symbol = CommonHelpers.getSymbolAtPosition(context.Document.Id, sourceText, fixupPosition, context.Document.FilePath, defines, SymbolLookupKind.Fuzzy)
                        match interfaceState, symbol with
                        | Some state, Some symbol ->                                
                            let fcsTextLineNumber = textLine.LineNumber + 1
                            let lineContents = textLine.ToString()                            
                            let! options = context.Document.GetOptionsAsync(cancellationToken) |> Async.AwaitTask
                            let tabSize = options.GetOption(FormattingOptions.TabSize, FSharpCommonConstants.FSharpLanguageName)
                            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.RightColumn, lineContents, [symbol.Text])
                            symbolUse
                            |> Option.bind (fun symbolUse ->
                                match symbolUse.Symbol with
                                | :? FSharpEntity as entity -> 
                                    if InterfaceStubGenerator.isInterface entity then
                                        Some (entity, symbolUse.DisplayContext)
                                    else None
                                | _ -> None)
                            |> Option.iter (fun (entity, displayContext) ->                                
                                getSuggestions (context, checkFileResults, state, displayContext, entity, tabSize))
                        | _ -> ()
                    | None -> ()
            | None -> ()
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
