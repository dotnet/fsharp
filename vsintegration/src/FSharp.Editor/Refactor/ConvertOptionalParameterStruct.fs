// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Features
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open CancellableTasks

[<RequireQualifiedAccess>]
module private OptionalParameterConversion =

    [<NoComparison; NoEquality>]
    type Parameter =
        {
            Ident: Ident
            OptionalValRange: range
            StructAttribute: struct (SynAttributeList * SynAttribute) voption
            MemberName: Ident
            MemberRange: range
        }

        member this.IsStruct = this.StructAttribute.IsSome

    let private spanOf (sourceText: SourceText) (m: range) =
        RoslynHelpers.FSharpRangeToTextSpan(sourceText, m)

    let private hasText (text: string) (ident: Ident) =
        String.Equals(ident.idText, text, StringComparison.Ordinal)

    let private isSame (expr: SynExpr) (other: SynExpr) = obj.ReferenceEquals(expr, other)

    let private isOperator (name: string) (expr: SynExpr) =
        match expr with
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ operator ])) -> hasText name operator
        | _ -> false

    [<return: Struct>]
    let private (|SingleIdent|_|) (expr: SynExpr) =
        match expr with
        | SynExpr.Ident ident
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ ident ])) -> ValueSome ident
        | _ -> ValueNone

    /// Functions of both option modules that take the option last and return the same type for either kind.
    let private moduleFunctions =
        set
            [
                "contains"
                "count"
                "defaultValue"
                "defaultWith"
                "exists"
                "forall"
                "get"
                "isNone"
                "isSome"
                "iter"
                "toArray"
                "toList"
                "toNullable"
                "toObj"
            ]

    let private isStructAttribute (attribute: SynAttribute) =
        attribute.Target.IsNone
        && match List.tryLast attribute.TypeName.LongIdent with
           | Some name -> hasText "Struct" name || hasText "StructAttribute" name
           | None -> false

    let rec private tryOptionalVal (pat: SynPat) =
        match pat with
        | SynPat.OptionalVal(ident, m) -> ValueSome(struct (ident, m))
        | SynPat.Typed(pat = inner)
        | SynPat.Attrib(pat = inner)
        | SynPat.Paren(pat = inner) -> tryOptionalVal inner
        | _ -> ValueNone

    let rec private attributesOf (pat: SynPat) =
        [
            match pat with
            | SynPat.Attrib(pat = inner; attributes = attributes) ->
                yield! attributes
                yield! attributesOf inner
            | SynPat.Typed(pat = inner)
            | SynPat.Paren(pat = inner) -> yield! attributesOf inner
            | _ -> ()
        ]

    let private tryStructAttribute (attributes: SynAttributes) =
        attributes
        |> Seq.tryPickV (fun list ->
            list.Attributes
            |> Seq.tryFindV isStructAttribute
            |> ValueOption.map (fun attribute -> struct (list, attribute)))

    /// The name and the whole range of the member whose parameters contain the pattern, when it is a member of a type.
    let rec private tryMember (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynBinding(SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = ids))) as binding) :: SyntaxNode.SynMemberDefn(SynMemberDefn.Member _) :: _ ->
            match List.tryLast ids with
            | Some name -> ValueSome(struct (name, binding.RangeOfBindingWithRhs))
            | None -> ValueNone
        | SyntaxNode.SynPat _ :: rest -> tryMember rest
        | _ -> ValueNone

    let tryParameter (caret: pos) (parseTree: ParsedInput) =
        (ValueNone, parseTree)
        ||> ParsedInput.fold (fun found path node ->
            match found, node with
            | ValueNone, SyntaxNode.SynPat pat when Position.posGeq caret pat.Range.Start && Position.posGeq pat.Range.End caret ->
                match tryOptionalVal pat, tryMember path with
                | ValueSome(struct (ident, m)), ValueSome(struct (memberName, memberRange)) ->
                    ValueSome
                        {
                            Ident = ident
                            OptionalValRange = m
                            StructAttribute = tryStructAttribute (attributesOf pat)
                            MemberName = memberName
                            MemberRange = memberRange
                        }
                | _ -> ValueNone
            | _ -> found)

    let rec private functionOf (expr: SynExpr) =
        match expr with
        | SynExpr.App(isInfix = false; funcExpr = funcExpr) -> functionOf funcExpr
        | _ -> expr

    let private tryModuleQualifier (isStruct: bool) (func: SynExpr) =
        match functionOf func with
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ qualifier; name ])) when
            hasText (if isStruct then "ValueOption" else "Option") qualifier
            && moduleFunctions.Contains name.idText
            ->
            ValueSome [ qualifier ]
        | _ -> ValueNone

    let private isCase (isStruct: bool) (ident: Ident) =
        if isStruct then
            hasText "ValueSome" ident || hasText "ValueNone" ident
        else
            hasText "Some" ident || hasText "None" ident

    let private tryClauseHeads (isStruct: bool) (clauses: SynMatchClause list) =
        (ValueSome [], clauses)
        ||> List.fold (fun heads (SynMatchClause(pat = pat)) ->
            match heads, pat with
            | ValueSome heads, (SynPat.LongIdent(longDotId = SynLongIdent(id = [ head ])) | SynPat.Named(ident = SynIdent(head, _))) when
                isCase isStruct head
                ->
                ValueSome(head :: heads)
            | ValueSome heads, (SynPat.Wild _ | SynPat.Named _) -> ValueSome heads
            | _ -> ValueNone)

    /// The identifiers to rename for one use of the parameter in the member body, when the use keeps its type.
    let private tryUseRenames (isStruct: bool) (node: SynExpr) (path: SyntaxVisitorPath) =
        match node, path with
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = _ :: property :: _)), _ when
            hasText "IsSome" property
            || hasText "IsNone" property
            || hasText "Value" property
            ->
            ValueSome []
        | _, SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = SingleIdent func; argExpr = arg)) :: _ when
            isSame arg node
            && hasText (if isStruct then "defaultValueArg" else "defaultArg") func
            ->
            ValueSome [ func ]
        | _, SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = func; argExpr = arg)) :: _ when isSame arg node ->
            tryModuleQualifier isStruct func
        | _,
          SyntaxNode.SynExpr(SynExpr.App(isInfix = true; funcExpr = pipe; argExpr = arg)) :: SyntaxNode.SynExpr(SynExpr.App(
              isInfix = false; argExpr = func)) :: _ when isSame arg node && isOperator "op_PipeRight" pipe ->
            tryModuleQualifier isStruct func
        | _, SyntaxNode.SynExpr(SynExpr.Match(expr = scrutinee; clauses = clauses)) :: _ when isSame scrutinee node ->
            tryClauseHeads isStruct clauses
        | _ -> ValueNone

    let private tryUseNode (parseTree: ParsedInput) (useRange: range) =
        (useRange.Start, parseTree)
        ||> ParsedInput.tryPickLast (fun path node ->
            match node with
            | SyntaxNode.SynExpr(SynExpr.Ident ident as expr)
            | SyntaxNode.SynExpr(SynExpr.LongIdent(longDotId = SynLongIdent(id = ident :: _)) as expr) when
                Position.posEq ident.idRange.Start useRange.Start
                && Position.posEq ident.idRange.End useRange.End
                ->
                Some(expr, path)
            | _ -> None)

    /// The identifiers to rename in the member body, when every use of the parameter keeps its type.
    let tryBodyRenames (parseTree: ParsedInput) (isStruct: bool) (uses: range seq) =
        (ValueSome [], uses)
        ||> Seq.fold (fun renames useRange ->
            match renames, tryUseNode parseTree useRange with
            | ValueSome renames, Some(node, path) ->
                tryUseRenames isStruct node path
                |> ValueOption.map (fun more -> [ yield! more; yield! renames ])
            | _ -> ValueNone)

    /// `Some` ⟷ `ValueSome`, `Option` ⟷ `ValueOption`, `defaultArg` ⟷ `defaultValueArg`.
    let private renamed (sourceText: SourceText) (isStruct: bool) (ident: Ident) =
        let text =
            match isStruct, ident.idText with
            | false, "defaultArg" -> "defaultValueArg"
            | true, "defaultValueArg" -> "defaultArg"
            | false, name -> "Value" + name
            | true, name -> name.Substring "Value".Length

        TextChange(spanOf sourceText ident.idRange, text)

    let private attributeRemoval (sourceText: SourceText) (list: SynAttributeList) (attribute: SynAttribute) =
        match list.Attributes with
        | [ _ ] ->
            let listSpan = spanOf sourceText list.Range
            let line = sourceText.Lines.GetLineFromPosition listSpan.Start
            let after = sourceText.ToString(TextSpan.FromBounds(listSpan.End, line.End))
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

    let definitionChanges (sourceText: SourceText) (parameter: Parameter) (renames: Ident list) =
        [
            match parameter.StructAttribute with
            | ValueSome(struct (list, attribute)) -> TextChange(attributeRemoval sourceText list attribute, "")
            | ValueNone -> TextChange(TextSpan((spanOf sourceText parameter.OptionalValRange).Start, 0), "[<Struct>] ")

            for ident in renames do
                renamed sourceText parameter.IsStruct ident
        ]

    let private isAtomic (expr: SynExpr) =
        match expr with
        | SynExpr.Ident _
        | SynExpr.LongIdent _
        | SynExpr.Paren _
        | SynExpr.Const _ -> true
        | _ -> false

    /// The values passed as `?name = value` in the arguments of a call.
    let private optionalArgumentValues (name: string) (arguments: SynExpr) =
        let arguments =
            match arguments with
            | SynExpr.Paren(expr = SynExpr.Tuple(exprs = exprs)) -> exprs
            | SynExpr.Paren(expr = single) -> [ single ]
            | _ -> []

        arguments
        |> List.choose (function
            | SynExpr.App(
                isInfix = false
                funcExpr = SynExpr.App(
                    isInfix = true
                    funcExpr = equals
                    argExpr = SynExpr.LongIdent(isOptional = true; longDotId = SynLongIdent(id = [ argumentName ])))
                argExpr = value) when isOperator "op_Equality" equals && hasText name argumentName -> Some value
            | _ -> None)

    let private valueChanges (sourceText: SourceText) (isStruct: bool) (value: SynExpr) =
        let valueSpan = spanOf sourceText value.Range
        let conversion = if isStruct then "toOption" else "ofOption"
        let inverse = if isStruct then "ofOption" else "toOption"

        match functionOf value, value with
        | SingleIdent head, _ when isCase isStruct head -> [ renamed sourceText isStruct head ]
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ qualifier; name ])), SynExpr.App(isInfix = false; argExpr = inner) when
            hasText "ValueOption" qualifier && hasText inverse name
            ->
            [ TextChange(valueSpan, sourceText.ToString(spanOf sourceText inner.Range)) ]
        | _ when isAtomic value -> [ TextChange(TextSpan(valueSpan.Start, 0), $"ValueOption.{conversion} ") ]
        | _ ->
            [
                TextChange(TextSpan(valueSpan.Start, 0), $"ValueOption.{conversion} (")
                TextChange(TextSpan(valueSpan.End, 0), ")")
            ]

    /// Changes to the `?name = value` arguments of the call whose function ends at the use of the member.
    let callSiteChanges (sourceText: SourceText) (parseTree: ParsedInput) (isStruct: bool) (name: string) (useRange: range) =
        let arguments =
            (useRange.Start, parseTree)
            ||> ParsedInput.tryPickLast (fun _ node ->
                match node with
                | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = func; argExpr = arguments)) when
                    Position.posEq func.Range.End useRange.End
                    ->
                    Some arguments
                | _ -> None)

        match arguments with
        | Some arguments ->
            optionalArgumentValues name arguments
            |> List.collect (valueChanges sourceText isStruct)
        | None -> []

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ConvertOptionalParameterStruct"); Shared>]
type internal FSharpConvertOptionalParameterStructRefactoring [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static let hasSignatureFile (document: Document) =
        let signaturePath = document.FilePath + "i"

        document.Project.Documents
        |> Seq.exists (fun d -> String.Equals(d.FilePath, signaturePath, StringComparison.OrdinalIgnoreCase))

    static let tryGetSymbolUse (checkResults: FSharpCheckFileResults) (sourceText: SourceText) (ident: Ident) =
        let line = sourceText.Lines[Line.toZ ident.idRange.EndLine].ToString()
        checkResults.GetSymbolUseAtLocation(ident.idRange.EndLine, ident.idRange.EndColumn, line, [ ident.idText ])

    static let isConvertibleMember (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            not (
                mfv.IsOverrideOrExplicitInterfaceImplementation
                || mfv.IsDispatchSlot
                || mfv.IsConstructor
                || mfv.IsExtensionMember
            )
        | _ -> false

    override _.ComputeRefactoringsAsync context =
        cancellableTask {
            let document = context.Document

            if not (document.IsFSharpSignatureFile || hasSignatureFile document) then
                let! cancellationToken = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync cancellationToken
                let! parseResults = document.GetFSharpParseResultsAsync(nameof FSharpConvertOptionalParameterStructRefactoring)

                let caret =
                    let linePosition = sourceText.Lines.GetLinePosition context.Span.Start
                    Position.mkPos (Line.fromZ linePosition.Line) linePosition.Character

                match OptionalParameterConversion.tryParameter caret parseResults.ParseTree with
                | ValueNone -> ()
                | ValueSome parameter ->
                    let! _, langVersion = document.GetFsharpParsingOptionsAsync(nameof FSharpConvertOptionalParameterStructRefactoring)

                    if
                        parameter.IsStruct
                        || LanguageVersion(langVersion).SupportsFeature LanguageFeature.SupportValueOptionsAsOptionalParameters
                    then
                        let! _, checkResults =
                            document.GetFSharpParseAndCheckResultsAsync(nameof FSharpConvertOptionalParameterStructRefactoring)

                        match
                            tryGetSymbolUse checkResults sourceText parameter.MemberName,
                            tryGetSymbolUse checkResults sourceText parameter.Ident
                        with
                        | Some memberUse, Some parameterUse when isConvertibleMember memberUse.Symbol ->
                            let uses =
                                checkResults.GetUsesOfSymbolInFile(parameterUse.Symbol, cancellationToken = cancellationToken)
                                // Named arguments at call sites are uses of the parameter too; only the body is rewritten here.
                                |> Seq.filter (fun symbolUse ->
                                    not symbolUse.IsFromDefinition
                                    && Position.posGeq symbolUse.Range.Start parameter.MemberRange.Start
                                    && Position.posGeq parameter.MemberRange.End symbolUse.Range.End)
                                |> Seq.map _.Range

                            match OptionalParameterConversion.tryBodyRenames parseResults.ParseTree parameter.IsStruct uses with
                            | ValueSome renames ->
                                let title =
                                    if parameter.IsStruct then
                                        SR.UseOptionForOptionalParameter()
                                    else
                                        SR.UseValueOptionForOptionalParameter()

                                let definitionChanges =
                                    OptionalParameterConversion.definitionChanges sourceText parameter renames

                                let changedSolution =
                                    cancellableTask {
                                        let! cancellationToken = CancellableTask.getCancellationToken ()
                                        let! memberUses = SymbolHelpers.getSymbolUses memberUse document checkResults

                                        let usesByDocument =
                                            memberUses
                                            |> Seq.groupBy (fun (useDocument: Document, _) -> useDocument.Id)
                                            |> Seq.toArray

                                        let mutable solution = document.Project.Solution

                                        for documentId, documentUses in usesByDocument do
                                            let useDocument = solution.GetDocument documentId
                                            let! text = useDocument.GetTextAsync cancellationToken

                                            let! useParseResults =
                                                useDocument.GetFSharpParseResultsAsync(
                                                    nameof FSharpConvertOptionalParameterStructRefactoring
                                                )

                                            let changes =
                                                [
                                                    if documentId = document.Id then
                                                        yield! definitionChanges

                                                    for _, useRange in documentUses do
                                                        yield!
                                                            OptionalParameterConversion.callSiteChanges
                                                                text
                                                                useParseResults.ParseTree
                                                                parameter.IsStruct
                                                                parameter.Ident.idText
                                                                useRange
                                                ]
                                                |> List.distinctBy _.Span
                                                |> List.sortBy _.Span.Start

                                            solution <- solution.WithDocumentText(documentId, text.WithChanges changes)

                                        if not (usesByDocument |> Array.exists (fun (documentId, _) -> documentId = document.Id)) then
                                            solution <- solution.WithDocumentText(document.Id, sourceText.WithChanges definitionChanges)

                                        return solution
                                    }

                                let action =
                                    CodeAction.Create(
                                        title,
                                        Func<CancellationToken, Task<Solution>>(fun cancellationToken ->
                                            CancellableTask.start cancellationToken changedSolution),
                                        title
                                    )

                                context.RegisterRefactoring action
                            | ValueNone -> ()
                        | _ -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
