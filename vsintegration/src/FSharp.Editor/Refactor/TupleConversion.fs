// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.TupleConversion

open System
open System.Globalization

open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

let spanOf (sourceText: SourceText) (m: range) =
    RoslynHelpers.FSharpRangeToTextSpan(sourceText, m)

let isSame (node: 'T) (other: 'T) = obj.ReferenceEquals(node, other)

let rec stripParenTypes (ty: SynType) =
    match ty with
    | SynType.Paren(innerType = inner) -> stripParenTypes inner
    | _ -> ty

let private charAt (sourceText: SourceText) (position: int) =
    if position >= 0 && position < sourceText.Length then
        sourceText[position]
    else
        '\000'

let private structKeyword (sourceText: SourceText) (m: range) =
    let start = (spanOf sourceText m).Start
    let mutable finish = start + "struct".Length

    while finish < sourceText.Length && Char.IsWhiteSpace sourceText[finish] do
        finish <- finish + 1

    TextSpan.FromBounds(start, finish)

let private endOfBlockComment (sourceText: SourceText) (start: int) =
    let mutable depth = 1
    let mutable current = start + 2

    while depth > 0 && current < sourceText.Length do
        match charAt sourceText current, charAt sourceText (current + 1) with
        | '(', '*' ->
            depth <- depth + 1
            current <- current + 2
        | '*', ')' ->
            depth <- depth - 1
            current <- current + 2
        | _ -> current <- current + 1

    current

let rec private skipTrivia (sourceText: SourceText) (position: int) =
    let at offset = charAt sourceText (position + offset)

    if position >= sourceText.Length then
        position
    elif Char.IsWhiteSpace(at 0) then
        skipTrivia sourceText (position + 1)
    elif at 0 = '/' && at 1 = '/' then
        let line = sourceText.Lines.GetLineFromPosition position
        skipTrivia sourceText line.EndIncludingLineBreak
    elif at 0 = '(' && at 1 = '*' && at 2 <> ')' then
        skipTrivia sourceText (endOfBlockComment sourceText position)
    else
        position

let isIdentifierCharBefore (sourceText: SourceText) (position: int) =
    if position <= 0 then
        false
    else
        let last = sourceText[position - 1]

        let category =
            if
                Char.IsLowSurrogate last
                && position >= 2
                && Char.IsHighSurrogate sourceText[position - 2]
            then
                CharUnicodeInfo.GetUnicodeCategory(String([| sourceText[position - 2]; last |]), 0)
            else
                CharUnicodeInfo.GetUnicodeCategory last

        match category with
        | UnicodeCategory.UppercaseLetter
        | UnicodeCategory.LowercaseLetter
        | UnicodeCategory.TitlecaseLetter
        | UnicodeCategory.ModifierLetter
        | UnicodeCategory.OtherLetter
        | UnicodeCategory.LetterNumber
        | UnicodeCategory.DecimalDigitNumber
        | UnicodeCategory.ConnectorPunctuation
        | UnicodeCategory.NonSpacingMark
        | UnicodeCategory.SpacingCombiningMark
        | UnicodeCategory.Format -> true
        | _ -> last = '\''

let needsSpaceBefore (sourceText: SourceText) (position: int) =
    isIdentifierCharBefore sourceText position
    || match charAt sourceText (position - 1) with
       | '`'
       | ')'
       | ']'
       | '}'
       | '>' -> true
       | _ -> false

let private structAt (sourceText: SourceText) (position: int) =
    if needsSpaceBefore sourceText position then
        " struct "
    else
        "struct "

let private nonBlankAround (sourceText: SourceText) (span: TextSpan) =
    let mutable before = span.Start - 1

    while before >= 0 && Char.IsWhiteSpace sourceText[before] do
        before <- before - 1

    let mutable after = span.End

    while after < sourceText.Length && Char.IsWhiteSpace sourceText[after] do
        after <- after + 1

    struct (before, after)

let private tryEnclosingParen (sourceText: SourceText) (span: TextSpan) =
    let struct (before, after) = nonBlankAround sourceText span

    if charAt sourceText before = '(' && charAt sourceText after = ')' then
        ValueSome before
    else
        ValueNone

let private isGenericArgument (sourceText: SourceText) (span: TextSpan) =
    let struct (before, after) = nonBlankAround sourceText span

    (charAt sourceText before = '<' || charAt sourceText before = ',')
    && (charAt sourceText after = '>' || charAt sourceText after = ',')

let private insertStructParens (span: TextSpan) =
    [
        TextChange(TextSpan(span.Start, 0), "struct (")
        TextChange(TextSpan(span.End, 0), ")")
    ]

let private addStructChanges (sourceText: SourceText) (m: range) =
    let span = spanOf sourceText m

    match tryEnclosingParen sourceText span with
    | ValueSome openParen -> [ TextChange(TextSpan(openParen, 0), "struct ") ]
    | ValueNone -> insertStructParens span

let private dropStructChanges (sourceText: SourceText) (isWholeAnnotation: bool) (m: range) =
    let span = spanOf sourceText m
    let keyword = structKeyword sourceText m
    let openParen = skipTrivia sourceText keyword.End

    let hasRedundantParens =
        (isWholeAnnotation
         || isGenericArgument sourceText (TextSpan.FromBounds(keyword.Start, span.End)))
        && openParen < span.End
        && sourceText[openParen] = '('

    if hasRedundantParens then
        [
            TextChange(keyword, "")
            TextChange(TextSpan(openParen, 1), "")
            TextChange(TextSpan(span.End - 1, 1), "")
        ]
    else
        [ TextChange(keyword, "") ]

let typeChanges (sourceText: SourceText) (toStruct: bool) (isWholeAnnotation: bool) (tupleType: SynType) =
    match tupleType with
    | SynType.Tuple(isStruct = isStruct; range = m) when isStruct <> toStruct ->
        if toStruct then
            addStructChanges sourceText m
        else
            dropStructChanges sourceText isWholeAnnotation m
    | _ -> []

let rec tryCallee (expr: SynExpr) (applied: int) =
    match expr with
    | SynExpr.App(isInfix = false; funcExpr = funcExpr) -> tryCallee funcExpr (applied + 1)
    | SynExpr.TypeApp(expr = inner)
    | SynExpr.Paren(expr = inner) -> tryCallee inner applied
    | SynExpr.Ident ident -> ValueSome(struct (ident, applied))
    | SynExpr.LongIdent(longDotId = SynLongIdent(id = ids))
    | SynExpr.DotGet(longDotId = SynLongIdent(id = ids)) ->
        match List.tryLast ids with
        | Some ident -> ValueSome(struct (ident, applied))
        | None -> ValueNone
    | _ -> ValueNone

let symbolUseAtIdent (sourceText: SourceText) (checkResults: FSharpCheckFileResults) (ident: Ident) =
    let line = sourceText.Lines[Line.toZ ident.idRange.EndLine].ToString()
    checkResults.GetSymbolUseAtLocation(ident.idRange.EndLine, ident.idRange.EndColumn, line, [ ident.idText ])

let private takesArgumentList (symbol: FSharpSymbol) =
    match symbol with
    | :? FSharpMemberOrFunctionOrValue as mfv ->
        mfv.IsMember
        && not mfv.IsProperty
        && not mfv.IsEvent
        && mfv.CurriedParameterGroups.Count <= 1
    | :? FSharpUnionCase as unionCase ->
        unionCase.Fields.Count > 1
        || unionCase.Fields |> Seq.exists _.FieldType.IsTupleType
    | :? FSharpEntity -> true
    | _ -> false

let callsMethod (sourceText: SourceText) (checkResults: FSharpCheckFileResults) (funcExpr: SynExpr) =
    match tryCallee funcExpr 0 with
    | ValueSome(struct (ident, 0)) ->
        symbolUseAtIdent sourceText checkResults ident
        |> Option.forall (fun symbolUse -> takesArgumentList symbolUse.Symbol)
    | _ -> false

let isArgumentList (callsMethod: SynExpr -> bool) (tuple: SynExpr) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) as paren) :: SyntaxNode.SynExpr(SynExpr.New(expr = arg)) :: _ ->
        isSame inner tuple && isSame arg paren
    | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) as paren) :: SyntaxNode.SynExpr(SynExpr.App(
        isInfix = false; funcExpr = funcExpr; argExpr = arg)) :: _ when isSame inner tuple && isSame arg paren -> callsMethod funcExpr
    | _ -> false

let isParameterList (pat: SynPat) (path: SyntaxVisitorPath) =
    let struct (argument, headPath) =
        match path with
        | SyntaxNode.SynPat(SynPat.Paren(pat = inner) as paren) :: rest when isSame inner pat -> struct (paren, rest)
        | _ -> struct (pat, path)

    match headPath with
    | SyntaxNode.SynPat(SynPat.LongIdent(argPats = SynArgPats.Pats [ only ])) :: SyntaxNode.SynBinding(SynBinding(
        valData = SynValData(memberFlags = Some _))) :: _ -> isSame only argument
    | _ -> false

let private tupleChanges (sourceText: SourceText) (toStruct: bool) (isStruct: bool) (m: range) (enclosingParen: range voption) =
    if isStruct = toStruct then
        ValueSome []
    elif not toStruct then
        ValueSome [ TextChange(structKeyword sourceText m, "") ]
    else
        match enclosingParen with
        | ValueSome parenRange ->
            let start = (spanOf sourceText parenRange).Start
            ValueSome [ TextChange(TextSpan(start, 0), structAt sourceText start) ]
        | ValueNone when m.StartLine = m.EndLine -> ValueSome(insertStructParens (spanOf sourceText m))
        | ValueNone -> ValueNone

let tryExprChanges (sourceText: SourceText) (toStruct: bool) (tuple: SynExpr) (path: SyntaxVisitorPath) =
    match tuple, path with
    | SynExpr.Tuple(isStruct = isStruct; range = m), SyntaxNode.SynExpr(SynExpr.Paren(expr = inner; range = parenRange)) :: _ when
        isSame inner tuple
        ->
        tupleChanges sourceText toStruct isStruct m (ValueSome parenRange)
    | SynExpr.Tuple(isStruct = isStruct; range = m), _ -> tupleChanges sourceText toStruct isStruct m ValueNone
    | _ -> ValueNone

let tryPatChanges (sourceText: SourceText) (toStruct: bool) (tuple: SynPat) (path: SyntaxVisitorPath) =
    match tuple, path with
    | SynPat.Tuple(isStruct = isStruct; range = m), SyntaxNode.SynPat(SynPat.Paren(pat = inner; range = parenRange)) :: _ when
        isSame inner tuple
        ->
        tupleChanges sourceText toStruct isStruct m (ValueSome parenRange)
    | SynPat.Tuple(isStruct = isStruct; range = m), _ -> tupleChanges sourceText toStruct isStruct m ValueNone
    | _ -> ValueNone

let rec tryTupleTypeAt (position: pos) (ty: SynType) =
    if not (Range.rangeContainsPos ty.Range position) then
        ValueNone
    else
        let inner =
            match ty with
            | SynType.Paren(innerType = inner)
            | SynType.Array(elementType = inner)
            | SynType.WithGlobalConstraints(typeName = inner) -> tryTupleTypeAt position inner
            | SynType.App(typeName = typeName; typeArgs = typeArgs)
            | SynType.LongIdentApp(typeName = typeName; typeArgs = typeArgs) ->
                typeName :: typeArgs |> Seq.tryPickV (tryTupleTypeAt position)
            | SynType.Fun(argType = argType; returnType = returnType) -> [ argType; returnType ] |> Seq.tryPickV (tryTupleTypeAt position)
            | SynType.Tuple(path = segments) ->
                segments
                |> Seq.tryPickV (function
                    | SynTupleTypeSegment.Type element -> tryTupleTypeAt position element
                    | _ -> ValueNone)
            | _ -> ValueNone

        match inner, ty with
        | ValueSome _, _ -> inner
        | ValueNone, SynType.Tuple _ -> ValueSome ty
        | ValueNone, _ -> ValueNone

[<RequireQualifiedAccess; NoComparison; NoEquality>]
type Annotated =
    | Pattern of pat: SynPat * path: SyntaxVisitorPath
    | Expression of expr: SynExpr * path: SyntaxVisitorPath
    | Return of binding: SynBinding
    | Field of field: SynField

[<RequireQualifiedAccess; NoComparison; NoEquality>]
type CaretNode =
    | Expr of tuple: SynExpr * path: SyntaxVisitorPath
    | Pat of tuple: SynPat * path: SyntaxVisitorPath
    | Type of tuple: SynType * annotated: Annotated * isWholeAnnotation: bool

let isStructNode (node: CaretNode) =
    match node with
    | CaretNode.Expr(tuple = SynExpr.Tuple(isStruct = isStruct))
    | CaretNode.Pat(tuple = SynPat.Tuple(isStruct = isStruct))
    | CaretNode.Type(tuple = SynType.Tuple(isStruct = isStruct)) -> isStruct
    | _ -> false

// A lambda parameter that is not a plain name is matched again in the body, without its parentheses and annotation.
let private isLambdaParameterAgain (pat: SynPat) (path: SyntaxVisitorPath) =
    let rec lambdaArgs (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynMatchClause _ :: SyntaxNode.SynExpr(SynExpr.Match _) :: rest -> lambdaArgs rest
        | SyntaxNode.SynExpr(SynExpr.Lambda(parsedData = Some(args, _))) :: _ -> args
        | _ -> []

    let rec abovePatterns (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynPat _ :: rest -> abovePatterns rest
        | _ -> path

    match abovePatterns path with
    | SyntaxNode.SynMatchClause _ :: _ as clausePath ->
        lambdaArgs clausePath
        |> List.exists (fun arg -> Range.rangeContainsRange arg.Range pat.Range)
    | _ -> false

let tryCaretNode (callsMethod: SynExpr -> bool) (caret: pos) (parseTree: ParsedInput) =
    let annotationAt (annotation: SynType) (annotated: Annotated) =
        tryTupleTypeAt caret annotation
        |> ValueOption.map (fun tuple -> CaretNode.Type(tuple, annotated, isSame (stripParenTypes annotation) tuple))

    (ValueNone, parseTree)
    ||> ParsedInput.fold (fun found path node ->
        match node with
        | SyntaxNode.SynExpr(SynExpr.Tuple(range = m) as tuple) when
            Range.rangeContainsPos m caret && not (isArgumentList callsMethod tuple path)
            ->
            ValueSome(CaretNode.Expr(tuple, path))
        | SyntaxNode.SynPat pat when Range.rangeContainsPos pat.Range caret && isLambdaParameterAgain pat path -> found
        | SyntaxNode.SynPat(SynPat.Tuple(range = m) as tuple) when Range.rangeContainsPos m caret && not (isParameterList tuple path) ->
            ValueSome(CaretNode.Pat(tuple, path))
        | SyntaxNode.SynPat(SynPat.Typed(targetType = annotation) as pat) when Range.rangeContainsPos annotation.Range caret ->
            match annotationAt annotation (Annotated.Pattern(pat, path)) with
            | ValueSome(CaretNode.Type(isWholeAnnotation = true)) when isParameterList pat path -> found
            | node -> node |> ValueOption.orElse found
        | SyntaxNode.SynExpr(SynExpr.Typed(targetType = annotation) as expr) when Range.rangeContainsPos annotation.Range caret ->
            annotationAt annotation (Annotated.Expression(expr, path))
            |> ValueOption.orElse found
        | SyntaxNode.SynBinding(SynBinding(returnInfo = Some(SynBindingReturnInfo(typeName = annotation))) as binding) when
            Range.rangeContainsPos annotation.Range caret
            ->
            annotationAt annotation (Annotated.Return binding) |> ValueOption.orElse found
        | SyntaxNode.SynTypeDefn(SynTypeDefn(
            typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Record(recordFieldsAndSpreads = fields), _))) ->
            fields
            |> Seq.tryPickV (function
                | SynFieldOrSpread.Field(SynField(fieldType = annotation) as field) when Range.rangeContainsPos annotation.Range caret ->
                    annotationAt annotation (Annotated.Field field)
                | _ -> ValueNone)
            |> ValueOption.orElse found
        | _ -> found)

let canChange (sourceText: SourceText) (node: CaretNode) =
    let toStruct = not (isStructNode node)

    match node with
    | CaretNode.Expr(tuple, path) -> (tryExprChanges sourceText toStruct tuple path).IsSome
    | CaretNode.Pat(tuple, path) -> (tryPatChanges sourceText toStruct tuple path).IsSome
    | CaretNode.Type _ -> true
