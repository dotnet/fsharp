// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Composition
open System.Threading

open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open RefactoringHelpers
open CancellableTasks

[<RequireQualifiedAccess>]
module private FunctionExtraction =

    [<NoComparison; NoEquality>]
    type Parameter =
        {
            Name: string
            Type: string
            IsMutable: bool
            Uses: range list
        }

    [<NoComparison; NoEquality>]
    type Captures =
        {
            Parameters: Parameter list
            UsesThis: bool
            UsesStructThis: bool
            UsesBase: bool
            TypeParameterDeclarations: range list
            MethodNameEnds: HashSet<pos>
        }

    let private tryEnclosingDeclaration (path: SyntaxVisitorPath) =
        path
        |> Seq.tryPickV (function
            | SyntaxNode.SynMemberDefn(SynMemberDefn.Member(range = m) | SynMemberDefn.LetBindings(range = m))
            | SyntaxNode.SynModule(SynModuleDecl.Let(range = m) | SynModuleDecl.Expr(range = m)) -> ValueSome m
            | _ -> ValueNone)

    let tryTargetInDeclaration (sourceText: SourceText) (parseTree: ParsedInput) (selection: TextSpan) =
        trySelectedTarget sourceText parseTree selection
        |> ValueOption.bind (fun target ->
            tryEnclosingDeclaration target.Path
            |> ValueOption.map (fun declaration -> struct (target, declaration)))

    let private tryEntityOf (fullType: FSharpType) =
        let stripped = fullType.StripAbbreviations()

        if stripped.HasTypeDefinition then
            ValueSome stripped.TypeDefinition
        else
            ValueNone

    let private isByRefLike (fullType: FSharpType) =
        tryEntityOf fullType
        |> ValueOption.exists (fun entity ->
            entity.IsByRef
            || entity.Attributes
               |> Seq.exists (fun attribute ->
                   String.Equals(attribute.AttributeType.CompiledName, "IsByRefLikeAttribute", StringComparison.Ordinal)))

    let private isStruct (fullType: FSharpType) =
        tryEntityOf fullType
        |> ValueOption.exists (fun entity -> entity.IsByRef || entity.IsValueType)

    let private (|CapturedLocal|_|) (selection: range) (scope: range) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as value when not value.IsModuleValueOrMember ->
            match symbol.DeclarationLocation with
            | Some declaration when
                Range.rangeContainsRange scope declaration
                && not (Range.rangeContainsRange selection declaration)
                ->
                Some(value, declaration)
            | _ -> None
        | _ -> None

    let private (|TypeParameterDeclaredOutside|_|) (selection: range) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpGenericParameter ->
            symbol.DeclarationLocation
            |> Option.filter (fun declaration -> not (Range.rangeContainsRange selection declaration))
        | _ -> None

    let private tryAddParameter
        (parameters: Dictionary<range, Parameter>)
        (value: FSharpMemberOrFunctionOrValue)
        (symbolUse: FSharpSymbolUse)
        (declaration: range)
        =
        match parameters.TryGetValue declaration, value.FullTypeSafe with
        | (true, parameter), _ ->
            parameters[declaration] <-
                { parameter with
                    Uses = symbolUse.Range :: parameter.Uses
                }

            true
        | (false, _), Some fullType when not (isByRefLike fullType) && value.GenericParameters.Count = 0 ->
            parameters[declaration] <-
                {
                    Name = value.DisplayName
                    Type = fullType.FormatWithConstraints symbolUse.DisplayContext
                    IsMutable = value.IsMutable
                    Uses = [ symbolUse.Range ]
                }

            true
        | _ -> false

    let private firstUse (parameter: Parameter) =
        parameter.Uses
        |> List.map (fun m -> struct (m.StartLine, m.StartColumn))
        |> List.min

    let tryCaptures (checkResults: FSharpCheckFileResults) (selection: range) (scope: range) (cancellationToken: CancellationToken) =
        let parameters = Dictionary<range, Parameter>(Range.comparer)
        let typeParameterDeclarations = ResizeArray<range>()
        let methodNameEnds = HashSet<pos>()
        let mutable usesThis = false
        let mutable usesStructThis = false
        let mutable usesBase = false
        let mutable capturable = true

        let usesInSelection =
            checkResults.GetAllUsesOfAllSymbolsInFile cancellationToken
            |> Seq.filter (fun symbolUse ->
                not symbolUse.IsFromDefinition
                && Range.rangeContainsRange selection symbolUse.Range)

        for symbolUse in usesInSelection do
            match symbolUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as value when value.IsMemberThisValue || value.IsConstructorThisValue ->
                usesThis <- true
                usesStructThis <- usesStructThis || (value.FullTypeSafe |> Option.exists isStruct)
            | :? FSharpMemberOrFunctionOrValue as value when value.IsBaseValue -> usesBase <- true
            | CapturedLocal selection scope (value, declaration) ->
                capturable <- capturable && tryAddParameter parameters value symbolUse declaration
            | :? FSharpMemberOrFunctionOrValue as value when value.IsMember -> methodNameEnds.Add symbolUse.Range.End |> ignore
            | TypeParameterDeclaredOutside selection declaration -> typeParameterDeclarations.Add declaration
            | _ -> ()

        if capturable then
            ValueSome
                {
                    Parameters = parameters.Values |> Seq.sortBy firstUse |> Seq.toList
                    UsesThis = usesThis
                    UsesStructThis = usesStructThis
                    UsesBase = usesBase
                    TypeParameterDeclarations = Seq.toList typeParameterDeclarations
                    MethodNameEnds = methodNameEnds
                }
        else
            ValueNone

    let readsMutableInDelayedCode (expr: SynExpr) (parameters: Parameter list) =
        let delayed =
            (ResizeArray<range>(), [ SyntaxNode.SynExpr expr ])
            ||> SyntaxNodes.fold (fun delayed _ node ->
                match node with
                | SyntaxNode.SynExpr(SynExpr.Lambda(range = m) | SynExpr.DotLambda(range = m) | SynExpr.MatchLambda(range = m) | SynExpr.Lazy(
                    range = m) | SynExpr.ObjExpr(range = m) | SynExpr.ComputationExpr(range = m)) -> delayed.Add m
                | SyntaxNode.SynBinding(SynBinding(headPat = SynPat.LongIdent(argPats = SynArgPats.Pats(_ :: _))) as binding) ->
                    delayed.Add binding.RangeOfBindingWithRhs
                | _ -> ()

                delayed)

        parameters
        |> List.exists (fun parameter ->
            parameter.IsMutable
            && parameter.Uses
               |> List.exists (fun m -> delayed |> Seq.exists (fun code -> Range.rangeContainsRange code m)))

    let assignsCapturedValue (expr: SynExpr) (parameters: Parameter list) =
        let assigned =
            (HashSet<pos>(), [ SyntaxNode.SynExpr expr ])
            ||> SyntaxNodes.fold (fun positions _ node ->
                match node with
                | SyntaxNode.SynExpr(SynExpr.LongIdentSet(longDotId = SynLongIdent(id = [ ident ])))
                | SyntaxNode.SynExpr(SynExpr.Set(targetExpr = SynExpr.Ident ident)) -> positions.Add ident.idRange.Start |> ignore
                | _ -> ()

                positions)

        parameters
        |> List.exists (fun parameter -> parameter.Uses |> List.exists (fun m -> assigned.Contains m.Start))

    let rec private identifierArguments (argument: SynExpr) =
        match argument with
        | SynExpr.Ident ident -> [ ident ]
        | SynExpr.Paren(expr = inner) -> identifierArguments inner
        | SynExpr.Tuple(exprs = arguments) -> arguments |> List.collect identifierArguments
        | _ -> []

    let positionsNeedingType (expr: SynExpr) (methodNameEnds: HashSet<pos>) =
        (HashSet<pos>(), [ SyntaxNode.SynExpr expr ])
        ||> SyntaxNodes.fold (fun positions _ node ->
            match node with
            | SyntaxNode.SynExpr(SynExpr.App(funcExpr = method; argExpr = argument)) when methodNameEnds.Contains method.Range.End ->
                positions.UnionWith(identifierArguments argument |> Seq.map _.idRange.Start)
            | SyntaxNode.SynExpr(SynExpr.LongIdent(longDotId = SynLongIdent(id = head :: _ :: _)))
            | SyntaxNode.SynExpr(SynExpr.DotGet(expr = SynExpr.Ident head))
            | SyntaxNode.SynExpr(SynExpr.DotIndexedGet(objectExpr = SynExpr.Ident head))
            | SyntaxNode.SynExpr(SynExpr.App(
                flag = ExprAtomicFlag.Atomic; funcExpr = SynExpr.Ident head; argExpr = SynExpr.ArrayOrListComputed _))
            | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = SynExpr.App(isInfix = true; argExpr = SynExpr.Ident head)))
            | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = SynExpr.App(isInfix = true); argExpr = SynExpr.Ident head)) ->
                positions.Add head.idRange.Start |> ignore
            | _ -> ()

            positions)

    let private curried (annotate: Parameter -> bool) (parameter: Parameter) =
        if annotate parameter then
            $"({parameter.Name}: {parameter.Type})"
        else
            parameter.Name

    let private tupled (annotate: Parameter -> bool) (parameter: Parameter) =
        if annotate parameter then
            $"{parameter.Name}: {parameter.Type}"
        else
            parameter.Name

    let private never (_: Parameter) = false

    let private curriedParameters (annotate: Parameter -> bool) (parameters: Parameter list) =
        if parameters.IsEmpty then
            "()"
        else
            parameters |> List.map (curried annotate) |> String.concat " "

    let private tupledParameters (annotate: Parameter -> bool) (parameters: Parameter list) =
        let declarations = parameters |> List.map (tupled annotate) |> String.concat ", "
        $"({declarations})"

    let private callNeedsParentheses (target: ExtractionTarget) =
        match target.Expr, target.Path with
        | SynExpr.Paren _, _ when target.Replaced = target.Extracted -> false
        | _, SyntaxNode.SynExpr(SynExpr.App _ | SynExpr.DotGet _ | SynExpr.DotIndexedGet _ | SynExpr.TypeApp _) :: _ -> true
        | expr,
          SyntaxNode.SynExpr(SynExpr.DotSet(targetExpr = receiver) | SynExpr.DotIndexedSet(objectExpr = receiver) | SynExpr.DotNamedIndexedPropertySet(
              targetExpr = receiver)) :: _ -> obj.ReferenceEquals(expr, receiver)
        | _ -> false

    let private isInRecursiveModuleLet (path: SyntaxVisitorPath) =
        path
        |> List.exists (function
            | SyntaxNode.SynModule(SynModuleDecl.Let(isRecursive = true)) -> true
            | _ -> false)

    let private isPrivate (binding: SynBinding) =
        match binding with
        | SynBinding(accessibility = Some(SynAccess.Private _))
        | SynBinding(headPat = SynPat.LongIdent(accessibility = Some(SynAccess.Private _)))
        | SynBinding(headPat = SynPat.Named(accessibility = Some(SynAccess.Private _))) -> true
        | _ -> false

    let rec private isInExposedInline (path: SyntaxVisitorPath) =
        match path with
        | [] -> false
        | SyntaxNode.SynBinding(SynBinding(isInline = isInline) as binding) :: (SyntaxNode.SynModule _ | SyntaxNode.SynMemberDefn _) :: _ ->
            isInline && not (isPrivate binding)
        | _ :: rest -> isInExposedInline rest

    let private bindsName (name: string) (binding: SynBinding) =
        (false, [ SyntaxNode.SynBinding binding ])
        ||> SyntaxNodes.fold (fun found _ node ->
            match node with
            | SyntaxNode.SynPat(SynPat.Named(ident = SynIdent(ident, _)))
            | SyntaxNode.SynPat(SynPat.LongIdent(longDotId = SynLongIdent(id = [ ident ]))) ->
                found || String.Equals(ident.idText, name, StringComparison.Ordinal)
            | _ -> found)

    let private tryEnclosingMember (path: SyntaxVisitorPath) =
        let rec loop (path: SyntaxVisitorPath) =
            match path with
            | SyntaxNode.SynMemberDefn(SynMemberDefn.Member(memberDefn = binding; range = memberRange)) :: SyntaxNode.SynTypeDefn(SynTypeDefn(
                typeInfo = info)) :: _ -> ValueSome(struct (binding, memberRange, info))
            | (SyntaxNode.SynMemberDefn(SynMemberDefn.Interface _) | SyntaxNode.SynExpr(SynExpr.ObjExpr _)) :: _
            | [] -> ValueNone
            | _ :: rest -> loop rest

        loop path

    let private tryMemberCallee (binding: SynBinding) (info: SynComponentInfo) =
        match binding, info with
        | SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = [ self; _ ]))), _ when
            not (self.idText.StartsWith("_", StringComparison.Ordinal))
            && not (bindsName self.idText binding)
            ->
            let self = PrettyNaming.NormalizeIdentifierBackticks self.idText

            ValueSome
                struct {|
                    Declaration = $"member private {self}."
                    Receiver = self
                |}
        | SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = [ _ ]))), SynComponentInfo(typeParams = None) ->
            ValueSome
                struct {|
                    Declaration = "static member private "
                    Receiver = PrettyNaming.NormalizeIdentifierBackticks (List.last info.LongIdent).idText
                |}
        | _ -> ValueNone

    let private tryDeclareAfterMember (extraction: Extraction) (memberRange: range) (header: string) (call: string) =
        let sourceText = extraction.SourceText
        let lastLine = sourceText.Lines[Line.toZ memberRange.EndLine]

        let indent =
            leadingSpaces sourceText sourceText.Lines[Line.toZ memberRange.StartLine]

        tryDeclaration extraction header (indent + extraction.IndentSize)
        |> ValueOption.map (fun declaration ->
            let insertion =
                if lastLine.EndIncludingLineBreak > lastLine.End then
                    TextChange(TextSpan(lastLine.EndIncludingLineBreak, 0), $"{padding indent}{declaration}{extraction.LineBreak}")
                else
                    TextChange(TextSpan(lastLine.End, 0), $"{extraction.LineBreak}{padding indent}{declaration}")

            [ TextChange(extraction.Target.Replaced, call); insertion ])

    let private closureCannotCapture (captures: Captures) =
        captures.UsesBase || captures.UsesStructThis

    let private curriedFunction (extraction: Extraction) (captures: Captures) (annotate: Parameter -> bool) =
        let name = uniqueName "extractedFunction" extraction.Names
        let application = $"{name} {curriedParameters never captures.Parameters}"

        struct {|
            Header = $"{name} {curriedParameters annotate captures.Parameters}"
            Call =
                if callNeedsParentheses extraction.Target then
                    $"({application})"
                else
                    application
        |}

    let private localFunctionChanges (extraction: Extraction) (captures: Captures) (annotate: Parameter -> bool) =
        match anchorsOf extraction.Target.Expr extraction.Target.Path with
        | anchor :: _ when not (closureCannotCapture captures) ->
            let newFunction = curriedFunction extraction captures annotate
            tryDeclareInFront extraction anchor $"let {newFunction.Header}" newFunction.Call
        | _ -> ValueNone

    let private moduleFunctionChanges (extraction: Extraction) (captures: Captures) (annotate: Parameter -> bool) =
        let path = extraction.Target.Path

        if
            captures.UsesThis
            || captures.UsesBase
            || isInExposedInline path
            || not captures.TypeParameterDeclarations.IsEmpty
            || isInRecursiveModuleLet path
        then
            ValueNone
        else
            let newFunction = curriedFunction extraction captures annotate
            tryDeclareInFrontOfModuleLet extraction [] $"let private {newFunction.Header}" newFunction.Call

    let private privateMemberChanges (extraction: Extraction) (captures: Captures) (annotate: Parameter -> bool) =
        let declaresNoneOf (memberRange: range) =
            captures.TypeParameterDeclarations
            |> List.forall (fun declaration -> not (Range.rangeContainsRange memberRange declaration))

        match tryEnclosingMember extraction.Target.Path with
        | ValueSome(struct (binding, memberRange, info)) when not (isInExposedInline extraction.Target.Path) && declaresNoneOf memberRange ->
            tryMemberCallee binding info
            |> ValueOption.bind (fun callee ->
                let name = uniqueName "ExtractedMethod" extraction.Names

                let header =
                    $"{callee.Declaration}{name}{tupledParameters annotate captures.Parameters}"

                let call = $"{callee.Receiver}.{name}{tupledParameters never captures.Parameters}"
                tryDeclareAfterMember extraction memberRange header call)
        | _ -> ValueNone

    let private annotationOf (setting: ParameterAnnotationSetting) (needingType: HashSet<pos>) (parameter: Parameter) =
        match setting with
        | ParameterAnnotationSetting.Always -> true
        | ParameterAnnotationSetting.WhenNeeded -> parameter.Uses |> List.exists (fun m -> needingType.Contains m.Start)
        | ParameterAnnotationSetting.Never -> false

    let offer (extraction: Extraction) (declaration: range) (setting: ParameterAnnotationSetting) =
        let target = extraction.Target

        let selection =
            RoslynHelpers.TextSpanToFSharpRange(extraction.Context.Document.FilePath, target.Extracted, extraction.SourceText)

        match tryCaptures extraction.CheckResults selection declaration extraction.Context.CancellationToken with
        | ValueSome captures when
            not (assignsCapturedValue target.Expr captures.Parameters)
            && not (readsMutableInDelayedCode target.Expr captures.Parameters)
            ->
            let annotate =
                annotationOf setting (positionsNeedingType target.Expr captures.MethodNameEnds)

            let register = registerRefactoring extraction
            register (SR.ExtractToLocalFunction()) "local" (localFunctionChanges extraction captures annotate)
            register (SR.ExtractToModuleFunction()) "module" (moduleFunctionChanges extraction captures annotate)
            register (SR.ExtractToPrivateMember()) "member" (privateMemberChanges extraction captures annotate)
        | _ -> ()

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ExtractFunction"); Shared>]
type internal FSharpExtractFunctionRefactoring [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    override _.ComputeRefactoringsAsync context =
        cancellableTask {
            let document = context.Document

            if not document.IsFSharpSignatureFile then
                let! sourceText = document.GetTextAsync context.CancellationToken
                let! parseResults = document.GetFSharpParseResultsAsync(nameof FSharpExtractFunctionRefactoring)
                let parseTree = parseResults.ParseTree

                match FunctionExtraction.tryTargetInDeclaration sourceText parseTree context.Span with
                | ValueNone -> ()
                | ValueSome(struct (target, declaration)) ->
                    let! extraction = prepareExtraction context (nameof FSharpExtractFunctionRefactoring) sourceText parseTree target
                    FunctionExtraction.offer extraction declaration document.Project.FSharpExtractFunctionParameterAnnotations
        }
        |> CancellableTask.startAsTask context.CancellationToken
