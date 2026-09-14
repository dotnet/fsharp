// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.Features
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Text

open CancellableTasks

[<RequireQualifiedAccess>]
module private ActivePatternReturnConversion =

    [<RequireQualifiedAccess; NoComparison; NoEquality>]
    type Annotation =
        | Absent
        | Head of Ident
        | Unsupported

    [<NoComparison; NoEquality>]
    type Target =
        {
            IsStruct: bool
            Cases: Ident list
            Annotation: Annotation
            StructAttribute: struct (SynAttributeList * SynAttribute) voption
            Keyword: SynLeadingKeyword
            IsModuleLevel: bool
        }

    let private spanOf (sourceText: SourceText) (m: range) =
        RoslynHelpers.FSharpRangeToTextSpan(sourceText, m)

    /// Whether the name is a case or type of a value option (true) or of an option (false).
    let private structnessOf (name: string) =
        match name with
        | "Some"
        | "None"
        | "option"
        | "Option" -> ValueSome false
        | "ValueSome"
        | "ValueNone"
        | "voption"
        | "ValueOption" -> ValueSome true
        | _ -> ValueNone

    let private raisingFunctions =
        set
            [
                "failwith"
                "failwithf"
                "invalidArg"
                "invalidOp"
                "nullArg"
                "raise"
                "reraise"
            ]

    let private isSingleCasePartialActivePattern (name: Ident) =
        match name.idText.Split([| '|' |], StringSplitOptions.RemoveEmptyEntries) with
        | [| _; "_" |] -> name.idText.StartsWith("|", StringComparison.Ordinal)
        | _ -> false

    let rec private resultLeaves (expr: SynExpr) =
        [
            match expr with
            | SynExpr.Paren(expr = inner) -> yield! resultLeaves inner
            | SynExpr.IfThenElse(thenExpr = thenExpr; elseExpr = Some elseExpr) ->
                yield! resultLeaves thenExpr
                yield! resultLeaves elseExpr
            | SynExpr.Match(clauses = clauses)
            | SynExpr.MatchLambda(matchClauses = clauses) ->
                for SynMatchClause(resultExpr = result) in clauses do
                    yield! resultLeaves result
            | SynExpr.TryWith(tryExpr = tryExpr; withCases = clauses) ->
                yield! resultLeaves tryExpr

                for SynMatchClause(resultExpr = result) in clauses do
                    yield! resultLeaves result
            | SynExpr.TryFinally(tryExpr = body)
            | SynExpr.Sequential(expr2 = body)
            | SynExpr.Lambda(parsedData = Some(_, body)) -> yield! resultLeaves body
            | SynExpr.LetOrUse letOrUse when not letOrUse.IsBang -> yield! resultLeaves letOrUse.Body
            | _ -> expr
        ]

    let rec private applicationHead (expr: SynExpr) =
        match expr with
        | SynExpr.Ident ident -> ValueSome ident
        | SynExpr.App(isInfix = false; funcExpr = funcExpr) -> applicationHead funcExpr
        | _ -> ValueNone

    /// The option cases the results are built with, when every result is a case of one option kind or raises.
    let private tryCases (body: SynExpr) =
        let heads = resultLeaves body |> List.map applicationHead

        let isRecognized head =
            match head with
            | ValueSome(head: Ident) -> (structnessOf head.idText).IsSome || raisingFunctions.Contains head.idText
            | ValueNone -> false

        let cases =
            heads
            |> List.choose (function
                | ValueSome head when (structnessOf head.idText).IsSome -> Some head
                | _ -> None)

        match cases with
        | first :: _ when List.forall isRecognized heads ->
            let structness = structnessOf first.idText

            if cases |> List.forall (fun case -> structnessOf case.idText = structness) then
                structness |> ValueOption.map (fun isStruct -> struct (isStruct, cases))
            else
                ValueNone
        | _ -> ValueNone

    let private annotationOf (isStruct: bool) (returnInfo: SynBindingReturnInfo option) =
        match returnInfo with
        | None -> Annotation.Absent
        | Some(SynBindingReturnInfo(typeName = SynType.App(typeName = SynType.LongIdent(SynLongIdent(id = [ head ]))))) when
            structnessOf head.idText = ValueSome isStruct
            ->
            Annotation.Head head
        | Some _ -> Annotation.Unsupported

    let private isReturnStruct (attribute: SynAttribute) =
        match attribute.Target, List.tryLast attribute.TypeName.LongIdent with
        | Some target, Some name ->
            String.Equals(target.idText, "return", StringComparison.Ordinal)
            && (String.Equals(name.idText, "Struct", StringComparison.Ordinal)
                || String.Equals(name.idText, "StructAttribute", StringComparison.Ordinal))
        | _ -> false

    let private tryReturnStructAttribute (attributes: SynAttributes) =
        attributes
        |> Seq.tryPickV (fun list ->
            list.Attributes
            |> Seq.tryFindV isReturnStruct
            |> ValueOption.map (fun attribute -> struct (list, attribute)))

    /// Whether the caret is on the attributes, the keyword, the name or the parameters of the binding.
    let private isOnHeader (caret: pos) (attributes: SynAttributes) (keyword: SynLeadingKeyword) (headPat: SynPat) =
        let start =
            (keyword.Range.Start, attributes)
            ||> List.fold (fun start list ->
                if Position.posGeq start list.Range.Start then
                    list.Range.Start
                else
                    start)

        Position.posGeq caret start && Position.posGeq headPat.Range.End caret

    let tryTarget (caret: pos) (parseTree: ParsedInput) =
        (ValueNone, parseTree)
        ||> ParsedInput.fold (fun found path node ->
            match found, node with
            | ValueNone,
              SyntaxNode.SynBinding(SynBinding(
                  attributes = attributes
                  headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = [ name ])) as headPat
                  returnInfo = returnInfo
                  expr = body
                  trivia = trivia)) when
                isSingleCasePartialActivePattern name
                && isOnHeader caret attributes trivia.LeadingKeyword headPat
                ->
                let body =
                    match returnInfo, body with
                    | Some _, SynExpr.Typed(expr = inner) -> inner
                    | _ -> body

                match trivia.LeadingKeyword, tryCases body with
                | (SynLeadingKeyword.Let _ | SynLeadingKeyword.LetRec _ | SynLeadingKeyword.And _), ValueSome(struct (isStruct, cases)) ->
                    let structAttribute = tryReturnStructAttribute attributes

                    match annotationOf isStruct returnInfo with
                    | Annotation.Unsupported -> ValueNone
                    | _ when not isStruct && structAttribute.IsSome -> ValueNone
                    | annotation ->
                        ValueSome
                            {
                                IsStruct = isStruct
                                Cases = cases
                                Annotation = annotation
                                StructAttribute = structAttribute
                                Keyword = trivia.LeadingKeyword
                                IsModuleLevel =
                                    match path with
                                    | SyntaxNode.SynModule(SynModuleDecl.Let _) :: _ -> true
                                    | _ -> false
                            }
                | _ -> ValueNone
            | _ -> found)

    let private lineBreakOf (sourceText: SourceText) (line: TextLine) =
        match line.EndIncludingLineBreak - line.End with
        | 0 -> Environment.NewLine
        | length -> sourceText.ToString(TextSpan(line.End, length))

    /// Adds or removes the `Value`/`v` prefix: `Some` ⟷ `ValueSome`, `option` ⟷ `voption`.
    let private renamed (sourceText: SourceText) (isStruct: bool) (ident: Ident) =
        let start = (spanOf sourceText ident.idRange).Start
        let prefix = if Char.IsLower ident.idText[0] then "v" else "Value"

        if isStruct then
            TextChange(TextSpan(start, prefix.Length), "")
        else
            TextChange(TextSpan(start, 0), prefix)

    let private structAttributeInsertion (sourceText: SourceText) (target: Target) =
        let keyword = target.Keyword.Range
        let line = sourceText.Lines[Line.toZ keyword.StartLine]
        let indent = sourceText.ToString(TextSpan(line.Start, keyword.StartColumn))

        match target.Keyword with
        | SynLeadingKeyword.Let _
        | SynLeadingKeyword.LetRec _ when String.IsNullOrWhiteSpace indent ->
            TextChange(TextSpan(line.Start, 0), $"{indent}[<return: Struct>]{lineBreakOf sourceText line}")
        | _ -> TextChange(TextSpan((spanOf sourceText keyword).End, 0), " [<return: Struct>]")

    let private attributeRemoval (sourceText: SourceText) (list: SynAttributeList) (attribute: SynAttribute) =
        match list.Attributes with
        | [ _ ] ->
            let listSpan = spanOf sourceText list.Range
            let line = sourceText.Lines.GetLineFromPosition listSpan.Start
            let before = sourceText.ToString(TextSpan.FromBounds(line.Start, listSpan.Start))
            let after = sourceText.ToString(TextSpan.FromBounds(listSpan.End, line.End))

            if String.IsNullOrWhiteSpace before && String.IsNullOrWhiteSpace after then
                TextSpan.FromBounds(line.Start, line.EndIncludingLineBreak)
            else
                TextSpan(listSpan.Start, listSpan.Length + after.Length - after.TrimStart().Length)
        | attributes ->
            let index =
                attributes
                |> List.findIndex (fun other -> obj.ReferenceEquals(other, attribute))

            let attributeSpan = spanOf sourceText attribute.Range

            if index + 1 < attributes.Length then
                TextSpan.FromBounds(attributeSpan.Start, (spanOf sourceText (List.item (index + 1) attributes).Range).Start)
            else
                TextSpan.FromBounds((spanOf sourceText (List.item (index - 1) attributes).Range).End, attributeSpan.End)

    let changes (sourceText: SourceText) (target: Target) =
        [
            for case in target.Cases do
                renamed sourceText target.IsStruct case

            match target.Annotation with
            | Annotation.Head head -> renamed sourceText target.IsStruct head
            | Annotation.Absent
            | Annotation.Unsupported -> ()

            // Attributes are not permitted on local bindings: there the value option return type alone makes it struct.
            match target.IsStruct, target.StructAttribute with
            | false, _ when target.IsModuleLevel -> structAttributeInsertion sourceText target
            | true, ValueSome(struct (list, attribute)) -> TextChange(attributeRemoval sourceText list attribute, "")
            | _ -> ()
        ]
        |> List.sortBy _.Span.Start

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ConvertActivePatternReturn"); Shared>]
type internal FSharpConvertActivePatternReturnRefactoring [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static let hasSignatureFile (document: Document) =
        let signaturePath = document.FilePath + "i"

        document.Project.Documents
        |> Seq.exists (fun d -> String.Equals(d.FilePath, signaturePath, StringComparison.OrdinalIgnoreCase))

    override _.ComputeRefactoringsAsync context =
        cancellableTask {
            let document = context.Document

            if not document.IsFSharpSignatureFile then
                let! cancellationToken = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync cancellationToken
                let! parseResults = document.GetFSharpParseResultsAsync(nameof FSharpConvertActivePatternReturnRefactoring)

                let caret =
                    let linePosition = sourceText.Lines.GetLinePosition context.Span.Start
                    Position.mkPos (Line.fromZ linePosition.Line) linePosition.Character

                match ActivePatternReturnConversion.tryTarget caret parseResults.ParseTree with
                | ValueSome target when not (hasSignatureFile document) ->
                    let! _, langVersion = document.GetFsharpParsingOptionsAsync(nameof FSharpConvertActivePatternReturnRefactoring)

                    let isSupported =
                        target.IsStruct
                        || target.IsModuleLevel
                        || LanguageVersion(langVersion).SupportsFeature
                            LanguageFeature.BooleanReturningAndReturnTypeDirectedPartialActivePattern

                    if isSupported then
                        let title =
                            if target.IsStruct then
                                SR.UseOptionActivePatternReturn()
                            else
                                SR.UseStructActivePatternReturn()

                        let changedDocument =
                            cancellableTask {
                                let changes = ActivePatternReturnConversion.changes sourceText target
                                return document.WithText(sourceText.WithChanges changes)
                            }

                        context.RegisterRefactoring(CodeAction.Create(title, changedDocument, title))
                | _ -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
