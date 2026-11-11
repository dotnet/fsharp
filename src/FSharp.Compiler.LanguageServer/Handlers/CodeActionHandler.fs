namespace FSharp.Compiler.LanguageServer.Handlers

open System
open System.Collections.Generic
open System.Text.RegularExpressions
open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Compiler.LanguageServer.Common
open FSharp.Compiler.LanguageServer
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Diagnostics.ExtendedData
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization
open System.Threading

#nowarn "57"

// Protocol Position is shadowed by FSharp.Compiler.Text.Position
type private LspPosition = Microsoft.VisualStudio.LanguageServer.Protocol.Position

// Context passed to each code fix function.
type CodeFixContext =
    {
        SourceText: ISourceText
        ParseResults: FSharpParseFileResults
        CheckResults: FSharpCheckFileResults option
        Diagnostic: FSharpDiagnostic
        DiagnosticRange: range
        FilePath: string
        DocumentUri: Uri
    }

module private LspCodeFixes =

    // ---------------------------------------------------------------
    // Utility functions
    // ---------------------------------------------------------------

    let fcsRangeToLspRange (range: range) =
        LspRange(
            Start = LspPosition(Line = range.StartLine - 1, Character = range.StartColumn),
            End = LspPosition(Line = range.EndLine - 1, Character = range.EndColumn)
        )

    let rangesOverlap (lspRange: LspRange) (fcsRange: range) =
        lspRange.Start.Line <= fcsRange.EndLine - 1
        && fcsRange.StartLine - 1 <= lspRange.End.Line

    let makeCodeAction (title: string) (documentUri: Uri) (editRange: range) (newText: string) =
        let changes = Dictionary<string, TextEdit array>()

        changes.[documentUri.AbsoluteUri] <- [| TextEdit(Range = fcsRangeToLspRange editRange, NewText = newText) |]

        [|
            CodeAction(Title = title, Kind = CodeActionKind.QuickFix, Edit = WorkspaceEdit(Changes = changes))
        |]

    let makeMultiEditCodeAction (title: string) (documentUri: Uri) (edits: (range * string) array) =
        let changes = Dictionary<string, TextEdit array>()

        changes.[documentUri.AbsoluteUri] <-
            edits
            |> Array.map (fun (r, newText) -> TextEdit(Range = fcsRangeToLspRange r, NewText = newText))

        [|
            CodeAction(Title = title, Kind = CodeActionKind.QuickFix, Edit = WorkspaceEdit(Changes = changes))
        |]

    let textAtRange (sourceText: ISourceText) (range: range) = sourceText.GetSubTextFromRange(range)

    let symbolAtPosition (ctx: CodeFixContext) (checkResults: FSharpCheckFileResults) (position: pos) =
        let line = ctx.SourceText.GetLineString(position.Line - 1)

        let rec identifierAt column =
            match QuickParse.GetCompleteIdentifierIsland true line column with
            | Some(_, endColumn, _) when endColumn > column && endColumn < line.Length && line[endColumn] = '.' ->
                identifierAt (endColumn + 1)
            | identifier -> identifier

        match identifierAt position.Column with
        | Some(_, endColumn, _) ->
            let name = QuickParse.GetPartialLongNameEx(line, endColumn - 1)
            let island = [ yield! name.QualifyingIdents; name.PartialIdent ]
            checkResults.GetSymbolUseAtLocation(position.Line, endColumn, line, island)
        | None -> None

    // ---------------------------------------------------------------
    // FS0760: Add 'new' keyword
    // ---------------------------------------------------------------

    let addNewKeywordFix (ctx: CodeFixContext) =
        let sourceText = ctx.SourceText
        let parseResults = ctx.ParseResults
        let diagnosticRange = ctx.DiagnosticRange
        let filePath = ctx.FilePath
        let documentUri = ctx.DocumentUri

        let getSourceLineStr line = sourceText.GetLineString(line - 1)

        let matchingApp path node =
            let (|TargetTy|_|) expr =
                match expr with
                | SynExpr.Ident id -> Some(SynType.LongIdent(SynLongIdent([ id ], [], [])))
                | SynExpr.LongIdent(longDotId = longDotId) -> Some(SynType.LongIdent longDotId)
                | SynExpr.TypeApp(SynExpr.Ident id, lessRange, typeArgs, commaRanges, greaterRange, _, range) ->
                    Some(
                        SynType.App(
                            SynType.LongIdent(SynLongIdent([ id ], [], [])),
                            Some lessRange,
                            typeArgs,
                            commaRanges,
                            greaterRange,
                            false,
                            range
                        )
                    )
                | SynExpr.TypeApp(SynExpr.LongIdent(longDotId = longDotId), lessRange, typeArgs, commaRanges, greaterRange, _, range) ->
                    Some(SynType.App(SynType.LongIdent longDotId, Some lessRange, typeArgs, commaRanges, greaterRange, false, range))
                | _ -> None

            match node with
            | SyntaxNode.SynExpr(SynExpr.App(funcExpr = TargetTy targetTy; argExpr = argExpr; range = m)) when
                Range.equals m diagnosticRange
                ->
                Some(targetTy, argExpr, path)
            | _ -> None

        match
            (diagnosticRange.Start, parseResults.ParseTree)
            ||> ParsedInput.tryPick matchingApp
        with
        | None -> [||]
        | Some(targetTy, argExpr, path) ->
            let needsParens =
                let newExpr = SynExpr.New(false, targetTy, argExpr, diagnosticRange)

                argExpr
                |> SynExpr.shouldBeParenthesizedInContext getSourceLineStr (SyntaxNode.SynExpr newExpr :: path)

            let targetTyText = sourceText.GetSubTextFromRange(targetTy.Range)

            let textBetween =
                let betweenRange = Range.mkRange filePath targetTy.Range.End argExpr.Range.Start

                if needsParens && betweenRange.StartLine = betweenRange.EndLine then
                    ""
                else
                    sourceText.GetSubTextFromRange(betweenRange)

            let argExprText =
                let originalArgText = sourceText.GetSubTextFromRange(argExpr.Range)

                if needsParens then
                    $"(%s{originalArgText})"
                else
                    originalArgText

            let newText = $"new %s{targetTyText}%s{textBetween}%s{argExprText}"

            makeCodeAction "Add 'new' keyword" documentUri diagnosticRange newText

    // FS0043: Replace '==' with '='
    let convertToSingleEquals (ctx: CodeFixContext) =
        let text = textAtRange ctx.SourceText ctx.DiagnosticRange

        if text = "==" then
            makeCodeAction "Use '=' for equality" ctx.DocumentUri ctx.DiagnosticRange "="
        else
            [||]

    // FS0043: Replace '!=' with '<>'
    let convertToNotEquals (ctx: CodeFixContext) =
        let text = textAtRange ctx.SourceText ctx.DiagnosticRange

        if text = "!=" then
            makeCodeAction "Use '<>' for inequality" ctx.DocumentUri ctx.DiagnosticRange "<>"
        else
            [||]

    // FS3198: Replace ':?>' with ':>' or 'downcast' with 'upcast'
    let changeToUpcast (ctx: CodeFixContext) =
        let text = textAtRange ctx.SourceText ctx.DiagnosticRange
        let hasDowncastOp = text.Contains(":?>")
        let hasDowncastKw = text.Contains("downcast")

        if hasDowncastOp && not hasDowncastKw then
            let newText = text.Replace(":?>", ":>")
            makeCodeAction "Use upcast operator" ctx.DocumentUri ctx.DiagnosticRange newText
        elif hasDowncastKw && not hasDowncastOp then
            let newText = text.Replace("downcast", "upcast")
            makeCodeAction "Use 'upcast'" ctx.DocumentUri ctx.DiagnosticRange newText
        else
            [||]

    // FS3366: Remove '.' before indexer bracket
    let fixIndexerAccess (ctx: CodeFixContext) =
        makeCodeAction "Remove indexer dot" ctx.DocumentUri ctx.DiagnosticRange ""

    // FS0597: Wrap expression in parentheses
    let wrapExpressionInParentheses (ctx: CodeFixContext) =
        let text = textAtRange ctx.SourceText ctx.DiagnosticRange
        makeCodeAction "Wrap expression in parentheses" ctx.DocumentUri ctx.DiagnosticRange $"(%s{text})"

    // FS0003: Change prefix negation to infix subtraction by adding space after '-'
    let changePrefixNegationToInfixSubtraction (ctx: CodeFixContext) =
        // The diagnostic range covers the identifier; the '-' is before it.
        // We need to find the '-' after the squiggly span and insert a space.
        let line = ctx.SourceText.GetLineString(ctx.DiagnosticRange.EndLine - 1)
        let col = ctx.DiagnosticRange.EndColumn

        let rec findDash i =
            if i >= line.Length then None
            elif line.[i] = '-' then Some i
            elif Char.IsWhiteSpace(line.[i]) then findDash (i + 1)
            else None

        match findDash col with
        | Some dashCol ->
            // Insert a space after the dash
            let insertPos = Position.mkPos ctx.DiagnosticRange.EndLine (dashCol + 1)

            let insertRange = Range.mkRange ctx.FilePath insertPos insertPos
            makeCodeAction "Use subtraction instead of negation" ctx.DocumentUri insertRange " "
        | None -> [||]

    // FS0039/FS0201: Replace 'using' with 'open' and remove ';'
    let convertCSharpUsingToFSharpOpen (ctx: CodeFixContext) =
        let msg = ctx.Diagnostic.Message
        let text = textAtRange ctx.SourceText ctx.DiagnosticRange

        let isUsingInError = msg.Contains("using ") || text.Contains("using ")

        if not isUsingInError then
            [||]
        else
            let lineIdx = ctx.DiagnosticRange.StartLine - 1
            let line = ctx.SourceText.GetLineString(lineIdx)

            if line.Contains("using") then
                let newLine = line.Replace("using", "open").Replace(";", "")

                let lineRange =
                    Range.mkRange
                        ctx.FilePath
                        (Position.mkPos ctx.DiagnosticRange.StartLine 0)
                        (Position.mkPos ctx.DiagnosticRange.StartLine line.Length)

                makeCodeAction "Replace 'using' with 'open'" ctx.DocumentUri lineRange newLine
            else
                [||]

    // FS0673: Add missing instance member parameter (e.g., 'x.')
    let addInstanceMemberParameter (ctx: CodeFixContext) =
        let insertRange =
            Range.mkRange ctx.FilePath ctx.DiagnosticRange.Start ctx.DiagnosticRange.Start

        makeCodeAction "Add missing instance member parameter" ctx.DocumentUri insertRange "x."

    // FS0010: Add '=' to type definition (when message mentions '=')
    let addMissingEqualsToTypeDefinition (ctx: CodeFixContext) =
        if
            not (ctx.Diagnostic.Message.Contains("="))
            || not (ctx.ParseResults.IsPositionWithinTypeDefinition(ctx.DiagnosticRange.Start))
        then
            [||]
        else
            let insertRange =
                Range.mkRange ctx.FilePath ctx.DiagnosticRange.Start ctx.DiagnosticRange.Start

            makeCodeAction "Add '=' to type definition" ctx.DocumentUri insertRange "= "

    // FS0010: Add 'fun' keyword (when message mentions '->')
    let addMissingFunKeyword (ctx: CodeFixContext) =
        let text = textAtRange ctx.SourceText ctx.DiagnosticRange

        if text <> "->" then
            [||]
        else
            let mutable lineNumber = ctx.DiagnosticRange.StartLine
            let mutable line = ctx.SourceText.GetLineString(lineNumber - 1)
            let mutable last = ctx.DiagnosticRange.StartColumn - 1

            while (last >= 0 && Char.IsWhiteSpace line[last]) || (last < 0 && lineNumber > 1) do
                if last < 0 then
                    lineNumber <- lineNumber - 1
                    line <- ctx.SourceText.GetLineString(lineNumber - 1)
                    last <- line.Length - 1
                else
                    last <- last - 1

            let argumentStart =
                if last >= 0 && (line[last] = ')' || line[last] = ']' || line[last] = '}') then
                    let mutable depth = 0
                    let mutable first = last

                    while first >= 0 && (first = last || depth > 0) do
                        match line[first] with
                        | ')'
                        | ']'
                        | '}' -> depth <- depth + 1
                        | '('
                        | '['
                        | '{' -> depth <- depth - 1
                        | _ -> ()

                        first <- first - 1

                    if depth = 0 then ValueSome(first + 1) else ValueNone
                else
                    match QuickParse.GetCompleteIdentifierIsland false line last with
                    | Some(identifier, endColumn, _) -> ValueSome(endColumn - identifier.Length)
                    | None -> ValueNone

            match argumentStart with
            | ValueSome column ->
                let position = Position.mkPos lineNumber column
                let insertRange = Range.mkRange ctx.FilePath position position
                makeCodeAction "Add 'fun' keyword" ctx.DocumentUri insertRange "fun "
            | ValueNone -> [||]

    // FS0010: Replace '=' with ':' in record field type definition
    let changeEqualsInFieldTypeToColon (ctx: CodeFixContext) =
        let text = textAtRange ctx.SourceText ctx.DiagnosticRange

        if
            text <> "="
            || not (ctx.ParseResults.IsPositionWithinRecordDefinition(ctx.DiagnosticRange.Start))
        then
            [||]
        else
            makeCodeAction "Replace '=' with ':'" ctx.DocumentUri ctx.DiagnosticRange ":"

    // FS0576: Add 'rec' to mutually recursive functions
    let addMissingRecToMutuallyRecFunctions (ctx: CodeFixContext) =
        // Find the 'let' keyword on the same line and insert 'rec' after it.
        let lineIdx = ctx.DiagnosticRange.StartLine - 1
        let line = ctx.SourceText.GetLineString(lineIdx)

        let letIdx = line.IndexOf("let ")

        if letIdx < 0 then
            [||]
        else
            let insertCol = letIdx + 3 // after "let"

            let insertPos = Position.mkPos ctx.DiagnosticRange.StartLine insertCol

            let insertRange = Range.mkRange ctx.FilePath insertPos insertPos
            makeCodeAction "Make declaration 'rec'" ctx.DocumentUri insertRange " rec"

    // FS0039: Make outer binding recursive
    let makeOuterBindingRecursive (ctx: CodeFixContext) =
        if not (ctx.ParseResults.IsPosContainedInApplication(ctx.DiagnosticRange.Start)) then
            [||]
        else
            match ctx.ParseResults.TryRangeOfNameOfNearestOuterBindingContainingPos(ctx.DiagnosticRange.Start) with
            | Some bindingNameRange ->
                let bindingName = textAtRange ctx.SourceText bindingNameRange
                let errorText = textAtRange ctx.SourceText ctx.DiagnosticRange

                if bindingName <> errorText then
                    [||]
                else
                    // Find 'let' before the binding name and insert 'rec' after it
                    let lineIdx = bindingNameRange.StartLine - 1
                    let line = ctx.SourceText.GetLineString(lineIdx)
                    let prefix = line.Substring(0, min bindingNameRange.StartColumn line.Length)
                    let letIdx = prefix.LastIndexOf("let ")

                    if letIdx < 0 then
                        [||]
                    else
                        let insertCol = letIdx + 3

                        let insertPos = Position.mkPos bindingNameRange.StartLine insertCol

                        let insertRange = Range.mkRange ctx.FilePath insertPos insertPos

                        makeCodeAction $"Make '%s{bindingName}' recursive" ctx.DocumentUri insertRange " rec"
            | None -> [||]

    // FS0039: Convert C# lambda (=>) to F# lambda (->)
    let convertCSharpLambdaToFSharpLambda (ctx: CodeFixContext) =
        match ctx.ParseResults.TryRangeOfParenEnclosingOpEqualsGreaterUsage(ctx.DiagnosticRange.Start) with
        | Some(fullParenRange, lambdaArgRange, lambdaBodyRange) ->
            let argText = textAtRange ctx.SourceText lambdaArgRange
            let bodyText = textAtRange ctx.SourceText lambdaBodyRange
            let newText = $"fun %s{argText} -> %s{bodyText}"
            makeCodeAction "Use F# lambda syntax" ctx.DocumentUri fullParenRange newText
        | None -> [||]

    // FS0747/FS0748: Remove unnecessary 'return' or 'yield'
    let removeReturnOrYield (ctx: CodeFixContext) =
        match ctx.ParseResults.TryRangeOfExprInYieldOrReturn(ctx.DiagnosticRange.Start) with
        | Some exprRange ->
            // Remove from diagnostic start to expression start (the keyword + whitespace)
            let removeRange =
                Range.mkRange ctx.FilePath ctx.DiagnosticRange.Start exprRange.Start

            let keyword = textAtRange ctx.SourceText removeRange |> fun s -> s.Trim()

            let title =
                match keyword with
                | "return!" -> "Remove 'return!'"
                | "return" -> "Remove 'return'"
                | "yield!" -> "Remove 'yield!'"
                | _ -> "Remove 'yield'"

            makeCodeAction title ctx.DocumentUri removeRange ""
        | None -> [||]

    // FS0020: Replace '=' with '<-' for mutable value assignment
    let useMutationWhenValueIsMutable (ctx: CodeFixContext) =
        match ctx.CheckResults with
        | None -> [||]
        | Some checkResults ->
            match symbolAtPosition ctx checkResults ctx.DiagnosticRange.Start with
            | Some symbolUse when
                match symbolUse.Symbol with
                | :? FSharpMemberOrFunctionOrValue as value -> value.IsMutable || value.HasSetterMethod
                | _ -> false
                ->
                let line = ctx.SourceText.GetLineString(symbolUse.Range.EndLine - 1)
                let mutable eqCol = symbolUse.Range.EndColumn

                while eqCol < line.Length && Char.IsWhiteSpace line[eqCol] do
                    eqCol <- eqCol + 1

                let equalsPosition = Position.mkPos symbolUse.Range.EndLine eqCol

                let eqRange =
                    Range.mkRange ctx.FilePath equalsPosition (Position.mkPos equalsPosition.Line (eqCol + 1))

                if
                    eqCol < line.Length
                    && line[eqCol] = '='
                    && (eqCol + 1 = line.Length || line[eqCol + 1] <> '=')
                    && Range.rangeContainsRange ctx.DiagnosticRange eqRange
                then
                    makeCodeAction "Use '<-' for mutation" ctx.DocumentUri eqRange "<-"
                else
                    [||]
            | _ -> [||]

    // FS3373: Use triple-quoted string interpolation
    let useTripleQuotedInterpolation (ctx: CodeFixContext) =
        match ctx.ParseResults.TryRangeOfStringInterpolationContainingPos(ctx.DiagnosticRange.Start) with
        | Some stringRange ->
            let text = textAtRange ctx.SourceText stringRange

            if text.StartsWith("$\"") && not (text.StartsWith("$\"\"\"")) then
                // $"..." -> $"""..."""
                let inner = text.Substring(2, text.Length - 3) // remove $" and trailing "
                let newText = "$\"\"\"" + inner + "\"\"\""
                makeCodeAction "Use triple-quoted interpolation" ctx.DocumentUri stringRange newText
            else
                [||]
        | None -> [||]

    // FS0001: Replace '!' (ref cell deref) with 'not' for boolean negation
    let changeRefCellDerefToNotExpression (ctx: CodeFixContext) =
        match ctx.ParseResults.TryRangeOfRefCellDereferenceContainingPos(ctx.DiagnosticRange.Start) with
        | Some derefRange ->
            // Replace the '!' with 'not '
            let bangRange =
                Range.mkRange ctx.FilePath derefRange.Start (Position.mkPos derefRange.StartLine (derefRange.StartColumn + 1))

            makeCodeAction "Use 'not' for boolean negation" ctx.DocumentUri bangRange "not "
        | None -> [||]

    // FS0039/FS0495/FS1129: Replace with compiler-suggested name
    let replaceWithSuggestion (ctx: CodeFixContext) =
        let msg = ctx.Diagnostic.Message
        // Diagnostic messages contain suggestions like: Did you mean 'xyz'?
        let m =
            Regex.Match(msg, @"Maybe you want one of the following:\s*(.*?)$", RegexOptions.Singleline)

        let suggestions =
            if m.Success then
                m.Groups.[1].Value.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
                |> Array.map (fun s -> s.Trim())
                |> Array.filter (fun s -> s.Length > 0)
            else
                // Try single suggestion: "... Did you mean 'X'?"
                let m2 = Regex.Match(msg, @"Did you mean '([^']+)'\?")

                if m2.Success then [| m2.Groups.[1].Value |] else [||]

        suggestions
        |> Array.map (fun suggestion ->
            let title = $"Replace with '%s{suggestion}'"

            (makeCodeAction title ctx.DocumentUri ctx.DiagnosticRange suggestion)
            |> Array.head)

    // FS1182: Prefix unused value with '_'
    let prefixUnusedValue (ctx: CodeFixContext) =
        let text = textAtRange ctx.SourceText ctx.DiagnosticRange

        if text.StartsWith("_") || text = "()" then
            [||]
        else
            let insertRange =
                Range.mkRange ctx.FilePath ctx.DiagnosticRange.Start ctx.DiagnosticRange.Start

            makeCodeAction $"Prefix '%s{text}' with underscore" ctx.DocumentUri insertRange "_"

    // FS1182: Rename unused value to '_'
    let discardUnusedValue (ctx: CodeFixContext) =
        let text = textAtRange ctx.SourceText ctx.DiagnosticRange

        if text = "_" || text = "()" then
            [||]
        else
            makeCodeAction $"Rename '%s{text}' to '_'" ctx.DocumentUri ctx.DiagnosticRange "_"

    // FS0027: Make declaration mutable
    let makeDeclarationMutable (ctx: CodeFixContext) =
        match ctx.CheckResults with
        | None -> [||]
        | Some checkResults ->
            if ctx.ParseResults.IsPositionContainedInACurriedParameter(ctx.DiagnosticRange.Start) then
                [||]
            else
                // The diagnostic points to the assignment site (e.g. "x <- 2").
                // Extract the identifier name at the start of the diagnostic range.
                let lineText = ctx.SourceText.GetLineString(ctx.DiagnosticRange.StartLine - 1)
                // Find end of identifier starting at StartColumn
                let startCol = ctx.DiagnosticRange.StartColumn
                let mutable endCol = startCol

                while endCol < lineText.Length
                      && (System.Char.IsLetterOrDigit(lineText.[endCol])
                          || lineText.[endCol] = '_'
                          || lineText.[endCol] = '\'') do
                    endCol <- endCol + 1

                if endCol <= startCol then
                    [||]
                else
                    let identName = lineText.Substring(startCol, endCol - startCol)

                    let symbolOpt =
                        checkResults.GetSymbolUseAtLocation(ctx.DiagnosticRange.StartLine, endCol, lineText, [ identName ])

                    match symbolOpt with
                    | Some symbolUse ->
                        let declRange = symbolUse.Symbol.DeclarationLocation

                        match declRange with
                        | Some declRange when declRange.FileName = ctx.FilePath ->
                            // Find 'let' before the declaration and insert 'mutable' after the identifier pattern
                            let declLineIdx = declRange.StartLine - 1
                            let declLine = ctx.SourceText.GetLineString(declLineIdx)
                            let prefix = declLine.Substring(0, min declRange.StartColumn declLine.Length)

                            if prefix.TrimEnd().EndsWith("let") || prefix.TrimEnd().EndsWith("and") then
                                let insertRange = Range.mkRange ctx.FilePath declRange.Start declRange.Start

                                makeCodeAction "Make declaration 'mutable'" ctx.DocumentUri insertRange "mutable "
                            else
                                [||]
                        | _ -> [||]
                    | None -> [||]

    // FS0072/FS3245: Add type annotation to object of indeterminate type
    let addTypeAnnotationToObjectOfIndeterminateType (ctx: CodeFixContext) =
        match ctx.CheckResults with
        | None -> [||]
        | Some checkResults ->
            let lineText = ctx.SourceText.GetLineString(ctx.DiagnosticRange.StartLine - 1)

            let symbolOpt =
                checkResults.GetSymbolUseAtLocation(
                    ctx.DiagnosticRange.StartLine,
                    ctx.DiagnosticRange.EndColumn,
                    lineText,
                    [ textAtRange ctx.SourceText ctx.DiagnosticRange ]
                )

            match symbolOpt with
            | Some symbolUse ->
                match symbolUse.Symbol with
                | :? FSharpMemberOrFunctionOrValue as mfv when not mfv.FullType.IsGenericParameter ->
                    let typeStr = mfv.FullType.Format(symbolUse.DisplayContext)
                    let annotation = $": %s{typeStr}"
                    // Insert after the identifier
                    let insertRange =
                        Range.mkRange ctx.FilePath ctx.DiagnosticRange.End ctx.DiagnosticRange.End

                    makeCodeAction "Add type annotation" ctx.DocumentUri insertRange $" %s{annotation}"
                | _ -> [||]
            | None -> [||]

    // FS0039/FS3578: Convert record to anonymous record
    let convertToAnonymousRecord (ctx: CodeFixContext) =
        match ctx.ParseResults.TryRangeOfRecordExpressionContainingPos(ctx.DiagnosticRange.Start) with
        | Some recordRange ->
            let text = textAtRange ctx.SourceText recordRange
            // { ... } -> {| ... |}
            if text.StartsWith("{") && text.EndsWith("}") && not (text.StartsWith("{|")) then
                let inner = text.Substring(1, text.Length - 2)
                let newText = $"{{|%s{inner}|}}"
                makeCodeAction "Convert to anonymous record" ctx.DocumentUri recordRange newText
            else
                [||]
        | None -> [||]

    // FS3873/FS0740: Add missing 'seq' to computation expression
    let addMissingSeq (ctx: CodeFixContext) =
        let text = textAtRange ctx.SourceText ctx.DiagnosticRange
        let newText = $"seq %s{text}"
        makeCodeAction "Add missing 'seq'" ctx.DocumentUri ctx.DiagnosticRange newText

    // FS0725/FS3548: Remove superfluous capture in union case pattern
    let removeSuperfluousCaptureForUnionCaseWithNoData (ctx: CodeFixContext) =
        match ctx.CheckResults with
        | None -> [||]
        | Some checkResults ->
            let items = checkResults.GetSemanticClassification(Some ctx.DiagnosticRange)

            let unionCaseItem =
                items
                |> Array.tryFind (fun item -> item.Type = FSharp.Compiler.EditorServices.SemanticClassificationType.UnionCase)

            match unionCaseItem with
            | Some item ->
                // Keep just the union case name, remove the capture
                let caseRange = item.Range
                let caseText = textAtRange ctx.SourceText caseRange
                makeCodeAction "Remove superfluous capture" ctx.DocumentUri ctx.DiagnosticRange caseText
            | None -> [||]

    // FS3218: Rename parameter to match signature file
    let renameParamToMatchSignature (ctx: CodeFixContext) =
        let suggestion =
            match ctx.Diagnostic.ExtendedData with
            | Some(:? ArgumentsInSigAndImplMismatchExtendedData as data) -> ValueSome data.SignatureName
            | _ ->
                let parts = Regex.Match(ctx.Diagnostic.Message, ".+'(.+)'.+'(.+)'.+")

                if parts.Success then
                    ValueSome parts.Groups[1].Value
                else
                    ValueNone

        match suggestion, ctx.CheckResults with
        | ValueSome name, Some checkResults ->
            match symbolAtPosition ctx checkResults ctx.DiagnosticRange.Start with
            | Some symbolUse ->
                let replacement = FSharpKeywords.NormalizeIdentifierBackticks name

                let edits =
                    checkResults.GetUsesOfSymbolInFile(symbolUse.Symbol)
                    |> Array.map (fun useOfParameter -> useOfParameter.Range, replacement)

                if Array.isEmpty edits then
                    [||]
                else
                    makeMultiEditCodeAction $"Replace with '{name}'" ctx.DocumentUri edits
            | None -> [||]
        | _ -> [||]

    // FS0366: Implement interface members
    let implementInterface (ctx: CodeFixContext) =
        async {
            match ctx.CheckResults with
            | None -> return [||]
            | Some checkResults ->
                let interfaceDataOpt =
                    InterfaceStubGenerator.TryFindInterfaceDeclaration ctx.DiagnosticRange.Start ctx.ParseResults.ParseTree

                match interfaceDataOpt with
                | None -> return [||]
                | Some interfaceData ->
                    let symbolUseOpt = symbolAtPosition ctx checkResults interfaceData.Range.Start

                    match symbolUseOpt with
                    | Some symbolUse ->
                        match symbolUse.Symbol with
                        | :? FSharpEntity as entity when
                            InterfaceStubGenerator.IsInterface entity
                            && not (InterfaceStubGenerator.HasNoInterfaceMember entity)
                            ->
                            let membersAndRanges = InterfaceStubGenerator.GetMemberNameAndRanges interfaceData

                            let indentation lineNumber =
                                let line = ctx.SourceText.GetLineString(lineNumber - 1)
                                line.Length - line.TrimStart().Length

                            let startColumn =
                                match membersAndRanges with
                                | (_, memberRange) :: _ -> indentation memberRange.StartLine
                                | [] -> indentation interfaceData.Range.StartLine + 4

                            let typeInstances = interfaceData.TypeParameters
                            let methodBody = "raise (System.NotImplementedException())"
                            let displayContext = symbolUse.DisplayContext

                            let getMemberByLocation (name, memberRange: range) =
                                let line = ctx.SourceText.GetLineString(memberRange.EndLine - 1)
                                checkResults.GetSymbolUseAtLocation(memberRange.EndLine, memberRange.EndColumn, line, [ name ])

                            let! implementedMembers =
                                InterfaceStubGenerator.GetImplementedMemberSignatures getMemberByLocation displayContext interfaceData

                            let stub =
                                InterfaceStubGenerator.FormatInterface
                                    startColumn
                                    4
                                    typeInstances
                                    "this"
                                    methodBody
                                    displayContext
                                    implementedMembers
                                    entity
                                    true

                            if System.String.IsNullOrWhiteSpace(stub) then
                                return [||]
                            else
                                let withKeyword =
                                    (interfaceData.Range.Start, ctx.ParseResults.ParseTree)
                                    ||> ParsedInput.tryPick (fun _ node ->
                                        match node with
                                        | SyntaxNode.SynMemberDefn(SynMemberDefn.Interface(interfaceType = ty; withKeyword = keyword)) when
                                            Range.equals ty.Range interfaceData.Range
                                            ->
                                            Some keyword
                                        | SyntaxNode.SynExpr(SynExpr.ObjExpr(objType = ty; withKeyword = keyword)) when
                                            Range.equals ty.Range interfaceData.Range
                                            ->
                                            Some keyword
                                        | _ -> None)
                                    |> Option.flatten

                                let memberEnds =
                                    match interfaceData with
                                    | InterfaceData.Interface(_, Some members) -> members |> List.map _.Range.End
                                    | InterfaceData.ObjExpr(_, bindings) -> bindings |> List.map _.RangeOfBindingWithRhs.End
                                    | _ -> []

                                let insertPosition =
                                    match memberEnds with
                                    | [] -> withKeyword |> Option.map _.End |> Option.defaultValue interfaceData.Range.End
                                    | positions ->
                                        let last = positions |> List.maxBy (fun position -> position.Line, position.Column)

                                        match interfaceData with
                                        | InterfaceData.Interface _ ->
                                            Position.mkPos last.Line (ctx.SourceText.GetLineString(last.Line - 1).Length)
                                        | InterfaceData.ObjExpr _ -> last

                                let newline =
                                    if
                                        ctx.SourceText.GetSubTextString(0, ctx.SourceText.Length).Contains("\r\n", StringComparison.Ordinal)
                                    then
                                        "\r\n"
                                    else
                                        "\n"

                                let stub =
                                    stub.Replace("\r\n", "\n").TrimStart(' ', '\t').Replace("\n", newline).TrimEnd('\r', '\n')

                                let insertText = (if withKeyword.IsSome then "" else " with") + stub

                                let insertRange = Range.mkRange ctx.FilePath insertPosition insertPosition

                                return makeCodeAction "Implement interface" ctx.DocumentUri insertRange insertText
                        | _ -> return [||]
                    | None -> return [||]
        }

    // ---------------------------------------------------------------
    // Registration
    // ---------------------------------------------------------------

    let private allCodeFixes =
        [
            "FS0760", nameof addNewKeywordFix, addNewKeywordFix
            "FS0043", nameof convertToSingleEquals, convertToSingleEquals
            "FS0043", nameof convertToNotEquals, convertToNotEquals
            "FS3198", nameof changeToUpcast, changeToUpcast
            "FS3366", nameof fixIndexerAccess, fixIndexerAccess
            "FS0597", nameof wrapExpressionInParentheses, wrapExpressionInParentheses
            "FS0003", nameof changePrefixNegationToInfixSubtraction, changePrefixNegationToInfixSubtraction
            "FS0039", nameof convertCSharpUsingToFSharpOpen, convertCSharpUsingToFSharpOpen
            "FS0201", nameof convertCSharpUsingToFSharpOpen, convertCSharpUsingToFSharpOpen
            "FS0673", nameof addInstanceMemberParameter, addInstanceMemberParameter
            "FS0010", nameof addMissingEqualsToTypeDefinition, addMissingEqualsToTypeDefinition
            "FS0010", nameof addMissingFunKeyword, addMissingFunKeyword
            "FS0010", nameof changeEqualsInFieldTypeToColon, changeEqualsInFieldTypeToColon
            "FS0576", nameof addMissingRecToMutuallyRecFunctions, addMissingRecToMutuallyRecFunctions
            "FS0039", nameof makeOuterBindingRecursive, makeOuterBindingRecursive
            "FS0039", nameof convertCSharpLambdaToFSharpLambda, convertCSharpLambdaToFSharpLambda
            "FS0747", nameof removeReturnOrYield, removeReturnOrYield
            "FS0748", nameof removeReturnOrYield, removeReturnOrYield
            "FS0020", nameof useMutationWhenValueIsMutable, useMutationWhenValueIsMutable
            "FS3373", nameof useTripleQuotedInterpolation, useTripleQuotedInterpolation
            "FS0001", nameof changeRefCellDerefToNotExpression, changeRefCellDerefToNotExpression
            "FS0039", nameof replaceWithSuggestion, replaceWithSuggestion
            "FS0495", nameof replaceWithSuggestion, replaceWithSuggestion
            "FS1129", nameof replaceWithSuggestion, replaceWithSuggestion
            "FS1182", nameof prefixUnusedValue, prefixUnusedValue
            "FS1182", nameof discardUnusedValue, discardUnusedValue
            "FS0027", nameof makeDeclarationMutable, makeDeclarationMutable
            "FS0072", nameof addTypeAnnotationToObjectOfIndeterminateType, addTypeAnnotationToObjectOfIndeterminateType
            "FS3245", nameof addTypeAnnotationToObjectOfIndeterminateType, addTypeAnnotationToObjectOfIndeterminateType
            "FS0039", nameof convertToAnonymousRecord, convertToAnonymousRecord
            "FS3578", nameof convertToAnonymousRecord, convertToAnonymousRecord
            "FS3873", nameof addMissingSeq, addMissingSeq
            "FS0740", nameof addMissingSeq, addMissingSeq
            "FS0725", nameof removeSuperfluousCaptureForUnionCaseWithNoData, removeSuperfluousCaptureForUnionCaseWithNoData
            "FS3548", nameof removeSuperfluousCaptureForUnionCaseWithNoData, removeSuperfluousCaptureForUnionCaseWithNoData
            "FS3218", nameof renameParamToMatchSignature, renameParamToMatchSignature
        ]
        |> List.map (fun (code, name, fix) -> code, (name, fun ctx -> async { return fix ctx }))
        |> fun fixes -> ("FS0366", (nameof implementInterface, implementInterface)) :: fixes

    let private codeFixMap =
        allCodeFixes
        |> List.groupBy fst
        |> List.map (fun (code, fixes) -> code, fixes |> List.map snd |> Array.ofList)
        |> dict

    // Get all applicable code actions for diagnostics overlapping the requested range.
    let getCodeActions
        (sourceText: ISourceText)
        (parseResults: FSharpParseFileResults)
        (checkResults: FSharpCheckFileResults option)
        (diagnostics: FSharpDiagnostic array)
        (requestRange: LspRange)
        (filePath: string)
        (documentUri: Uri)
        (logger: ILspLogger)
        (telemetry: ILspTelemetry)
        =
        async {
            let actions = ResizeArray<CodeAction>()

            for d in diagnostics |> Array.filter (fun d -> rangesOverlap requestRange d.Range) do
                match codeFixMap.TryGetValue(d.ErrorNumberText) with
                | true, fixes ->
                    let ctx =
                        {
                            SourceText = sourceText
                            ParseResults = parseResults
                            CheckResults = checkResults
                            Diagnostic = d
                            DiagnosticRange = d.Range
                            FilePath = filePath
                            DocumentUri = documentUri
                        }

                    for name, fix in fixes do
                        try
                            let! result = fix ctx
                            actions.AddRange result
                        with
                        | :? OperationCanceledException as ex -> System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw()
                        | ex ->
                            logger.LogException(ex, $"Code fix '{name}' failed")

                            Telemetry.reportEvent
                                telemetry
                                TelemetryEvents.ServerFault
                                [| "fix_name", name :> obj; "exception_type", ex.GetType().Name :> obj |]
                | false, _ -> ()

            return actions.ToArray()
        }

type CodeActionHandler() =
    interface IMethodHandler with
        member _.MutatesSolutionState = false

    interface IRequestHandler<CodeActionParams, CodeAction array, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentCodeActionName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync(request: CodeActionParams, context: FSharpRequestContext, cancellationToken: CancellationToken) =
            cancellableTask {
                let telemetry = context.LspServices.GetRequiredService<ILspTelemetry>()

                use _scope =
                    telemetry.ReportEventWithDuration(
                        TelemetryEvents.GetCodeActions,
                        [| "uri_hash", hash request.TextDocument.Uri :> obj |]
                    )

                let uri = request.TextDocument.Uri
                let projectContext = tryGetProjectContext request.TextDocument

                let! diagnosticReport = context.Workspace.Query.GetDiagnosticsForFile(uri, ?context = projectContext)

                let! parseResultsOpt, checkResultsOpt =
                    context.Workspace.Query.GetParseAndCheckResultsForFile(uri, ?context = projectContext)

                let! sourceTextOpt = context.Workspace.Query.GetSource uri

                match parseResultsOpt, sourceTextOpt with
                | Some parseResults, Some sourceText ->
                    let filePath = uri.LocalPath

                    return!
                        LspCodeFixes.getCodeActions
                            sourceText
                            parseResults
                            checkResultsOpt
                            diagnosticReport.Diagnostics
                            request.Range
                            filePath
                            uri
                            context.Logger
                            telemetry
                | _ -> return [||]
            }
            |> CancellableTask.start cancellationToken
