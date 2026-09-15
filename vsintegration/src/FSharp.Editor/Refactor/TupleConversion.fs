// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.TupleConversion

open System

open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.Syntax

open StructConversion

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
    match if position > 0 then sourceText[position - 1] else ' ' with
    | c when Char.IsLetterOrDigit c || c = '_' || c = '\'' || c = '`' || c = ')' || c = ']' -> " struct "
    | _ -> "struct "

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
let private typeChanges (sourceText: SourceText) (toStruct: bool) (isWholeAnnotation: bool) (tupleType: SynType) =
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
let private isArgumentList (tuple: SynExpr) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) as paren) :: SyntaxNode.SynExpr(SynExpr.App(flag = ExprAtomicFlag.Atomic; argExpr = arg) | SynExpr.New(
        expr = arg)) :: _ -> isSame inner tuple && isSame arg paren
    | _ -> false

/// Changes giving a tuple expression the target kind; ValueNone when it is not a tuple or cannot change in place.
let private tryExprChanges (sourceText: SourceText) (toStruct: bool) (tuple: SynExpr) (path: SyntaxVisitorPath) =
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
let private tryPatChanges (sourceText: SourceText) (toStruct: bool) (tuple: SynPat) (path: SyntaxVisitorPath) =
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

/// Whether the tuple pattern is the parameter list of a member or constructor: its only argument, parenthesized or
/// (a struct tuple) not.
let private isParameterList (tuple: SynPat) (path: SyntaxVisitorPath) =
    let struct (argument, headPath) =
        match path with
        | SyntaxNode.SynPat(SynPat.Paren(pat = inner) as paren) :: rest when isSame inner tuple -> struct (paren, rest)
        | _ -> struct (tuple, path)

    match headPath with
    | SyntaxNode.SynPat(SynPat.LongIdent(argPats = SynArgPats.Pats [ only ])) :: SyntaxNode.SynBinding(SynBinding(
        valData = SynValData(memberFlags = Some _))) :: _ -> isSame only argument
    | _ -> false

let kind: StructKind =
    {
        IsExpr =
            fun expr path ->
                match expr with
                | SynExpr.Tuple _ -> not (isArgumentList expr path)
                | _ -> false
        IsPat =
            fun pat path ->
                match pat with
                | SynPat.Tuple _ -> not (isParameterList pat path)
                | _ -> false
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
