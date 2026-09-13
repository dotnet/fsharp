// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.RefactoringHelpers

open System
open System.Collections.Generic
open System.Text

open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Text

/// A place in front of which a declaration used by the selected expression can be inserted.
[<RequireQualifiedAccess; NoComparison; NoEquality>]
type Anchor =
    /// An expression that is a statement of its block.
    | Statement of start: pos
    /// The body of a binding, match clause, branch or lambda, after the keyword ending at keywordEnd.
    | Clause of keywordEnd: pos * body: range

[<NoComparison; NoEquality>]
type ExtractionTarget =
    {
        Expr: SynExpr
        Path: SyntaxVisitorPath
        /// The selection without its parentheses when the selected expression is parenthesized.
        Content: TextSpan
        /// The text the new name replaces; the parentheses of a method call stay.
        Replaced: TextSpan
    }

let private isSame (expr: SynExpr) (other: SynExpr) = obj.ReferenceEquals(expr, other)

let private hasName (name: string) (ident: Ident) =
    String.Equals(ident.idText, name, StringComparison.Ordinal)

let textSpanOf (sourceText: SourceText) (m: range) =
    RoslynHelpers.FSharpRangeToTextSpan(sourceText, m)

let positionOf (sourceText: SourceText) (offset: int) =
    let linePosition = sourceText.Lines.GetLinePosition offset
    Position.mkPos (Line.fromZ linePosition.Line) linePosition.Character

let leadingSpaces (sourceText: SourceText) (line: TextLine) =
    let mutable position = line.Start

    while position < line.End && sourceText[position] = ' ' do
        position <- position + 1

    position - line.Start

let lineBreakOf (sourceText: SourceText) =
    sourceText.Lines
    |> Seq.tryFind (fun line -> line.EndIncludingLineBreak > line.End)
    |> Option.map (fun line -> sourceText.ToString(TextSpan.FromBounds(line.End, line.EndIncludingLineBreak)))
    |> Option.defaultValue Environment.NewLine

let isLineLeading (sourceText: SourceText) (position: pos) =
    let line = sourceText.Lines[Line.toZ position.Line]
    leadingSpaces sourceText line = position.Column

/// Whether only closing brackets and a line comment follow the position on its line.
let restOfLineIsClosers (sourceText: SourceText) (position: pos) =
    let line = sourceText.Lines[Line.toZ position.Line]

    let rest =
        sourceText.ToString(TextSpan.FromBounds(line.Start + position.Column, line.End))

    let code =
        match rest.IndexOf("//", StringComparison.Ordinal) with
        | -1 -> rest
        | comment -> rest.Substring(0, comment)

    code
    |> Seq.forall (fun c -> Char.IsWhiteSpace c || c = ')' || c = ']' || c = '}' || c = '|')

let linesInsideLiterals (parseTree: ParsedInput) =
    (HashSet<int>(), parseTree)
    ||> ParsedInput.fold (fun lines _ node ->
        match node with
        | SyntaxNode.SynExpr(SynExpr.Const(range = m))
        | SyntaxNode.SynExpr(SynExpr.InterpolatedString(range = m))
        | SyntaxNode.SynPat(SynPat.Const(range = m)) ->
            for line in m.StartLine + 1 .. m.EndLine do
                lines.Add(Line.toZ line) |> ignore
        | _ -> ()

        lines)

let usedNames (parseTree: ParsedInput) =
    (HashSet<string>(StringComparer.Ordinal), parseTree)
    ||> ParsedInput.fold (fun names _ node ->
        match node with
        | SyntaxNode.SynExpr(SynExpr.Ident ident)
        | SyntaxNode.SynExpr(SynExpr.LongIdent(longDotId = SynLongIdent(id = ident :: _)))
        | SyntaxNode.SynPat(SynPat.Named(ident = SynIdent(ident, _)))
        | SyntaxNode.SynPat(SynPat.LongIdent(longDotId = SynLongIdent(id = ident :: _))) -> names.Add ident.idText |> ignore
        | _ -> ()

        names)

let uniqueName (baseName: string) (used: HashSet<string>) =
    Seq.initInfinite (fun i -> if i = 0 then baseName else $"{baseName}{i}")
    |> Seq.find (used.Contains >> not)

/// The text of the span with every line after the first moved by as many columns as the first line moves to reach
/// column. Lines inside multi-line string literals are kept as they are; a line that would need a negative indentation
/// makes the whole text unavailable.
let tryIndentedText (sourceText: SourceText) (span: TextSpan) (column: int) (literalLines: HashSet<int>) =
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
                match literalLines.Contains lineNumber with
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

let private isExtractableShape (expr: SynExpr) (path: SyntaxVisitorPath) =
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
        | SyntaxNode.SynExpr(SynExpr.InterpolatedString _ | SynExpr.Quote _ | SynExpr.Lazy _) :: _, _ -> true
        | SyntaxNode.SynExpr(SynExpr.While(whileExpr = condition) | SynExpr.WhileBang(whileExpr = condition)) :: _, SyntaxNode.SynExpr expr when
            isSame condition expr
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

/// Whether the expression contains a computation expression construct outside any computation expression of its own.
let private hasStatementOnlyConstruct (expr: SynExpr) =
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

let tryExtractionTarget (sourceText: SourceText) (parseTree: ParsedInput) (selection: TextSpan) =
    let span = trimmed sourceText selection

    match parseTree with
    | ParsedInput.ImplFile file when not span.IsEmpty ->
        let start = positionOf sourceText span.Start
        let finish = positionOf sourceText span.End

        let crossesDirective =
            file.Trivia.ConditionalDirectives
            |> List.exists (fun directive ->
                let line = directiveLine directive
                line >= start.Line && line <= finish.Line)

        let exact =
            (start, parseTree)
            ||> ParsedInput.tryPickLast (fun path node ->
                match node with
                | SyntaxNode.SynExpr expr when Position.posEq expr.Range.Start start && Position.posEq expr.Range.End finish ->
                    Some(expr, path)
                | _ -> None)

        match exact with
        | Some(expr, path) when
            not crossesDirective
            && isExtractableShape expr path
            && not (isInExcludedContext expr path)
            && not (hasStatementOnlyConstruct expr)
            ->
            let content =
                match expr with
                | SynExpr.Paren(expr = inner) -> textSpanOf sourceText inner.Range
                | _ -> span

            let replaced =
                match expr with
                | SynExpr.Paren _ when isCall path -> content
                | _ -> span

            ValueSome
                {
                    Expr = expr
                    Path = path
                    Content = content
                    Replaced = replaced
                }
        | _ -> ValueNone
    | _ -> ValueNone

let private isFunctionBinding (binding: SynBinding) =
    match binding with
    | SynBinding(headPat = SynPat.LongIdent(argPats = SynArgPats.Pats(_ :: _))) -> true
    | _ -> false

let private anchorOf (child: SyntaxNode) (parent: SyntaxNode) (grandparent: SyntaxNode voption) =
    match parent, child with
    | SyntaxNode.SynExpr(SynExpr.Sequential _), SyntaxNode.SynExpr expr -> ValueSome(Anchor.Statement expr.Range.Start)
    | SyntaxNode.SynExpr(SynExpr.LetOrUse letOrUse), SyntaxNode.SynExpr expr when isSame letOrUse.Body expr ->
        ValueSome(Anchor.Statement expr.Range.Start)
    | SyntaxNode.SynExpr(SynExpr.For(doBody = body) | SynExpr.ForEach(bodyExpr = body) | SynExpr.While(doExpr = body) | SynExpr.TryWith(
        tryExpr = body) | SynExpr.TryFinally(tryExpr = body) | SynExpr.ComputationExpr(expr = body)),
      SyntaxNode.SynExpr expr when isSame body expr -> ValueSome(Anchor.Statement expr.Range.Start)
    | SyntaxNode.SynModule(SynModuleDecl.Expr _), SyntaxNode.SynExpr expr -> ValueSome(Anchor.Statement expr.Range.Start)
    | SyntaxNode.SynBinding(SynBinding(expr = rhs; trivia = trivia) as binding), SyntaxNode.SynExpr expr when isSame rhs expr ->
        match grandparent, trivia.EqualsRange with
        | ValueSome(SyntaxNode.SynExpr(SynExpr.LetOrUse letOrUse)), _ when not letOrUse.IsRecursive && not (isFunctionBinding binding) ->
            ValueSome(Anchor.Statement letOrUse.Range.Start)
        | _, Some equals -> ValueSome(Anchor.Clause(equals.End, expr.Range))
        | _ -> ValueNone
    | SyntaxNode.SynMatchClause(SynMatchClause(resultExpr = result; trivia = trivia)), SyntaxNode.SynExpr expr when isSame result expr ->
        match trivia.ArrowRange with
        | Some arrow -> ValueSome(Anchor.Clause(arrow.End, expr.Range))
        | None -> ValueNone
    | SyntaxNode.SynExpr(SynExpr.IfThenElse(thenExpr = thenExpr; elseExpr = elseExpr; trivia = trivia)), SyntaxNode.SynExpr expr ->
        match elseExpr, trivia.ElseKeyword with
        | _ when isSame thenExpr expr -> ValueSome(Anchor.Clause(trivia.ThenKeyword.End, expr.Range))
        | Some elseExpr, Some elseKeyword when isSame elseExpr expr -> ValueSome(Anchor.Clause(elseKeyword.End, expr.Range))
        | _ -> ValueNone
    | SyntaxNode.SynExpr(SynExpr.Lambda(parsedData = Some(_, body); trivia = trivia)), SyntaxNode.SynExpr expr when isSame body expr ->
        match trivia.ArrowRange with
        | Some arrow -> ValueSome(Anchor.Clause(arrow.End, expr.Range))
        | None -> ValueNone
    | _ -> ValueNone

/// The places a declaration used by the expression can go, innermost first.
let anchorsOf (expr: SynExpr) (path: SyntaxVisitorPath) =
    let rec loop (child: SyntaxNode) (path: SyntaxVisitorPath) =
        match path with
        | [] -> []
        | parent :: rest ->
            let grandparent =
                match rest with
                | node :: _ -> ValueSome node
                | [] -> ValueNone

            match anchorOf child parent grandparent with
            | ValueSome anchor -> anchor :: loop parent rest
            | ValueNone -> loop parent rest

    loop (SyntaxNode.SynExpr expr) path

let isLiteralConstant (expr: SynExpr) =
    match expr with
    | SynExpr.Const((SynConst.Bool _ | SynConst.SByte _ | SynConst.Byte _ | SynConst.Int16 _ | SynConst.UInt16 _ | SynConst.Int32 _ | SynConst.UInt32 _ | SynConst.Int64 _ | SynConst.UInt64 _ | SynConst.IntPtr _ | SynConst.UIntPtr _ | SynConst.Single _ | SynConst.Double _ | SynConst.Char _ | SynConst.String _),
                    _) -> true
    | _ -> false

/// The module-level let declaration containing the path, with its first binding.
let tryEnclosingModuleLet (path: SyntaxVisitorPath) =
    path
    |> List.tryPick (function
        | SyntaxNode.SynModule(SynModuleDecl.Let(bindings = binding :: _; range = m)) -> Some struct (binding, m)
        | _ -> None)

let private padding (width: int) = String(' ', width)

/// `header = <content>`, with a multi-line content starting on its own line at bodyColumn.
let private tryDeclaration (sourceText: SourceText) (content: TextSpan) (header: string) (bodyColumn: int) literalLines lineBreak =
    tryIndentedText sourceText content bodyColumn literalLines
    |> ValueOption.map (fun body ->
        let lines = sourceText.Lines

        if lines.GetLineFromPosition(content.Start).LineNumber = lines.GetLineFromPosition(content.End).LineNumber then
            $"{header} = {body}"
        else
            $"{header} ={lineBreak}{padding bodyColumn}{body}")

/// Changes that declare `header = <selection>` in front of the anchor and put replacement where the selection was.
/// A body that shares its line with its keyword moves to new lines under that keyword, when only closing brackets
/// and a comment follow it.
let tryDeclareInFront
    (sourceText: SourceText)
    (target: ExtractionTarget)
    (anchor: Anchor)
    (header: string)
    (replacement: string)
    (indentSize: int)
    (literalLines: HashSet<int>)
    =
    let lines = sourceText.Lines
    let lineBreak = lineBreakOf sourceText
    let replacementChange = TextChange(target.Replaced, replacement)

    let inFrontOfLine (start: pos) =
        let line = lines[Line.toZ start.Line]

        tryDeclaration sourceText target.Content header (start.Column + indentSize) literalLines lineBreak
        |> ValueOption.map (fun declaration ->
            [
                TextChange(TextSpan(line.Start, 0), $"{padding start.Column}{declaration}{lineBreak}")
                replacementChange
            ])

    match anchor with
    | Anchor.Statement start when isLineLeading sourceText start -> inFrontOfLine start
    | Anchor.Clause(_, body) when isLineLeading sourceText body.Start -> inFrontOfLine body.Start
    | Anchor.Clause(keywordEnd, body) when keywordEnd.Line = body.StartLine && restOfLineIsClosers sourceText body.End ->
        let keywordLine = lines[Line.toZ keywordEnd.Line]
        let keywordEndOffset = keywordLine.Start + keywordEnd.Column
        let bodySpan = textSpanOf sourceText body
        let newIndent = leadingSpaces sourceText keywordLine + indentSize
        let shift = newIndent - body.StartColumn

        let continuation =
            [
                for lineNumber in Line.toZ body.StartLine + 1 .. Line.toZ body.EndLine do
                    let line = lines[lineNumber]

                    let keep =
                        literalLines.Contains lineNumber
                        || String.IsNullOrWhiteSpace(line.ToString())
                        || (line.Start >= target.Replaced.Start && line.Start <= target.Replaced.End)

                    if not keep then
                        line
            ]

        let movable =
            continuation
            |> List.forall (fun line -> shift >= 0 || leadingSpaces sourceText line >= -shift)

        let separatedByWhitespace =
            String.IsNullOrWhiteSpace(sourceText.ToString(TextSpan.FromBounds(keywordEndOffset, bodySpan.Start)))

        if movable && separatedByWhitespace then
            tryDeclaration sourceText target.Content header (newIndent + indentSize) literalLines lineBreak
            |> ValueOption.map (fun declaration ->
                [
                    TextChange(
                        TextSpan.FromBounds(keywordEndOffset, bodySpan.Start),
                        $"{lineBreak}{padding newIndent}{declaration}{lineBreak}{padding newIndent}"
                    )

                    for line in continuation do
                        if shift > 0 then
                            TextChange(TextSpan(line.Start, 0), padding shift)
                        elif shift < 0 then
                            TextChange(TextSpan(line.Start, -shift), "")

                    replacementChange
                ]
                |> List.sortBy _.Span.Start)
        else
            ValueNone
    | _ -> ValueNone

/// Changes that declare `header = <selection>`, preceded by the attribute lines, in front of the module-level let
/// containing the selection, and put replacement where the selection was.
let tryDeclareInFrontOfModuleLet
    (sourceText: SourceText)
    (target: ExtractionTarget)
    (attributes: string list)
    (header: string)
    (replacement: string)
    (indentSize: int)
    (literalLines: HashSet<int>)
    =
    match tryEnclosingModuleLet target.Path with
    | Some(struct (SynBinding(xmlDoc = xmlDoc), declaration)) ->
        let firstLine =
            if xmlDoc.IsEmpty then
                declaration.StartLine
            else
                min declaration.StartLine xmlDoc.Range.StartLine

        let line = sourceText.Lines[Line.toZ firstLine]
        let indent = leadingSpaces sourceText line
        let lineBreak = lineBreakOf sourceText

        tryDeclaration sourceText target.Content header (indent + indentSize) literalLines lineBreak
        |> ValueOption.map (fun declaration ->
            let attributeLines =
                attributes
                |> List.map (fun attribute -> $"{padding indent}{attribute}{lineBreak}")
                |> String.concat ""

            [
                TextChange(TextSpan(line.Start, 0), $"{attributeLines}{padding indent}{declaration}{lineBreak}{lineBreak}")
                TextChange(target.Replaced, replacement)
            ])
    | None -> ValueNone
