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

let rec stripParenTypes (ty: SynType) =
    match ty with
    | SynType.Paren(innerType = inner) -> stripParenTypes inner
    | _ -> ty

let charAt (sourceText: SourceText) (position: int) =
    if position >= 0 && position < sourceText.Length then
        sourceText[position]
    else
        '\000'

let structKeyword (sourceText: SourceText) (m: range) =
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

let rec skipTrivia (sourceText: SourceText) (position: int) =
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

let structAt (sourceText: SourceText) (position: int) =
    if needsSpaceBefore sourceText position then
        " struct "
    else
        "struct "

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

[<NoComparison; NoEquality>]
type StructKind =
    {
        IsExpr: (SynExpr -> bool) -> SynExpr -> SyntaxVisitorPath -> bool
        IsPat: SynPat -> SyntaxVisitorPath -> bool
        // `M(p: int * int)` is still called as `M(1, 2)`: an annotation on a member's parameter list converts nothing else.
        IsParameterList: SynPat -> SyntaxVisitorPath -> bool
        IsType: SynType -> bool
        IsStruct: CaretNode -> bool
        ExprChanges: SourceText -> bool -> SynExpr -> SyntaxVisitorPath -> TextChange list voption
        PatChanges: SourceText -> bool -> SynPat -> SyntaxVisitorPath -> TextChange list voption
        TypeChanges: SourceText -> bool -> bool -> SynType -> TextChange list
    }

let rec private tryTypeAt (kind: StructKind) (position: pos) (ty: SynType) =
    if not (Range.rangeContainsPos ty.Range position) then
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

let tryCaretNode (kind: StructKind) (callsMethod: SynExpr -> bool) (caret: pos) (parseTree: ParsedInput) =
    let annotationAt (annotation: SynType) (annotated: Annotated) =
        tryTypeAt kind caret annotation
        |> ValueOption.map (fun node -> CaretNode.Type(node, annotated, isSame (stripParenTypes annotation) node))

    (ValueNone, parseTree)
    ||> ParsedInput.fold (fun found path node ->
        match node with
        | SyntaxNode.SynExpr expr when Range.rangeContainsPos expr.Range caret && kind.IsExpr callsMethod expr path ->
            ValueSome(CaretNode.Expr(expr, path))
        | SyntaxNode.SynPat pat when Range.rangeContainsPos pat.Range caret && isLambdaParameterAgain pat path -> found
        | SyntaxNode.SynPat pat when Range.rangeContainsPos pat.Range caret && kind.IsPat pat path -> ValueSome(CaretNode.Pat(pat, path))
        | SyntaxNode.SynPat(SynPat.Typed(targetType = annotation) as pat) when Range.rangeContainsPos annotation.Range caret ->
            match annotationAt annotation (Annotated.Pattern(pat, path)) with
            | ValueSome(CaretNode.Type(isWholeAnnotation = true)) when kind.IsParameterList pat path -> found
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

let canChange (kind: StructKind) (sourceText: SourceText) (caretNode: CaretNode) =
    let toStruct = not (kind.IsStruct caretNode)

    match caretNode with
    | CaretNode.Expr(node, path) -> (kind.ExprChanges sourceText toStruct node path).IsSome
    | CaretNode.Pat(node, path) -> (kind.PatChanges sourceText toStruct node path).IsSome
    | CaretNode.Type _ -> true
