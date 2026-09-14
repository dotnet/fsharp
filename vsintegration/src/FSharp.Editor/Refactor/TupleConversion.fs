// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.TupleConversion

open System

open Microsoft.CodeAnalysis.Text

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

            if isWholeAnnotation || isGenericArgument sourceText keyword.Start span.End then
                [
                    TextChange(TextSpan(keyword.Start, keyword.Length + 1), "")
                    TextChange(TextSpan(span.End - 1, 1), "")
                ]
            else
                [ TextChange(keyword, "") ]
    | _ -> []

/// Whether the tuple is the argument list of a method, constructor or union case call rather than a tuple value.
let isArgumentList (tuple: SynExpr) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) as paren) :: SyntaxNode.SynExpr(SynExpr.App(flag = ExprAtomicFlag.Atomic; argExpr = arg) | SynExpr.New(
        expr = arg)) :: _ -> isSame inner tuple && isSame arg paren
    | _ -> false

/// Changes giving a tuple expression the target kind; ValueNone when it is not a tuple or cannot change in place.
let tryExprChanges (sourceText: SourceText) (toStruct: bool) (tuple: SynExpr) (path: SyntaxVisitorPath) =
    match tuple with
    | SynExpr.Tuple(isStruct = isStruct) when isStruct = toStruct -> ValueSome []
    | SynExpr.Tuple(range = m) when not toStruct -> ValueSome [ TextChange(structKeyword sourceText m, "") ]
    | SynExpr.Tuple(range = m) ->
        match path with
        | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner; range = parenRange)) :: _ when isSame inner tuple ->
            ValueSome [ TextChange(TextSpan((spanOf sourceText parenRange).Start, 0), "struct ") ]
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
            ValueSome [ TextChange(TextSpan((spanOf sourceText parenRange).Start, 0), "struct ") ]
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
let tryCaretNode (caret: pos) (parseTree: ParsedInput) =
    let annotationAt (annotation: SynType) (annotated: Annotated) =
        tryTupleTypeAt caret annotation
        |> ValueOption.map (fun tuple -> CaretNode.Type(tuple, annotated, isSame (stripParenTypes annotation) tuple))

    (ValueNone, parseTree)
    ||> ParsedInput.fold (fun found path node ->
        match node with
        | SyntaxNode.SynExpr(SynExpr.Tuple(range = m) as tuple) when containsPos m caret && not (isArgumentList tuple path) ->
            ValueSome(CaretNode.Expr(tuple, path))
        | SyntaxNode.SynPat(SynPat.Tuple(range = m) as tuple) when containsPos m caret -> ValueSome(CaretNode.Pat(tuple, path))
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
