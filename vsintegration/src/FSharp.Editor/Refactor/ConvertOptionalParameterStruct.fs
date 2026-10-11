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
open FSharp.Compiler.EditorServices
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

    let private isInFSharpCore (entityFullName: string) (symbol: FSharpSymbol) =
        let declaringEntityName =
            match symbol with
            | :? FSharpUnionCase as case -> case.DeclaringEntity.TryFullName
            | :? FSharpMemberOrFunctionOrValue as value -> value.DeclaringEntity |> Option.bind _.TryFullName
            | _ -> None

        match declaringEntityName with
        | Some name ->
            String.Equals(name, entityFullName, StringComparison.Ordinal)
            && String.Equals(symbol.Assembly.SimpleName, "FSharp.Core", StringComparison.Ordinal)
        | None -> false

    /// Whether the long identifier resolves to a value or union case of the FSharp.Core entity with the compiled full name.
    let private isFSharpCoreSymbol
        (checkResults: FSharpCheckFileResults)
        (sourceText: SourceText)
        (entityFullName: string)
        (longIdent: Ident list)
        =
        match List.tryLast longIdent with
        | None -> false
        | Some ident ->
            let line = sourceText.Lines[Line.toZ ident.idRange.EndLine].ToString()
            let names = longIdent |> List.map _.idText

            match checkResults.GetSymbolUseAtLocation(ident.idRange.EndLine, ident.idRange.EndColumn, line, names) with
            | None -> false
            | Some symbolUse -> isInFSharpCore entityFullName symbolUse.Symbol

    /// Whether the names, written at the position, can only resolve to a value or union case of the FSharp.Core entity
    /// with the compiled full name: completion lists every candidate in scope, the shadowed ones included.
    let private wouldBeFSharpCoreSymbol
        (checkResults: FSharpCheckFileResults)
        (sourceText: SourceText)
        (entityFullName: string)
        (names: string list)
        (at: pos)
        =
        match List.rev names with
        | [] -> false
        | name :: qualifier ->
            let partialName =
                {
                    QualifyingIdents = List.rev qualifier
                    PartialIdent = ""
                    EndColumn = at.Column
                    LastDotPos = None
                }

            let options =
                { FSharpCodeCompletionOptions.Default with
                    SuggestObsoleteSymbols = true
                }

            let candidates =
                checkResults.GetDeclarationListSymbols(
                    None,
                    at.Line,
                    sourceText.Lines[Line.toZ at.Line].ToString(),
                    partialName,
                    options = options
                )
                |> Seq.concat
                |> Seq.filter (fun candidate -> String.Equals(candidate.Symbol.DisplayName, name, StringComparison.Ordinal))
                |> Seq.toArray

            candidates.Length > 0
            && candidates
               |> Array.forall (fun candidate -> isInFSharpCore entityFullName candidate.Symbol)

    let private optionType (isStruct: bool) =
        if isStruct then
            "Microsoft.FSharp.Core.FSharpValueOption`1"
        else
            "Microsoft.FSharp.Core.FSharpOption`1"

    let private optionModule (isStruct: bool) =
        if isStruct then
            "Microsoft.FSharp.Core.ValueOption"
        else
            "Microsoft.FSharp.Core.OptionModule"

    let private operators (_isStruct: bool) = "Microsoft.FSharp.Core.Operators"

    /// `Some` ⟷ `ValueSome`, `Option` ⟷ `ValueOption`, `defaultArg` ⟷ `defaultValueArg`.
    let private counterpart (isStruct: bool) (ident: Ident) =
        match isStruct, ident.idText with
        | false, "defaultArg" -> "defaultValueArg"
        | true, "defaultValueArg" -> "defaultArg"
        | false, name -> "Value" + name
        | true, name -> name.Substring "Value".Length

    /// Whether the long identifier resolves to the FSharp.Core entity of this option kind,
    /// and to the entity of the other kind once its first identifier is replaced by its counterpart.
    let private convertsInFSharpCore
        (checkResults: FSharpCheckFileResults)
        (sourceText: SourceText)
        (isStruct: bool)
        (entityOf: bool -> string)
        (longIdent: Ident list)
        =
        match longIdent with
        | [] -> false
        | first :: rest ->
            let counterparts = counterpart isStruct first :: (rest |> List.map _.idText)

            isFSharpCoreSymbol checkResults sourceText (entityOf isStruct) longIdent
            && wouldBeFSharpCoreSymbol checkResults sourceText (entityOf (not isStruct)) counterparts first.idRange.Start

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

    /// Functions of both option modules that take the option last and return the same type for either kind,
    /// with the number of their arguments.
    let private moduleFunctions =
        Map
            [
                "contains", 2
                "count", 1
                "defaultValue", 2
                "defaultWith", 2
                "exists", 2
                "forall", 2
                "get", 1
                "isNone", 1
                "isSome", 1
                "iter", 2
                "toArray", 1
                "toList", 1
                "toNullable", 1
                "toObj", 1
            ]

    let private isStructAttribute (attribute: SynAttribute) =
        match List.tryLast attribute.TypeName.LongIdent with
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

    let rec private argumentCount (expr: SynExpr) =
        match expr with
        | SynExpr.App(isInfix = false; funcExpr = funcExpr) -> argumentCount funcExpr + 1
        | _ -> 0

    /// Whether the next argument of the application is the option that the module function takes last.
    let private takesOptionNext (name: Ident) (func: SynExpr) =
        match moduleFunctions.TryGetValue name.idText with
        | true, arity -> argumentCount func + 1 = arity
        | _ -> false

    let private tryModuleQualifier (checkResults: FSharpCheckFileResults) (sourceText: SourceText) (isStruct: bool) (func: SynExpr) =
        match functionOf func with
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ qualifier; name ])) when
            hasText (if isStruct then "ValueOption" else "Option") qualifier
            && takesOptionNext name func
            && convertsInFSharpCore checkResults sourceText isStruct optionModule [ qualifier; name ]
            ->
            ValueSome [ qualifier ]
        | _ -> ValueNone

    let private isCase (isStruct: bool) (ident: Ident) =
        if isStruct then
            hasText "ValueSome" ident || hasText "ValueNone" ident
        else
            hasText "Some" ident || hasText "None" ident

    let private tryClauseHeads
        (checkResults: FSharpCheckFileResults)
        (sourceText: SourceText)
        (isStruct: bool)
        (clauses: SynMatchClause list)
        =
        (ValueSome [], clauses)
        ||> List.fold (fun heads (SynMatchClause(pat = pat)) ->
            match heads, pat with
            | ValueSome heads, (SynPat.LongIdent(longDotId = SynLongIdent(id = [ head ])) | SynPat.Named(ident = SynIdent(head, _))) when
                isCase isStruct head
                ->
                if convertsInFSharpCore checkResults sourceText isStruct optionType [ head ] then
                    ValueSome(head :: heads)
                else
                    ValueNone
            | ValueSome heads, SynPat.Wild _ -> ValueSome heads
            | _ -> ValueNone)

    /// The identifiers to rename for one use of the parameter in the member body, when the use keeps its type.
    let private tryUseRenames
        (checkResults: FSharpCheckFileResults)
        (sourceText: SourceText)
        (isStruct: bool)
        (node: SynExpr)
        (path: SyntaxVisitorPath)
        =
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
            && convertsInFSharpCore checkResults sourceText isStruct operators [ func ]
            ->
            ValueSome [ func ]
        | _, SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = func; argExpr = arg)) :: _ when isSame arg node ->
            tryModuleQualifier checkResults sourceText isStruct func
        | _,
          SyntaxNode.SynExpr(SynExpr.App(isInfix = true; funcExpr = SingleIdent pipe; argExpr = arg)) :: SyntaxNode.SynExpr(SynExpr.App(
              isInfix = false; argExpr = func)) :: _ when
            isSame arg node
            && hasText "op_PipeRight" pipe
            && isFSharpCoreSymbol checkResults sourceText (operators isStruct) [ pipe ]
            ->
            tryModuleQualifier checkResults sourceText isStruct func
        | _, SyntaxNode.SynExpr(SynExpr.Match(expr = scrutinee; clauses = clauses)) :: _ when isSame scrutinee node ->
            tryClauseHeads checkResults sourceText isStruct clauses
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
    let tryBodyRenames
        (checkResults: FSharpCheckFileResults)
        (sourceText: SourceText)
        (parseTree: ParsedInput)
        (isStruct: bool)
        (uses: range seq)
        =
        (ValueSome [], uses)
        ||> Seq.fold (fun renames useRange ->
            match renames, tryUseNode parseTree useRange with
            | ValueSome renames, Some(node, path) ->
                tryUseRenames checkResults sourceText isStruct node path
                |> ValueOption.map (fun more -> [ yield! more; yield! renames ])
            | _ -> ValueNone)

    let private renamed (sourceText: SourceText) (isStruct: bool) (ident: Ident) =
        TextChange(spanOf sourceText ident.idRange, counterpart isStruct ident)

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

    let private valueChanges (checkResults: FSharpCheckFileResults) (sourceText: SourceText) (isStruct: bool) (value: SynExpr) =
        let valueSpan = spanOf sourceText value.Range
        let conversion = if isStruct then "toOption" else "ofOption"
        let inverse = if isStruct then "ofOption" else "toOption"

        match functionOf value, value with
        | SingleIdent head, _ when
            isCase isStruct head
            && convertsInFSharpCore checkResults sourceText isStruct optionType [ head ]
            ->
            [ renamed sourceText isStruct head ]
        | _,
          SynExpr.App(
              isInfix = false; funcExpr = SynExpr.LongIdent(longDotId = SynLongIdent(id = [ qualifier; name ])) as func; argExpr = inner) when
            hasText "ValueOption" qualifier
            && hasText inverse name
            && isFSharpCoreSymbol checkResults sourceText (optionModule true) [ qualifier; name ]
            ->
            let innerStart = (spanOf sourceText inner.Range).Start

            let trivia =
                sourceText.ToString(TextSpan.FromBounds((spanOf sourceText func.Range).End, innerStart))

            [
                TextChange(TextSpan.FromBounds(valueSpan.Start, innerStart), trivia.TrimStart())
            ]
        | _ ->
            let wrapper =
                if wouldBeFSharpCoreSymbol checkResults sourceText (optionModule true) [ "ValueOption"; conversion ] value.Range.Start then
                    $"ValueOption.{conversion}"
                else
                    $"Microsoft.FSharp.Core.ValueOption.{conversion}"

            if isAtomic value then
                [ TextChange(TextSpan(valueSpan.Start, 0), $"{wrapper} ") ]
            else
                [
                    TextChange(TextSpan(valueSpan.Start, 0), $"{wrapper} (")
                    TextChange(TextSpan(valueSpan.End, 0), ")")
                ]

    /// Whether the function of a call ends at the position, with or without its type arguments.
    let private endsAt (position: pos) (func: SynExpr) =
        Position.posEq func.Range.End position
        || match func with
           | SynExpr.TypeApp(expr = callee) -> Position.posEq callee.Range.End position
           | _ -> false

    /// Changes to the `?name = value` arguments of the call whose function ends at the use of the member.
    let callSiteChanges
        (checkResults: FSharpCheckFileResults)
        (sourceText: SourceText)
        (parseTree: ParsedInput)
        (isStruct: bool)
        (name: string)
        (useRange: range)
        =
        let arguments =
            (useRange.Start, parseTree)
            ||> ParsedInput.tryPickLast (fun _ node ->
                match node with
                | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = func; argExpr = arguments)) when endsAt useRange.End func ->
                    Some arguments
                | _ -> None)

        match arguments with
        | Some arguments ->
            optionalArgumentValues name arguments
            |> List.collect (valueChanges checkResults sourceText isStruct)
        | None -> []

    let private isOptionType (isStruct: bool) (ty: FSharpType) =
        ty.HasTypeDefinition
        && match ty.TypeDefinition.TryFullName with
           | Some name -> String.Equals(name, optionType isStruct, StringComparison.Ordinal)
           | None -> false

    let rec private typeParametersIn (ty: FSharpType) : FSharpType seq =
        if ty.IsGenericParameter then
            Seq.singleton ty
        else
            ty.GenericArguments |> Seq.collect typeParametersIn

    /// Whether the other overload has the signature the member gets once the parameter changes its option kind.
    let private hasConvertedSignature (parameter: Parameter) (mfv: FSharpMemberOrFunctionOrValue) (other: FSharpMemberOrFunctionOrValue) =
        let typeParameters =
            mfv.CurriedParameterGroups
            |> Seq.concat
            |> Seq.collect (fun own -> typeParametersIn own.Type)
            |> Seq.toArray

        // Each overload has method type parameters of its own, so they are matched by position.
        let ownTypeParameters =
            [
                for ownParameter, otherParameter in Seq.zip mfv.GenericParameters other.GenericParameters do
                    match typeParameters |> Array.tryFindV (fun ty -> ty.GenericParameter = ownParameter) with
                    | ValueSome ty -> otherParameter, ty
                    | ValueNone -> ()
            ]

        let becomes (own: FSharpParameter) (other: FSharpParameter) =
            let ownType = own.Type.StripAbbreviations()

            let otherType = other.Type.Instantiate(ownTypeParameters).StripAbbreviations()

            match own.Name with
            | Some name when String.Equals(name, parameter.Ident.idText, StringComparison.Ordinal) ->
                isOptionType (not parameter.IsStruct) otherType
                && otherType.GenericArguments[0] = ownType.GenericArguments[0]
            | _ -> otherType = ownType

        other.GenericParameters.Count = mfv.GenericParameters.Count
        && other.CurriedParameterGroups.Count = mfv.CurriedParameterGroups.Count
        && (mfv.CurriedParameterGroups, other.CurriedParameterGroups)
           ||> Seq.forall2 (fun own others -> own.Count = others.Count && Seq.forall2 becomes own others)

    /// Whether another overload already has the signature the member gets once the parameter changes its option kind.
    let collidesWithOverload (parameter: Parameter) (mfv: FSharpMemberOrFunctionOrValue) =
        match mfv.DeclaringEntity with
        | Some entity ->
            let overloads =
                entity.MembersFunctionsAndValues
                |> Seq.filter (fun other ->
                    String.Equals(other.LogicalName, mfv.LogicalName, StringComparison.Ordinal)
                    && other.IsInstanceMember = mfv.IsInstanceMember)
                |> Seq.toArray

            // The members listed by the entity share its type parameters; the symbol at a definition has copies of them.
            match overloads |> Array.tryFindV _.IsEffectivelySameAs(mfv) with
            | ValueSome own ->
                overloads
                |> Array.exists (fun other ->
                    not (obj.ReferenceEquals(other, own))
                    && hasConvertedSignature parameter own other)
            | ValueNone -> false
        | None -> false

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

    static let isConvertibleMember (parameter: OptionalParameterConversion.Parameter) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            not (
                mfv.IsOverrideOrExplicitInterfaceImplementation
                || mfv.IsDispatchSlot
                || mfv.IsConstructor
                || mfv.IsExtensionMember
                || OptionalParameterConversion.collidesWithOverload parameter mfv
            )
        | _ -> false

    /// Whether the project and the F# projects that depend on it can call members with value option parameters.
    static let supportsValueOptions (project: Project) =
        cancellableTask {
            let solution = project.Solution

            let projectIds =
                [
                    project.Id
                    yield! solution.GetProjectDependencyGraph().GetProjectsThatTransitivelyDependOnThisProject project.Id
                ]

            let mutable isSupported = true

            for projectId in projectIds do
                let candidate = solution.GetProject projectId

                if isSupported && candidate.IsFSharp then
                    let! _, _, parsingOptions, _ = candidate.GetFSharpCompilationOptionsAsync()

                    isSupported <-
                        LanguageVersion(parsingOptions.LangVersionText).SupportsFeature
                            LanguageFeature.SupportValueOptionsAsOptionalParameters

            return isSupported
        }

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
                    let! isSupported =
                        if parameter.IsStruct then
                            CancellableTask.singleton true
                        else
                            supportsValueOptions document.Project

                    if isSupported then
                        let! _, checkResults =
                            document.GetFSharpParseAndCheckResultsAsync(nameof FSharpConvertOptionalParameterStructRefactoring)

                        match
                            tryGetSymbolUse checkResults sourceText parameter.MemberName,
                            tryGetSymbolUse checkResults sourceText parameter.Ident
                        with
                        | Some memberUse, Some parameterUse when isConvertibleMember parameter memberUse.Symbol ->
                            let uses =
                                checkResults.GetUsesOfSymbolInFile(parameterUse.Symbol, cancellationToken = cancellationToken)
                                // Named arguments at call sites are uses of the parameter too; only the body is rewritten here.
                                |> Seq.filter (fun symbolUse ->
                                    not symbolUse.IsFromDefinition
                                    && Position.posGeq symbolUse.Range.Start parameter.MemberRange.Start
                                    && Position.posGeq parameter.MemberRange.End symbolUse.Range.End)
                                |> Seq.map _.Range

                            match
                                OptionalParameterConversion.tryBodyRenames
                                    checkResults
                                    sourceText
                                    parseResults.ParseTree
                                    parameter.IsStruct
                                    uses
                            with
                            | ValueSome renames ->
                                let title =
                                    if parameter.IsStruct then
                                        SR.UseOptionForOptionalParameter()
                                    else
                                        SR.UseValueOptionForOptionalParameter()

                                let definitionChanges =
                                    OptionalParameterConversion.definitionChanges sourceText parameter renames

                                let changedSolution () =
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

                                            let! useParseResults, useCheckResults =
                                                useDocument.GetFSharpParseAndCheckResultsAsync(
                                                    nameof FSharpConvertOptionalParameterStructRefactoring
                                                )

                                            let changes =
                                                [
                                                    if documentId = document.Id then
                                                        yield! definitionChanges

                                                    for _, useRange in documentUses do
                                                        yield!
                                                            OptionalParameterConversion.callSiteChanges
                                                                useCheckResults
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
                                            CancellableTask.start cancellationToken (changedSolution ())),
                                        title
                                    )

                                context.RegisterRefactoring action
                            | ValueNone -> ()
                        | _ -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
