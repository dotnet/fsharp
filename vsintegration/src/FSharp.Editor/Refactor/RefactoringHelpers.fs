// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.RefactoringHelpers

open System
open System.Collections.Generic
open System.Text

open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization

open CancellableTasks

[<RequireQualifiedAccess; NoComparison; NoEquality>]
type Anchor =
    | Statement of start: pos
    | Clause of keywordEnd: pos * body: range * offsideColumn: int

[<NoComparison; NoEquality>]
type ExtractionTarget =
    {
        Expr: SynExpr
        Path: SyntaxVisitorPath
        Extracted: TextSpan
        Replaced: TextSpan
    }

[<NoComparison; NoEquality>]
type Extraction =
    {
        Context: CodeRefactoringContext
        Provider: string
        SourceText: SourceText
        Target: ExtractionTarget
        CheckResults: FSharpCheckFileResults
        IndentSize: int
        LiteralLines: HashSet<int>
        LineBreak: string
        Names: HashSet<string>
    }

let private isSame (expr: SynExpr) (other: SynExpr) = obj.ReferenceEquals(expr, other)

let private hasName (name: string) (ident: Ident) =
    String.Equals(ident.idText, name, StringComparison.Ordinal)

let textSpanOf (sourceText: SourceText) (m: range) =
    RoslynHelpers.FSharpRangeToTextSpan(sourceText, m)

let positionOf (sourceText: SourceText) (offset: int) =
    let linePosition = sourceText.Lines.GetLinePosition offset
    Position.mkPos (Line.fromZ linePosition.Line) linePosition.Character

let padding (width: int) = String(' ', width)

let leadingSpaces (sourceText: SourceText) (line: TextLine) =
    let mutable position = line.Start

    while position < line.End && sourceText[position] = ' ' do
        position <- position + 1

    position - line.Start

let lineBreakOf (sourceText: SourceText) =
    sourceText.Lines
    |> Seq.tryFindV (fun line -> line.EndIncludingLineBreak > line.End)
    |> ValueOption.map (fun line -> sourceText.ToString(TextSpan.FromBounds(line.End, line.EndIncludingLineBreak)))
    |> ValueOption.defaultValue Environment.NewLine

let private isLineLeading (sourceText: SourceText) (position: pos) =
    let line = sourceText.Lines[Line.toZ position.Line]
    leadingSpaces sourceText line = position.Column

let private onlyClosersFollow (sourceText: SourceText) (position: pos) =
    let line = sourceText.Lines[Line.toZ position.Line]

    let isCloser c =
        Char.IsWhiteSpace c || c = ')' || c = ']' || c = '}' || c = '|'

    let mutable offset = line.Start + position.Column

    while offset < line.End && isCloser (sourceText[offset]) do
        offset <- offset + 1

    offset = line.End
    || (offset + 1 < line.End
        && sourceText[offset] = '/'
        && sourceText[offset + 1] = '/')

let private startsInsideString (state: FSharpTokenizerLexState) =
    match FSharpLineTokenizer.ColorStateOfLexState state with
    | FSharpTokenizerColorState.String
    | FSharpTokenizerColorState.VerbatimString
    | FSharpTokenizerColorState.TripleQuoteString -> true
    // The public enum does not name the interpolated string states.
    | color -> not (Enum.IsDefined(typeof<FSharpTokenizerColorState>, color))

let private isSkipped (state: FSharpTokenizerLexState) =
    match FSharpLineTokenizer.ColorStateOfLexState state with
    | FSharpTokenizerColorState.IfDefSkip
    | FSharpTokenizerColorState.EndLineThenSkip -> true
    | _ -> false

let private startsWithDirective (code: ReadOnlySpan<char>) (keyword: string) =
    code.StartsWith(keyword.AsSpan(), StringComparison.Ordinal)
    && (code.Length = keyword.Length
        || Char.IsWhiteSpace code[keyword.Length]
        || code[keyword.Length] = '/')

let private isSkippedDirective (line: string) =
    let code = line.AsSpan().TrimStart()

    startsWithDirective code "#if"
    || startsWithDirective code "#elif"
    || startsWithDirective code "#else"
    || startsWithDirective code "#endif"

let private scanLine (tokenizer: FSharpSourceTokenizer) (line: string) (state: FSharpTokenizerLexState) =
    let lineTokenizer = tokenizer.CreateLineTokenizer line

    let rec loop state =
        match lineTokenizer.ScanToken state with
        | Some _, next -> loop next
        | None, next -> next

    loop state

let private linesInsideInactiveLiterals (sourceText: SourceText) (defines: string list) (langVersion: string) (lines: HashSet<int>) =
    let fileTokenizer = FSharpSourceTokenizer(defines, None, Some langVersion)
    let branchTokenizer = FSharpSourceTokenizer([], None, Some langVersion)
    let mutable fileState = FSharpTokenizerLexState.Initial
    let mutable branchState = FSharpTokenizerLexState.Initial

    for line in sourceText.Lines do
        let text = line.ToString()
        let inInactiveBranch = isSkipped fileState && not (isSkippedDirective text)

        if inInactiveBranch && startsInsideString branchState then
            lines.Add line.LineNumber |> ignore

        branchState <-
            if inInactiveBranch then
                scanLine branchTokenizer text branchState
            else
                FSharpTokenizerLexState.Initial

        fileState <- scanLine fileTokenizer text fileState

let private linesAfterFirst (m: range) =
    seq { for line in m.StartLine + 1 .. m.EndLine -> Line.toZ line }

let linesInsideLiterals (sourceText: SourceText) (parseTree: ParsedInput) (defines: string list) (langVersion: string) =
    let lines =
        (HashSet<int>(), parseTree)
        ||> ParsedInput.fold (fun lines _ node ->
            match node with
            | SyntaxNode.SynExpr(SynExpr.Const(range = m))
            | SyntaxNode.SynExpr(SynExpr.InterpolatedString(range = m))
            | SyntaxNode.SynPat(SynPat.Const(range = m)) -> lines.UnionWith(linesAfterFirst m)
            | _ -> ()

            lines)

    let directives =
        match parseTree with
        | ParsedInput.ImplFile file -> file.Trivia.ConditionalDirectives
        | ParsedInput.SigFile file -> file.Trivia.ConditionalDirectives

    if not directives.IsEmpty then
        linesInsideInactiveLiterals sourceText defines langVersion lines

    lines

let usedNames (parseTree: ParsedInput) =
    (HashSet<string>(StringComparer.Ordinal), parseTree)
    ||> ParsedInput.fold (fun names _ node ->
        match node with
        | SyntaxNode.SynExpr(SynExpr.Ident ident)
        | SyntaxNode.SynExpr(SynExpr.LongIdent(longDotId = SynLongIdent(id = ident :: _)))
        | SyntaxNode.SynExpr(SynExpr.LongIdentSet(longDotId = SynLongIdent(id = ident :: _)))
        | SyntaxNode.SynExpr(SynExpr.NamedIndexedPropertySet(longDotId = SynLongIdent(id = ident :: _)))
        | SyntaxNode.SynPat(SynPat.Named(ident = SynIdent(ident, _))) -> names.Add ident.idText |> ignore
        | SyntaxNode.SynPat(SynPat.LongIdent(longDotId = SynLongIdent(id = idents))) -> names.UnionWith(idents |> Seq.map _.idText)
        | _ -> ()

        names)

let uniqueName (baseName: string) (used: HashSet<string>) =
    Seq.initInfinite (fun i -> if i = 0 then baseName else $"{baseName}{i}")
    |> Seq.find (used.Contains >> not)

let private tryIndentedText (extraction: Extraction) (column: int) =
    let sourceText = extraction.SourceText
    let span = extraction.Target.Extracted
    let lines = sourceText.Lines
    let first = lines.GetLineFromPosition span.Start
    let last = lines.GetLineFromPosition span.End
    let shift = column - (span.Start - first.Start)

    let text =
        StringBuilder(sourceText.ToString(TextSpan.FromBounds(span.Start, min first.End span.End)))

    let rec append lineNumber =
        if lineNumber > last.LineNumber then
            ValueSome(text.ToString())
        else
            let previous = lines[lineNumber - 1]
            let line = lines[lineNumber]

            let content =
                sourceText.ToString(TextSpan.FromBounds(line.Start, min line.End span.End))

            let moved =
                match extraction.LiteralLines.Contains lineNumber with
                | true -> ValueSome content
                | false when String.IsNullOrWhiteSpace content -> ValueSome content
                | false when shift >= 0 -> ValueSome(String(' ', shift) + content)
                | false when leadingSpaces sourceText line >= -shift -> ValueSome(content.Substring(-shift))
                | false -> ValueNone

            match moved with
            | ValueSome moved ->
                text.Append(sourceText.ToString(TextSpan.FromBounds(previous.End, previous.EndIncludingLineBreak))).Append(moved)
                |> ignore

                append (lineNumber + 1)
            | ValueNone -> ValueNone

    append (first.LineNumber + 1)

let private trimmed (sourceText: SourceText) (span: TextSpan) =
    let mutable start = span.Start
    let mutable finish = span.End

    while start < finish && Char.IsWhiteSpace sourceText[start] do
        start <- start + 1

    while finish > start && Char.IsWhiteSpace sourceText[finish - 1] do
        finish <- finish - 1

    TextSpan.FromBounds(start, finish)

let private directiveLine (directive: ConditionalDirectiveTrivia) =
    match directive with
    | ConditionalDirectiveTrivia.If(range = m)
    | ConditionalDirectiveTrivia.Elif(range = m)
    | ConditionalDirectiveTrivia.Else(range = m)
    | ConditionalDirectiveTrivia.EndIf(range = m) -> m.StartLine

let private isOperator (name: string) (expr: SynExpr) =
    match expr with
    | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ operator ])) -> hasName name operator
    | _ -> false

let private isCall (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.App(flag = ExprAtomicFlag.Atomic) | SynExpr.New _) :: _ -> true
    | _ -> false

let private isMethodArgument (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.Paren _) :: call
    | SyntaxNode.SynExpr(SynExpr.Tuple _) :: SyntaxNode.SynExpr(SynExpr.Paren _) :: call -> isCall call
    | _ -> false

let rec private isExtractableShape (expr: SynExpr) (path: SyntaxVisitorPath) =
    match expr with
    | SynExpr.App(isInfix = true)
    | SynExpr.Ident _
    | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ _ ]))
    | SynExpr.Const(SynConst.Unit, _)
    | SynExpr.Paren(rightParenRange = None)
    | SynExpr.ComputationExpr _
    | SynExpr.YieldOrReturn _
    | SynExpr.YieldOrReturnFrom _
    | SynExpr.DoBang _
    | SynExpr.MatchBang _
    | SynExpr.WhileBang _
    | SynExpr.ImplicitZero _
    | SynExpr.SequentialOrImplicitYield _
    | SynExpr.JoinIn _
    | SynExpr.ArbitraryAfterError _
    | SynExpr.FromParseError _
    | SynExpr.DiscardAfterMissingQualificationAfterDot _
    | SynExpr.Typar _
    | SynExpr.TraitCall _
    | SynExpr.IndexRange _
    | SynExpr.IndexFromEnd _
    | SynExpr.Fixed _
    | SynExpr.AddressOf _
    | SynExpr.Do _
    | SynExpr.Dynamic _
    | SynExpr.LongIdentSet _
    | SynExpr.Set _
    | SynExpr.DotSet _
    | SynExpr.DotIndexedSet _
    | SynExpr.NamedIndexedPropertySet _
    | SynExpr.DotNamedIndexedPropertySet _ -> false
    | SynExpr.Paren(expr = inner) -> isExtractableShape inner (SyntaxNode.SynExpr expr :: path)
    | SynExpr.LetOrUse letOrUse -> not letOrUse.IsBang
    | SynExpr.Tuple _ -> not (isMethodArgument path)
    | SynExpr.App(isInfix = false; funcExpr = SynExpr.App(isInfix = true; funcExpr = equals; argExpr = SynExpr.Ident _)) when
        isOperator "op_Equality" equals
        ->
        not (isMethodArgument path)
    | _ -> true

let private isInExcludedContext (expr: SynExpr) (path: SyntaxVisitorPath) =
    let rec loop (child: SyntaxNode) (path: SyntaxVisitorPath) =
        match path, child with
        | [], _ -> false
        | SyntaxNode.SynExpr(SynExpr.InterpolatedString _ | SynExpr.Quote _ | SynExpr.Lazy _ | SynExpr.Assert _) :: _, _ -> true
        | SyntaxNode.SynExpr(SynExpr.While(whileExpr = condition) | SynExpr.WhileBang(whileExpr = condition)) :: _, SyntaxNode.SynExpr expr when
            isSame condition expr
            ->
            true
        | SyntaxNode.SynExpr(SynExpr.App(funcExpr = SynExpr.Ident operator; argExpr = operand)) :: _, SyntaxNode.SynExpr expr when
            hasName "nameof" operator && isSame operand expr
            ->
            true
        | SyntaxNode.SynMatchClause(SynMatchClause(whenExpr = Some guard)) :: _, SyntaxNode.SynExpr expr when isSame guard expr -> true
        | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = SynExpr.App(isInfix = true; funcExpr = operator); argExpr = right)) :: _,
          SyntaxNode.SynExpr expr when
            isSame right expr
            && (isOperator "op_BooleanAnd" operator || isOperator "op_BooleanOr" operator)
            ->
            true
        | parent :: rest, _ -> loop parent rest

    loop (SyntaxNode.SynExpr expr) path

let private hasOrphanedComputationConstruct (expr: SynExpr) =
    let containers = ResizeArray<range>()
    let statements = ResizeArray<range>()

    [ SyntaxNode.SynExpr expr ]
    |> SyntaxNodes.fold
        (fun () _ node ->
            match node with
            | SyntaxNode.SynExpr(SynExpr.ComputationExpr(range = m) | SynExpr.ArrayOrListComputed(range = m)) -> containers.Add m
            | SyntaxNode.SynExpr(SynExpr.YieldOrReturn(range = m) | SynExpr.YieldOrReturnFrom(range = m) | SynExpr.DoBang(range = m) | SynExpr.MatchBang(
                range = m) | SynExpr.WhileBang(range = m) | SynExpr.JoinIn(range = m)) -> statements.Add m
            | SyntaxNode.SynExpr(SynExpr.LetOrUse letOrUse) when letOrUse.IsBang -> statements.Add letOrUse.Range
            | _ -> ())
        ()

    statements
    |> Seq.exists (fun statement ->
        not (
            containers
            |> Seq.exists (fun container -> Range.rangeContainsRange container statement)
        ))

let private targetOf (sourceText: SourceText) (expr: SynExpr) (path: SyntaxVisitorPath) =
    let span = textSpanOf sourceText expr.Range

    let extracted =
        match expr with
        | SynExpr.Paren(expr = inner) -> textSpanOf sourceText inner.Range
        | _ -> span

    let replaced =
        match expr with
        | SynExpr.Paren _ when isCall path -> extracted
        | _ -> span

    {
        Expr = expr
        Path = path
        Extracted = extracted
        Replaced = replaced
    }

let private exprSpanning (start: pos) (finish: pos) (path: SyntaxVisitorPath) (node: SyntaxNode) =
    match node with
    | SyntaxNode.SynExpr expr when Position.posEq expr.Range.Start start && Position.posEq expr.Range.End finish -> Some(expr, path)
    | _ -> None

let private crossesDirective (directives: ConditionalDirectiveTrivia list) (start: pos) (finish: pos) =
    directives
    |> List.exists (fun directive ->
        let line = directiveLine directive
        line >= start.Line && line <= finish.Line)

let tryExtractionTarget (sourceText: SourceText) (parseTree: ParsedInput) (selection: TextSpan) =
    let span = trimmed sourceText selection

    match parseTree with
    | ParsedInput.ImplFile file when not span.IsEmpty ->
        let start = positionOf sourceText span.Start
        let finish = positionOf sourceText span.End

        match (start, parseTree) ||> ParsedInput.tryPickLast (exprSpanning start finish) with
        | Some(expr, path) when
            not (crossesDirective file.Trivia.ConditionalDirectives start finish)
            && isExtractableShape expr path
            && not (isInExcludedContext expr path)
            && not (hasOrphanedComputationConstruct expr)
            ->
            ValueSome(targetOf sourceText expr path)
        | _ -> ValueNone
    | _ -> ValueNone

let isLiteralConstant (expr: SynExpr) =
    match expr with
    | SynExpr.Const((SynConst.Bool _ | SynConst.SByte _ | SynConst.Byte _ | SynConst.Int16 _ | SynConst.UInt16 _ | SynConst.Int32 _ | SynConst.UInt32 _ | SynConst.Int64 _ | SynConst.UInt64 _ | SynConst.Single _ | SynConst.Double _ | SynConst.Decimal _ | SynConst.Char _ | SynConst.String _),
                    _) -> true
    | _ -> false

let private constantAt (position: pos) (path: SyntaxVisitorPath) (node: SyntaxNode) =
    match node with
    | SyntaxNode.SynExpr(SynExpr.Const(range = m) as expr) when
        isLiteralConstant expr
        && Position.posGeq position m.Start
        && Position.posGeq m.End position
        && not (isInExcludedContext expr path)
        ->
        Some(expr, path)
    | _ -> None

let tryConstantAtCaret (sourceText: SourceText) (parseTree: ParsedInput) (selection: TextSpan) =
    let position = positionOf sourceText selection.Start

    if selection.IsEmpty then
        match (position, parseTree) ||> ParsedInput.tryPickLast (constantAt position) with
        | Some(expr, path) -> ValueSome(targetOf sourceText expr path)
        | None -> ValueNone
    else
        ValueNone

let private parenthesizedLambdaHeaderAt (position: pos) (_: SyntaxVisitorPath) (node: SyntaxNode) =
    match node with
    | SyntaxNode.SynExpr(SynExpr.Paren(expr = SynExpr.Lambda(parsedData = Some _; trivia = { ArrowRange = Some arrow }) as lambda; range = m)) when
        Position.posGeq position lambda.Range.Start
        && Position.posGeq arrow.End position
        ->
        Some m
    | _ -> None

let private tryParenthesizedLambdaAtCaret (sourceText: SourceText) (parseTree: ParsedInput) (caret: int) =
    let position = positionOf sourceText caret

    match
        (position, parseTree)
        ||> ParsedInput.tryPickLast (parenthesizedLambdaHeaderAt position)
    with
    | Some m -> tryExtractionTarget sourceText parseTree (textSpanOf sourceText m)
    | None -> ValueNone

let trySelectedTarget (sourceText: SourceText) (parseTree: ParsedInput) (selection: TextSpan) =
    if selection.IsEmpty then
        tryParenthesizedLambdaAtCaret sourceText parseTree selection.Start
    else
        tryExtractionTarget sourceText parseTree selection

let private isFunctionBinding (binding: SynBinding) =
    match binding with
    | SynBinding(headPat = SynPat.LongIdent(argPats = SynArgPats.Pats(_ :: _))) -> true
    | _ -> false

let private anchorOf (child: SyntaxNode) (parent: SyntaxNode) (outer: SyntaxVisitorPath) (offsideColumn: int) =
    match parent, child with
    | SyntaxNode.SynExpr(SynExpr.Sequential _), SyntaxNode.SynExpr expr -> ValueSome(Anchor.Statement expr.Range.Start)
    | SyntaxNode.SynExpr(SynExpr.LetOrUse letOrUse), SyntaxNode.SynExpr expr when isSame letOrUse.Body expr ->
        ValueSome(Anchor.Statement expr.Range.Start)
    | SyntaxNode.SynExpr(SynExpr.For(doBody = body) | SynExpr.ForEach(bodyExpr = body) | SynExpr.While(doExpr = body) | SynExpr.TryWith(
        tryExpr = body) | SynExpr.TryFinally(tryExpr = body) | SynExpr.ComputationExpr(expr = body)),
      SyntaxNode.SynExpr expr when isSame body expr -> ValueSome(Anchor.Statement expr.Range.Start)
    | SyntaxNode.SynExpr(SynExpr.TryFinally(finallyExpr = body)), SyntaxNode.SynExpr expr when isSame body expr ->
        ValueSome(Anchor.Statement expr.Range.Start)
    | SyntaxNode.SynModule(SynModuleDecl.Expr _), SyntaxNode.SynExpr expr -> ValueSome(Anchor.Statement expr.Range.Start)
    | SyntaxNode.SynBinding(SynBinding(expr = rhs; trivia = trivia) as binding), SyntaxNode.SynExpr expr when isSame rhs expr ->
        match outer, trivia.EqualsRange with
        | SyntaxNode.SynExpr(SynExpr.LetOrUse letOrUse) :: _, _ when not letOrUse.IsRecursive && not (isFunctionBinding binding) ->
            ValueSome(Anchor.Statement letOrUse.Range.Start)
        | _, Some equals -> ValueSome(Anchor.Clause(equals.End, expr.Range, offsideColumn))
        | _ -> ValueNone
    | SyntaxNode.SynMatchClause(SynMatchClause(resultExpr = body; trivia = { ArrowRange = Some arrow })), SyntaxNode.SynExpr expr when
        isSame body expr
        ->
        ValueSome(Anchor.Clause(arrow.End, expr.Range, offsideColumn))
    | SyntaxNode.SynExpr(SynExpr.IfThenElse(thenExpr = body; trivia = trivia)), SyntaxNode.SynExpr expr when isSame body expr ->
        ValueSome(Anchor.Clause(trivia.ThenKeyword.End, expr.Range, offsideColumn))
    | SyntaxNode.SynExpr(SynExpr.IfThenElse(elseExpr = Some body; trivia = { ElseKeyword = Some elseKeyword })), SyntaxNode.SynExpr expr when
        isSame body expr
        ->
        ValueSome(Anchor.Clause(elseKeyword.End, expr.Range, offsideColumn))
    | SyntaxNode.SynExpr(SynExpr.Lambda(parsedData = Some(_, body); trivia = { ArrowRange = Some arrow })), SyntaxNode.SynExpr expr when
        isSame body expr
        ->
        ValueSome(Anchor.Clause(arrow.End, expr.Range, offsideColumn))
    | _ -> ValueNone

let private (|ElifBranch|_|) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.IfThenElse _ as nested) :: (SyntaxNode.SynExpr(SynExpr.IfThenElse(elseExpr = Some elseExpr)) :: _ as enclosingIf) when
        isSame elseExpr nested
        ->
        Some enclosingIf
    | _ -> None

let rec private offsideColumnOf (path: SyntaxVisitorPath) =
    match path with
    | [] -> 0
    | ElifBranch enclosingIf -> offsideColumnOf enclosingIf
    | SyntaxNode.SynBinding(SynBinding(trivia = trivia)) :: _ -> trivia.LeadingKeyword.Range.StartColumn
    | SyntaxNode.SynExpr(SynExpr.Match(range = m) | SynExpr.MatchBang(range = m) | SynExpr.TryWith(range = m) | SynExpr.TryFinally(range = m) | SynExpr.IfThenElse(
        range = m) | SynExpr.For(range = m) | SynExpr.ForEach(range = m) | SynExpr.While(range = m)) :: _ -> m.StartColumn
    | _ :: rest -> offsideColumnOf rest

let anchorsOf (expr: SynExpr) (path: SyntaxVisitorPath) =
    let rec loop (child: SyntaxNode) (path: SyntaxVisitorPath) =
        match path with
        | [] -> []
        | parent :: outer ->
            match anchorOf child parent outer (offsideColumnOf path) with
            | ValueSome anchor -> anchor :: loop parent outer
            | ValueNone -> loop parent outer

    loop (SyntaxNode.SynExpr expr) path

let rec private hasInferredTypeVariable (ty: FSharpType) =
    if ty.IsGenericParameter then
        ty.GenericParameter.IsCompilerGenerated
    else
        not ty.IsMeasureType
        && ty.GenericArguments |> Seq.exists hasInferredTypeVariable

let tryFunctionTypeText (checkResults: FSharpCheckFileResults) (target: ExtractionTarget) =
    let parenthesizedFunction =
        match target.Expr, target.Path with
        | SynExpr.Paren(expr = SynExpr.Lambda _ | SynExpr.MatchLambda _; range = m), _
        | (SynExpr.Lambda _ | SynExpr.MatchLambda _), SyntaxNode.SynExpr(SynExpr.Paren(range = m)) :: _ -> ValueSome m
        | _ -> ValueNone

    parenthesizedFunction
    |> ValueOption.bind (fun m ->
        match checkResults.TryGetCapturedType m, checkResults.TryGetCapturedDisplayContext m with
        | Some ty, Some displayContext when ty.StripAbbreviations().IsFunctionType && not (hasInferredTypeVariable ty) ->
            ValueSome(ty.Format displayContext)
        | _ -> ValueNone)

let literalAttributeAt (checkResults: FSharpCheckFileResults) (position: pos) =
    let attribute =
        checkResults.ProjectContext.GetReferencedAssemblies()
        |> List.tryPick _.Contents.FindEntityByPath([ "Microsoft"; "FSharp"; "Core"; "LiteralAttribute" ])

    match attribute with
    | Some attribute when not (checkResults.IsRelativeNameResolvableFromSymbol(position, [], attribute)) ->
        "global.Microsoft.FSharp.Core.Literal"
    | _ -> "Literal"

let private tryEnclosingModuleLet (path: SyntaxVisitorPath) =
    path
    |> List.tryPick (function
        | SyntaxNode.SynModule(SynModuleDecl.Let(bindings = binding :: _; range = m)) -> Some struct (binding, m)
        | _ -> None)

let tryDeclaration (extraction: Extraction) (header: string) (bodyColumn: int) =
    let sourceText = extraction.SourceText
    let lines = sourceText.Lines
    let extracted = extraction.Target.Extracted

    tryIndentedText extraction bodyColumn
    |> ValueOption.map (fun body ->
        if lines.GetLineFromPosition(extracted.Start).LineNumber = lines.GetLineFromPosition(extracted.End).LineNumber then
            $"{header} = {body}"
        else
            $"{header} ={extraction.LineBreak}{padding bodyColumn}{body}")

let private tryInFrontOfLine (extraction: Extraction) (start: pos) (header: string) (replacement: string) =
    let line = extraction.SourceText.Lines[Line.toZ start.Line]

    tryDeclaration extraction header (start.Column + extraction.IndentSize)
    |> ValueOption.map (fun declaration ->
        [
            TextChange(TextSpan(line.Start, 0), $"{padding start.Column}{declaration}{extraction.LineBreak}")
            TextChange(extraction.Target.Replaced, replacement)
        ])

let private linesMovedWithBody (extraction: Extraction) (body: range) =
    let lines = extraction.SourceText.Lines
    let replaced = extraction.Target.Replaced

    let stays (line: TextLine) =
        extraction.LiteralLines.Contains line.LineNumber
        || String.IsNullOrWhiteSpace(line.ToString())
        || (line.Start >= replaced.Start && line.Start <= replaced.End)

    [
        for lineNumber in Line.toZ body.StartLine + 1 .. Line.toZ body.EndLine -> lines[lineNumber]
    ]
    |> List.filter (stays >> not)

let private tryUnderKeyword
    (extraction: Extraction)
    (keywordEnd: pos)
    (body: range)
    (offsideColumn: int)
    (header: string)
    (replacement: string)
    =
    let sourceText = extraction.SourceText
    let keywordLine = sourceText.Lines[Line.toZ keywordEnd.Line]

    let gap =
        TextSpan.FromBounds(keywordLine.Start + keywordEnd.Column, (textSpanOf sourceText body).Start)

    let newIndent =
        max (leadingSpaces sourceText keywordLine) offsideColumn + extraction.IndentSize

    let shift = newIndent - body.StartColumn
    let movedLines = linesMovedWithBody extraction body

    let indentation =
        [
            for line in movedLines do
                if shift > 0 then
                    TextChange(TextSpan(line.Start, 0), padding shift)
                elif shift < 0 then
                    TextChange(TextSpan(line.Start, -shift), "")
        ]

    let canMove =
        movedLines
        |> List.forall (fun line -> shift >= 0 || leadingSpaces sourceText line >= -shift)

    if canMove && String.IsNullOrWhiteSpace(sourceText.ToString gap) then
        tryDeclaration extraction header (newIndent + extraction.IndentSize)
        |> ValueOption.map (fun declaration ->
            [
                TextChange(gap, $"{extraction.LineBreak}{padding newIndent}{declaration}{extraction.LineBreak}{padding newIndent}")
                yield! indentation
                TextChange(extraction.Target.Replaced, replacement)
            ]
            |> List.sortBy _.Span.Start)
    else
        ValueNone

let tryDeclareInFront (extraction: Extraction) (anchor: Anchor) (header: string) (replacement: string) =
    let sourceText = extraction.SourceText

    match anchor with
    | Anchor.Statement start when isLineLeading sourceText start -> tryInFrontOfLine extraction start header replacement
    | Anchor.Clause(body = body) when isLineLeading sourceText body.Start -> tryInFrontOfLine extraction body.Start header replacement
    | Anchor.Clause(keywordEnd, body, offsideColumn) when keywordEnd.Line = body.StartLine && onlyClosersFollow sourceText body.End ->
        tryUnderKeyword extraction keywordEnd body offsideColumn header replacement
    | _ -> ValueNone

let tryDeclareInFrontOfModuleLet (extraction: Extraction) (attributes: string list) (header: string) (replacement: string) =
    let sourceText = extraction.SourceText

    match tryEnclosingModuleLet extraction.Target.Path with
    | Some(struct (SynBinding(xmlDoc = xmlDoc), declaration)) when isLineLeading sourceText declaration.Start ->
        let firstLine =
            if xmlDoc.IsEmpty then
                declaration.StartLine
            else
                min declaration.StartLine xmlDoc.Range.StartLine

        let line = sourceText.Lines[Line.toZ firstLine]
        let indent = leadingSpaces sourceText line

        tryDeclaration extraction header (indent + extraction.IndentSize)
        |> ValueOption.map (fun declaration ->
            let attributeLines =
                attributes
                |> List.map (fun attribute -> $"{padding indent}{attribute}{extraction.LineBreak}")
                |> String.concat ""

            [
                TextChange(
                    TextSpan(line.Start, 0),
                    $"{attributeLines}{padding indent}{declaration}{extraction.LineBreak}{extraction.LineBreak}"
                )
                TextChange(extraction.Target.Replaced, replacement)
            ])
    | _ -> ValueNone

let prepareExtraction
    (context: CodeRefactoringContext)
    (provider: string)
    (sourceText: SourceText)
    (parseTree: ParsedInput)
    (target: ExtractionTarget)
    =
    cancellableTask {
        let document = context.Document
        let! options = document.GetOptionsAsync context.CancellationToken
        let! defines, langVersion = document.GetFsharpParsingOptionsAsync provider
        let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync provider

        return
            {
                Context = context
                Provider = provider
                SourceText = sourceText
                Target = target
                CheckResults = checkResults
                IndentSize = options.GetOption(FormattingOptions.IndentationSize, FSharpConstants.FSharpLanguageName)
                LiteralLines = linesInsideLiterals sourceText parseTree defines langVersion
                LineBreak = lineBreakOf sourceText
                Names = usedNames parseTree
            }
    }

let registerRefactoring (extraction: Extraction) (title: string) (kind: string) (changes: TextChange list voption) =
    match changes with
    | ValueSome changes ->
        let changedDocument =
            cancellableTask {
                TelemetryReporter.ReportSingleEvent(
                    TelemetryEvents.RefactoringActivated,
                    [| "name", box extraction.Provider; "kind", box kind |]
                )

                return extraction.Context.Document.WithText(extraction.SourceText.WithChanges changes)
            }

        extraction.Context.RegisterRefactoring(CodeAction.Create(title, changedDocument, title))
    | ValueNone -> ()
