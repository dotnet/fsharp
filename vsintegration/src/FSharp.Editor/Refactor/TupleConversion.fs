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

let containsPos (m: range) (position: pos) =
    Position.posGeq position m.Start && Position.posGeq m.End position

let rec stripParenTypes (ty: SynType) =
    match ty with
    | SynType.Paren(innerType = inner) -> stripParenTypes inner
    | _ -> ty

/// `struct` and the blanks after it, at the start of a struct tuple's range.
let private structKeyword (sourceText: SourceText) (m: range) =
    let start = (spanOf sourceText m).Start
    let mutable finish = start + "struct".Length

    while finish < sourceText.Length && Char.IsWhiteSpace sourceText[finish] do
        finish <- finish + 1

    TextSpan.FromBounds(start, finish)

/// The position of the first character from the position on that is not a blank or in a comment.
let rec private skipTrivia (sourceText: SourceText) (position: int) =
    let at offset =
        if position + offset < sourceText.Length then
            sourceText[position + offset]
        else
            '\000'

    if position >= sourceText.Length then
        position
    elif Char.IsWhiteSpace(at 0) then
        skipTrivia sourceText (position + 1)
    elif at 0 = '/' && at 1 = '/' then
        let line = sourceText.Lines.GetLineFromPosition position
        skipTrivia sourceText line.EndIncludingLineBreak
    elif at 0 = '(' && at 1 = '*' && at 2 <> ')' then
        let mutable depth = 1
        let mutable current = position + 2

        while depth > 0 && current < sourceText.Length do
            if
                sourceText[current] = '('
                && current + 1 < sourceText.Length
                && sourceText[current + 1] = '*'
            then
                depth <- depth + 1
                current <- current + 2
            elif
                sourceText[current] = '*'
                && current + 1 < sourceText.Length
                && sourceText[current + 1] = ')'
            then
                depth <- depth - 1
                current <- current + 2
            else
                current <- current + 1

        skipTrivia sourceText current
    else
        position

/// Whether the character before the position ends an identifier, by the lexer's rule for identifier characters.
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

/// Whether text inserted at the position would run into the name or closing bracket before it: `f(a, b)`.
let needsSpaceBefore (sourceText: SourceText) (position: int) =
    isIdentifierCharBefore sourceText position
    || position > 0
       && match sourceText[position - 1] with
          | '`'
          | ')'
          | ']'
          | '}' -> true
          | _ -> false

/// The position of the `(` that, with its `)`, encloses only the span and blanks.
let private tryEnclosingParen (sourceText: SourceText) (span: TextSpan) =
    let mutable before = span.Start - 1

    while before >= 0 && Char.IsWhiteSpace sourceText[before] do
        before <- before - 1

    let mutable after = span.End

    while after < sourceText.Length && Char.IsWhiteSpace sourceText[after] do
        after <- after + 1

    if
        before >= 0
        && after < sourceText.Length
        && sourceText[before] = '('
        && sourceText[after] = ')'
    then
        ValueSome before
    else
        ValueNone

/// `struct ` to insert in front of the `(` at the position, after a space when it would run into a name: `f(a, b)`.
let private structAt (sourceText: SourceText) (position: int) =
    if needsSpaceBefore sourceText position then
        " struct "
    else
        "struct "

/// Whether the text between start and finish is a whole generic argument: `<` or `,` before it, `>` or `,` after.
let private isGenericArgument (sourceText: SourceText) (start: int) (finish: int) =
    let mutable before = start - 1

    while before >= 0 && Char.IsWhiteSpace sourceText[before] do
        before <- before - 1

    let mutable after = finish

    while after < sourceText.Length && Char.IsWhiteSpace sourceText[after] do
        after <- after + 1

    before >= 0
    && after < sourceText.Length
    && (sourceText[before] = '<' || sourceText[before] = ',')
    && (sourceText[after] = '>' || sourceText[after] = ',')

/// Changes giving a tuple type the target kind; a whole annotation also loses the parentheses `struct` needed.
let typeChanges (sourceText: SourceText) (toStruct: bool) (isWholeAnnotation: bool) (tupleType: SynType) =
    match tupleType with
    | SynType.Tuple(isStruct = isStruct; range = m) when isStruct <> toStruct ->
        let span = spanOf sourceText m

        if toStruct then
            match tryEnclosingParen sourceText span with
            | ValueSome openParen -> [ TextChange(TextSpan(openParen, 0), "struct ") ]
            | ValueNone ->
                [
                    TextChange(TextSpan(span.Start, 0), "struct (")
                    TextChange(TextSpan(span.End, 0), ")")
                ]
        else
            let keyword = structKeyword sourceText m
            let openParen = skipTrivia sourceText keyword.End

            if
                (isWholeAnnotation || isGenericArgument sourceText keyword.Start span.End)
                && openParen < span.End
                && sourceText[openParen] = '('
            then
                [
                    TextChange(keyword, "")
                    TextChange(TextSpan(openParen, 1), "")
                    TextChange(TextSpan(span.End - 1, 1), "")
                ]
            else
                [ TextChange(keyword, "") ]
    | _ -> []

/// Whether the applied expression names a method or constructor that takes one argument list, or a union case of
/// several fields: its parenthesized argument is an argument list. A name that does not resolve counts as one.
let callsMethod (sourceText: SourceText) (checkResults: FSharpCheckFileResults) (funcExpr: SynExpr) =
    let rec tryName (expr: SynExpr) =
        match expr with
        | SynExpr.Ident ident -> ValueSome ident
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = ids))
        | SynExpr.DotGet(longDotId = SynLongIdent(id = ids)) ->
            match List.tryLast ids with
            | Some ident -> ValueSome ident
            | None -> ValueNone
        | SynExpr.TypeApp(expr = inner) -> tryName inner
        | _ -> ValueNone

    match tryName funcExpr with
    | ValueNone -> false
    | ValueSome ident ->
        let line = sourceText.Lines[Line.toZ ident.idRange.EndLine].ToString()

        match checkResults.GetSymbolUseAtLocation(ident.idRange.EndLine, ident.idRange.EndColumn, line, [ ident.idText ]) with
        | Some symbolUse ->
            match symbolUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv ->
                mfv.IsMember
                && not mfv.IsProperty
                && not mfv.IsEvent
                && mfv.CurriedParameterGroups.Count <= 1
            | :? FSharpUnionCase as unionCase -> unionCase.Fields.Count > 1
            | :? FSharpEntity -> true
            | _ -> false
        | None -> true

/// Whether the tuple is the argument list of a method, constructor or union case call rather than a tuple value,
/// however the call is spaced: `M(a, b)` and `M (a, b)` alike.
let isArgumentList (callsMethod: SynExpr -> bool) (tuple: SynExpr) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) as paren) :: SyntaxNode.SynExpr(SynExpr.New(expr = arg)) :: _ ->
        isSame inner tuple && isSame arg paren
    | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) as paren) :: SyntaxNode.SynExpr(SynExpr.App(
        isInfix = false; funcExpr = funcExpr; argExpr = arg)) :: _ when isSame inner tuple && isSame arg paren -> callsMethod funcExpr
    | _ -> false

/// Whether the tuple pattern is the parameter list of a member or constructor: its only argument, parenthesized or
/// (a struct tuple) not.
let isParameterList (tuple: SynPat) (path: SyntaxVisitorPath) =
    let struct (argument, headPath) =
        match path with
        | SyntaxNode.SynPat(SynPat.Paren(pat = inner) as paren) :: rest when isSame inner tuple -> struct (paren, rest)
        | _ -> struct (tuple, path)

    match headPath with
    | SyntaxNode.SynPat(SynPat.LongIdent(argPats = SynArgPats.Pats [ only ])) :: SyntaxNode.SynBinding(SynBinding(
        valData = SynValData(memberFlags = Some _))) :: _ -> isSame only argument
    | _ -> false

/// Changes giving a tuple expression the target kind; ValueNone when it is not a tuple or cannot change in place.
let tryExprChanges (sourceText: SourceText) (toStruct: bool) (tuple: SynExpr) (path: SyntaxVisitorPath) =
    match tuple with
    | SynExpr.Tuple(isStruct = isStruct) when isStruct = toStruct -> ValueSome []
    | SynExpr.Tuple(range = m) when not toStruct -> ValueSome [ TextChange(structKeyword sourceText m, "") ]
    | SynExpr.Tuple(range = m) ->
        match path with
        | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner; range = parenRange)) :: _ when isSame inner tuple ->
            let start = (spanOf sourceText parenRange).Start
            ValueSome [ TextChange(TextSpan(start, 0), structAt sourceText start) ]
        | _ when m.StartLine = m.EndLine ->
            let span = spanOf sourceText m

            ValueSome
                [
                    TextChange(TextSpan(span.Start, 0), "struct (")
                    TextChange(TextSpan(span.End, 0), ")")
                ]
        | _ -> ValueNone
    | _ -> ValueNone

/// Changes giving a tuple pattern the target kind; ValueNone when it is not a tuple or cannot change in place.
let tryPatChanges (sourceText: SourceText) (toStruct: bool) (tuple: SynPat) (path: SyntaxVisitorPath) =
    match tuple with
    | SynPat.Tuple(isStruct = isStruct) when isStruct = toStruct -> ValueSome []
    | SynPat.Tuple(range = m) when not toStruct -> ValueSome [ TextChange(structKeyword sourceText m, "") ]
    | SynPat.Tuple(range = m) ->
        match path with
        | SyntaxNode.SynPat(SynPat.Paren(pat = inner; range = parenRange)) :: _ when isSame inner tuple ->
            let start = (spanOf sourceText parenRange).Start
            ValueSome [ TextChange(TextSpan(start, 0), structAt sourceText start) ]
        | _ when m.StartLine = m.EndLine ->
            let span = spanOf sourceText m

            ValueSome
                [
                    TextChange(TextSpan(span.Start, 0), "struct (")
                    TextChange(TextSpan(span.End, 0), ")")
                ]
        | _ -> ValueNone
    | _ -> ValueNone

/// The innermost tuple type within the type that contains the position.
let rec tryTupleTypeAt (position: pos) (ty: SynType) =
    if not (containsPos ty.Range position) then
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

/// What a tuple type under the caret annotates.
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

/// The innermost tuple expression, pattern or annotated tuple type under the caret.
let tryCaretNode (callsMethod: SynExpr -> bool) (caret: pos) (parseTree: ParsedInput) =
    let annotationAt (annotation: SynType) (annotated: Annotated) =
        tryTupleTypeAt caret annotation
        |> ValueOption.map (fun tuple -> CaretNode.Type(tuple, annotated, isSame (stripParenTypes annotation) tuple))

    (ValueNone, parseTree)
    ||> ParsedInput.fold (fun found path node ->
        match node with
        | SyntaxNode.SynExpr(SynExpr.Tuple(range = m) as tuple) when containsPos m caret && not (isArgumentList callsMethod tuple path) ->
            ValueSome(CaretNode.Expr(tuple, path))
        | SyntaxNode.SynPat(SynPat.Tuple(range = m) as tuple) when containsPos m caret && not (isParameterList tuple path) ->
            ValueSome(CaretNode.Pat(tuple, path))
        | SyntaxNode.SynPat(SynPat.Typed(targetType = annotation) as pat) when containsPos annotation.Range caret ->
            annotationAt annotation (Annotated.Pattern(pat, path))
            |> ValueOption.orElse found
        | SyntaxNode.SynExpr(SynExpr.Typed(targetType = annotation) as expr) when containsPos annotation.Range caret ->
            annotationAt annotation (Annotated.Expression(expr, path))
            |> ValueOption.orElse found
        | SyntaxNode.SynBinding(SynBinding(returnInfo = Some(SynBindingReturnInfo(typeName = annotation))) as binding) when
            containsPos annotation.Range caret
            ->
            annotationAt annotation (Annotated.Return binding) |> ValueOption.orElse found
        | SyntaxNode.SynTypeDefn(SynTypeDefn(
            typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Record(recordFieldsAndSpreads = fields), _))) ->
            fields
            |> Seq.tryPickV (function
                | SynFieldOrSpread.Field(SynField(fieldType = annotation) as field) when containsPos annotation.Range caret ->
                    annotationAt annotation (Annotated.Field field)
                | _ -> ValueNone)
            |> ValueOption.orElse found
        | _ -> found)
