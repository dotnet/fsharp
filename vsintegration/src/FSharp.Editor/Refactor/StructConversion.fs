// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.StructConversion

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

/// `struct` and the blanks after it, at the start of a struct node's range.
let structKeyword (sourceText: SourceText) (m: range) =
    let start = (spanOf sourceText m).Start
    let mutable finish = start + "struct".Length

    while finish < sourceText.Length && Char.IsWhiteSpace sourceText[finish] do
        finish <- finish + 1

    TextSpan.FromBounds(start, finish)

/// The position of the first character from the position on that is not a blank or in a comment.
let rec skipTrivia (sourceText: SourceText) (position: int) =
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

/// What a type under the caret annotates.
[<RequireQualifiedAccess; NoComparison; NoEquality>]
type Annotated =
    | Pattern of pat: SynPat * path: SyntaxVisitorPath
    | Expression of expr: SynExpr * path: SyntaxVisitorPath
    | Return of binding: SynBinding
    | Field of field: SynField

[<RequireQualifiedAccess; NoComparison; NoEquality>]
type CaretNode =
    | Expr of node: SynExpr * path: SyntaxVisitorPath
    | Pat of node: SynPat * path: SyntaxVisitorPath
    | Type of node: SynType * annotated: Annotated * isWholeAnnotation: bool

/// A kind of node written in a reference and a struct form: how to recognize it and how to change its form.
[<NoComparison; NoEquality>]
type StructKind =
    {
        /// Whether the expression is a node of the kind, not only shaped like one (a method's argument list), given
        /// whether an applied expression calls a method.
        IsExpr: (SynExpr -> bool) -> SynExpr -> SyntaxVisitorPath -> bool
        /// Whether the pattern is a node of the kind, not only shaped like one (a method's parameter list).
        IsPat: SynPat -> SyntaxVisitorPath -> bool
        IsType: SynType -> bool
        IsStruct: CaretNode -> bool
        /// Changes giving a node of the kind the target form; ValueNone when it cannot change in place.
        ExprChanges: SourceText -> bool -> SynExpr -> SyntaxVisitorPath -> TextChange list voption
        PatChanges: SourceText -> bool -> SynPat -> SyntaxVisitorPath -> TextChange list voption
        /// Changes giving a type of the kind the target form, knowing whether it is a whole annotation.
        TypeChanges: SourceText -> bool -> bool -> SynType -> TextChange list
    }

/// The innermost type of the kind within the type that contains the position.
let rec private tryTypeAt (kind: StructKind) (position: pos) (ty: SynType) =
    if not (containsPos ty.Range position) then
        ValueNone
    else
        let inner =
            match ty with
            | SynType.Paren(innerType = inner)
            | SynType.Array(elementType = inner)
            | SynType.WithGlobalConstraints(typeName = inner) -> tryTypeAt kind position inner
            | SynType.App(typeName = typeName; typeArgs = typeArgs)
            | SynType.LongIdentApp(typeName = typeName; typeArgs = typeArgs) ->
                typeName :: typeArgs |> Seq.tryPickV (tryTypeAt kind position)
            | SynType.Fun(argType = argType; returnType = returnType) -> [ argType; returnType ] |> Seq.tryPickV (tryTypeAt kind position)
            | SynType.Tuple(path = segments) ->
                segments
                |> Seq.tryPickV (function
                    | SynTupleTypeSegment.Type element -> tryTypeAt kind position element
                    | _ -> ValueNone)
            | SynType.AnonRecd(fields = fields) -> fields |> Seq.tryPickV (fun (_, fieldType) -> tryTypeAt kind position fieldType)
            | _ -> ValueNone

        match inner with
        | ValueSome _ -> inner
        | ValueNone when kind.IsType ty -> ValueSome ty
        | ValueNone -> ValueNone

/// The innermost expression, pattern or annotated type of the kind under the caret.
let tryCaretNode (kind: StructKind) (callsMethod: SynExpr -> bool) (caret: pos) (parseTree: ParsedInput) =
    let annotationAt (annotation: SynType) (annotated: Annotated) =
        tryTypeAt kind caret annotation
        |> ValueOption.map (fun node -> CaretNode.Type(node, annotated, isSame (stripParenTypes annotation) node))

    (ValueNone, parseTree)
    ||> ParsedInput.fold (fun found path node ->
        match node with
        | SyntaxNode.SynExpr expr when containsPos expr.Range caret && kind.IsExpr callsMethod expr path ->
            ValueSome(CaretNode.Expr(expr, path))
        | SyntaxNode.SynPat pat when containsPos pat.Range caret && kind.IsPat pat path -> ValueSome(CaretNode.Pat(pat, path))
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
