// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

open FSharp.Compiler
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.SyntaxTree

[<NoEquality; NoComparison>]
type internal InterfaceState =
    { InterfaceData: InterfaceData 
      EndPosOfWith: pos option
      AppendBracketAt: int option
      Tokens: Tokenizer.SavedTokenInfo[] }

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "ImplementInterface"); Shared>]
type internal FSharpImplementInterfaceCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = ["FS0366"]
    let checker = checkerProvider.Checker
    static let userOpName = "ImplementInterfaceCodeFixProvider"

    let queryInterfaceState appendBracketAt (pos: pos) (tokens: Tokenizer.SavedTokenInfo[]) (ast: ParsedInput) =
        asyncMaybe {
            let line = pos.Line - 1
            let! iface = InterfaceStubGenerator.tryFindInterfaceDeclaration pos ast
            let endPosOfWidth =
                tokens 
                |> Array.tryPick (fun (t: Tokenizer.SavedTokenInfo) ->
                        if t.Tag = FSharpTokenTag.WITH || t.Tag = FSharpTokenTag.OWITH then
                            Some (Pos.fromZ line (t.RightColumn + 1))
                        else None)
            let appendBracketAt =
                match iface, appendBracketAt with
                | InterfaceData.ObjExpr _, Some _ -> appendBracketAt
                | _ -> None
            return { InterfaceData = iface; EndPosOfWith = endPosOfWidth; AppendBracketAt = appendBracketAt; Tokens = tokens } 
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
                |> Array.tryPick (fun (t: Tokenizer.SavedTokenInfo) ->
                            if t.Tag = FSharpTokenTag.NEW then
                                Some (t.LeftColumn + indentSize)
                            else None)
                // There is no reference point, we indent the content at the start column of the interface
                |> Option.defaultValue iface.Range.StartColumn

    let applyImplementInterface (sourceText: SourceText) state displayContext implementedMemberSignatures entity indentSize verboseMode = 
        let startColumn = inferStartColumn indentSize state sourceText
        let objectIdentifier = "this"
        let defaultBody = "raise (System.NotImplementedException())"
        let typeParams = state.InterfaceData.TypeParameters
        let stub = 
            let stub = InterfaceStubGenerator.formatInterface 
                           startColumn indentSize typeParams objectIdentifier defaultBody
                           displayContext implementedMemberSignatures entity verboseMode
            stub.TrimEnd(Environment.NewLine.ToCharArray())
        let stubChange =
            match state.EndPosOfWith with
            | Some pos -> 
                let currentPos = sourceText.Lines.[pos.Line-1].Start + pos.Column
                TextChange(TextSpan(currentPos, 0), stub)                
            | None ->
                let range = state.InterfaceData.Range
                let currentPos = sourceText.Lines.[range.EndLine-1].Start + range.EndColumn
                TextChange(TextSpan(currentPos, 0), " with" + stub)                
        match state.AppendBracketAt with
        | Some index ->
            sourceText.WithChanges(stubChange, TextChange(TextSpan(index, 0), " }"))
        | None ->
            sourceText.WithChanges(stubChange)

    let registerSuggestions (context: CodeFixContext, results: FSharpCheckFileResults, state: InterfaceState, displayContext, entity, indentSize) =
        if InterfaceStubGenerator.hasNoInterfaceMember entity then 
            ()
        else
            let membersAndRanges = InterfaceStubGenerator.getMemberNameAndRanges state.InterfaceData
            let interfaceMembers = InterfaceStubGenerator.getInterfaceMembers entity
            let hasTypeCheckError = results.Errors |> Array.exists (fun e -> e.Severity = FSharpErrorSeverity.Error)                
            // This comparison is a bit expensive
            if hasTypeCheckError && List.length membersAndRanges <> Seq.length interfaceMembers then    
                let diagnostics = context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id) |> Seq.toImmutableArray
                let registerCodeFix title verboseMode =
                    let codeAction =
                        CodeAction.Create(
                            title,
                            (fun (cancellationToken: CancellationToken) ->
                                async {
                                    let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                                    let getMemberByLocation(name, range: range) =
                                        let lineStr = sourceText.Lines.[range.EndLine-1].ToString()
                                        results.GetSymbolUseAtLocation(range.EndLine, range.EndColumn, lineStr, [name], userOpName=userOpName)
                                    let! implementedMemberSignatures =
                                        InterfaceStubGenerator.getImplementedMemberSignatures getMemberByLocation displayContext state.InterfaceData    
                                    let newSourceText = applyImplementInterface sourceText state displayContext implementedMemberSignatures entity indentSize verboseMode
                                    return context.Document.WithText(newSourceText)
                                } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
                            title)                
                    context.RegisterCodeFix(codeAction, diagnostics)

                registerCodeFix (SR.ImplementInterface()) true
                registerCodeFix (SR.ImplementInterfaceWithoutTypeAnnotation()) false
            else 
                ()
            
    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(context.Document, context.CancellationToken)
            let cancellationToken = context.CancellationToken
            let! sourceText = context.Document.GetTextAsync(cancellationToken)
            let! _, parsedInput, checkFileResults = checker.ParseAndCheckDocument(context.Document, projectOptions, sourceText = sourceText, userOpName = userOpName)
            let textLine = sourceText.Lines.GetLineFromPosition context.Span.Start
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            // Notice that context.Span doesn't return reliable ranges to find tokens at exact positions.
            // That's why we tokenize the line and try to find the last successive identifier token
            let tokens = Tokenizer.tokenizeLine(context.Document.Id, sourceText, context.Span.Start, context.Document.FilePath, defines)
            let startLeftColumn = context.Span.Start - textLine.Start
            let rec tryFindIdentifierToken acc i =
               if i >= tokens.Length then acc else
               match tokens.[i] with
               | t when t.LeftColumn < startLeftColumn ->
                   // Skip all the tokens starting before the context
                   tryFindIdentifierToken acc (i+1)
               | t when t.Tag = FSharpTokenTag.Identifier ->
                   tryFindIdentifierToken (Some t) (i+1)
               | t when t.Tag = FSharpTokenTag.DOT || Option.isNone acc ->
                   tryFindIdentifierToken acc (i+1)
               | _ -> acc
            let! token = tryFindIdentifierToken None 0
            let fixupPosition = textLine.Start + token.RightColumn
            let interfacePos = Pos.fromZ textLine.LineNumber token.RightColumn
            // We rely on the observation that the lastChar of the context should be '}' if that character is present
            let appendBracketAt =
                match sourceText.[context.Span.End-1] with
                | '}' -> None
                | _ -> 
                    Some context.Span.End
            let! interfaceState = queryInterfaceState appendBracketAt interfacePos tokens parsedInput                        
            let! symbol = Tokenizer.getSymbolAtPosition(context.Document.Id, sourceText, fixupPosition, context.Document.FilePath, defines, SymbolLookupKind.Greedy, false)
            let fcsTextLineNumber = textLine.LineNumber + 1
            let lineContents = textLine.ToString()                            
            let! options = context.Document.GetOptionsAsync(cancellationToken)
            let tabSize = options.GetOption(FormattingOptions.TabSize, FSharpConstants.FSharpLanguageName)
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.Ident.idRange.EndColumn, lineContents, symbol.FullIsland, userOpName=userOpName)
            let! entity, displayContext = 
                match symbolUse.Symbol with
                | :? FSharpEntity as entity -> 
                    if InterfaceStubGenerator.isInterface entity then
                        Some (entity, symbolUse.DisplayContext)
                    else None
                | _ -> None
            registerSuggestions (context, checkFileResults, interfaceState, displayContext, entity, tabSize)
        } 
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
