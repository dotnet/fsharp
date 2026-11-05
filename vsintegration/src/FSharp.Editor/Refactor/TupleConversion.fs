// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.TupleConversion

open System

open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open StructConversion

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

let private typeChanges (sourceText: SourceText) (toStruct: bool) (isWholeAnnotation: bool) (tupleType: SynType) =
    match tupleType with
    | SynType.Tuple(isStruct = isStruct; range = m) when isStruct <> toStruct ->
        if toStruct then
            addStructChanges sourceText m
        else
            dropStructChanges sourceText isWholeAnnotation m
    | _ -> []

let private isArgumentList (callsMethod: SynExpr -> bool) (tuple: SynExpr) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) as paren) :: SyntaxNode.SynExpr(SynExpr.New(expr = arg)) :: _ ->
        isSame inner tuple && isSame arg paren
    | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) as paren) :: SyntaxNode.SynExpr(SynExpr.App(
        isInfix = false; funcExpr = funcExpr; argExpr = arg)) :: _ when isSame inner tuple && isSame arg paren -> callsMethod funcExpr
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

let private tryExprChanges (sourceText: SourceText) (toStruct: bool) (tuple: SynExpr) (path: SyntaxVisitorPath) =
    match tuple, path with
    | SynExpr.Tuple(isStruct = isStruct; range = m), SyntaxNode.SynExpr(SynExpr.Paren(expr = inner; range = parenRange)) :: _ when
        isSame inner tuple
        ->
        tupleChanges sourceText toStruct isStruct m (ValueSome parenRange)
    | SynExpr.Tuple(isStruct = isStruct; range = m), _ -> tupleChanges sourceText toStruct isStruct m ValueNone
    | _ -> ValueNone

let private tryPatChanges (sourceText: SourceText) (toStruct: bool) (tuple: SynPat) (path: SyntaxVisitorPath) =
    match tuple, path with
    | SynPat.Tuple(isStruct = isStruct; range = m), SyntaxNode.SynPat(SynPat.Paren(pat = inner; range = parenRange)) :: _ when
        isSame inner tuple
        ->
        tupleChanges sourceText toStruct isStruct m (ValueSome parenRange)
    | SynPat.Tuple(isStruct = isStruct; range = m), _ -> tupleChanges sourceText toStruct isStruct m ValueNone
    | _ -> ValueNone

let private isParameterList (pat: SynPat) (path: SyntaxVisitorPath) =
    let struct (argument, headPath) =
        match path with
        | SyntaxNode.SynPat(SynPat.Paren(pat = inner) as paren) :: rest when isSame inner pat -> struct (paren, rest)
        | _ -> struct (pat, path)

    match headPath with
    | SyntaxNode.SynPat(SynPat.LongIdent(argPats = SynArgPats.Pats [ only ])) :: SyntaxNode.SynBinding(SynBinding(
        valData = SynValData(memberFlags = Some _))) :: _ -> isSame only argument
    | _ -> false

let kind: StructKind =
    {
        IsExpr =
            fun callsMethod expr path ->
                match expr with
                | SynExpr.Tuple _ -> not (isArgumentList callsMethod expr path)
                | _ -> false
        IsPat =
            fun pat path ->
                match pat with
                | SynPat.Tuple _ -> not (isParameterList pat path)
                | _ -> false
        IsParameterList = isParameterList
        IsType =
            function
            | SynType.Tuple _ -> true
            | _ -> false
        IsStruct =
            function
            | CaretNode.Expr(node = SynExpr.Tuple(isStruct = isStruct))
            | CaretNode.Pat(node = SynPat.Tuple(isStruct = isStruct))
            | CaretNode.Type(node = SynType.Tuple(isStruct = isStruct)) -> isStruct
            | _ -> false
        ExprChanges = tryExprChanges
        PatChanges = tryPatChanges
        TypeChanges = typeChanges
    }
