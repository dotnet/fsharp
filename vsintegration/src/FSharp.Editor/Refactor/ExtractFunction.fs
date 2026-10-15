// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Composition
open System.Threading

open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

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

    /// What the selection takes from the code around it.
    [<NoComparison; NoEquality>]
    type Captures =
        {
            /// The values read from the scope, in order of first use.
            Parameters: Parameter list
            UsesThis: bool
            /// The `this` of a struct is a byref, which a closure cannot capture.
            UsesStructThis: bool
            UsesBase: bool
            /// Where the type parameters the selection names are declared.
            TypeParameters: range list
            /// Where the names of the methods and constructors the selection calls end.
            MethodNameEnds: HashSet<pos>
        }

    let private padding (width: int) = String(' ', width)

    /// The declaration whose locals the extracted code may capture: the enclosing member or module-level declaration.
    let enclosingScope (path: SyntaxVisitorPath) =
        path
        |> List.tryPick (function
            | SyntaxNode.SynMemberDefn(SynMemberDefn.Member(range = m) | SynMemberDefn.LetBindings(range = m))
            | SyntaxNode.SynModule(SynModuleDecl.Let(range = m) | SynModuleDecl.Expr(range = m)) -> Some m
            | _ -> None)

    let private isByRefLike (fullType: FSharpType) =
        let stripped = fullType.StripAbbreviations()

        stripped.HasTypeDefinition
        && (stripped.TypeDefinition.IsByRef
            || stripped.TypeDefinition.Attributes
               |> Seq.exists (fun attribute ->
                   String.Equals(attribute.AttributeType.CompiledName, "IsByRefLikeAttribute", StringComparison.Ordinal)))

    let private isStruct (fullType: FSharpType) =
        let stripped = fullType.StripAbbreviations()

        stripped.HasTypeDefinition
        && (stripped.TypeDefinition.IsByRef || stripped.TypeDefinition.IsValueType)

    /// What the selection takes from its scope; ValueNone when a value it reads cannot become a parameter: one that has
    /// no type or a byref-like one, or a generic function, which a parameter would fix to one instance.
    let tryCaptures (checkResults: FSharpCheckFileResults) (selection: range) (scope: range) (cancellationToken: CancellationToken) =
        let parameters = Dictionary<range, Parameter>(Range.comparer)
        let typeParameters = ResizeArray<range>()
        let methodNameEnds = HashSet<pos>()
        let mutable usesThis = false
        let mutable usesStructThis = false
        let mutable usesBase = false
        let mutable capturable = true

        for symbolUse in checkResults.GetAllUsesOfAllSymbolsInFile cancellationToken do
            if
                not symbolUse.IsFromDefinition
                && Range.rangeContainsRange selection symbolUse.Range
            then
                match symbolUse.Symbol with
                | :? FSharpMemberOrFunctionOrValue as value when value.IsMemberThisValue || value.IsConstructorThisValue ->
                    usesThis <- true
                    usesStructThis <- usesStructThis || (value.FullTypeSafe |> Option.exists isStruct)
                | :? FSharpMemberOrFunctionOrValue as value when value.IsBaseValue -> usesBase <- true
                | :? FSharpMemberOrFunctionOrValue as value when not value.IsModuleValueOrMember ->
                    match symbolUse.Symbol.DeclarationLocation with
                    | Some declaration when
                        Range.rangeContainsRange scope declaration
                        && not (Range.rangeContainsRange selection declaration)
                        ->
                        match parameters.TryGetValue declaration, value.FullTypeSafe with
                        | (true, parameter), _ ->
                            parameters[declaration] <-
                                { parameter with
                                    Uses = symbolUse.Range :: parameter.Uses
                                }
                        | (false, _), Some fullType when not (isByRefLike fullType) && value.GenericParameters.Count = 0 ->
                            parameters[declaration] <-
                                {
                                    Name = value.DisplayName
                                    Type = fullType.FormatWithConstraints symbolUse.DisplayContext
                                    IsMutable = value.IsMutable
                                    Uses = [ symbolUse.Range ]
                                }
                        | (false, _), _ -> capturable <- false
                    | _ -> ()
                | :? FSharpMemberOrFunctionOrValue as value when value.IsMember -> methodNameEnds.Add symbolUse.Range.End |> ignore
                | :? FSharpGenericParameter ->
                    match symbolUse.Symbol.DeclarationLocation with
                    | Some declaration when not (Range.rangeContainsRange selection declaration) -> typeParameters.Add declaration
                    | _ -> ()
                | _ -> ()

        if capturable then
            let firstUse (parameter: Parameter) =
                parameter.Uses
                |> List.map (fun m -> struct (m.StartLine, m.StartColumn))
                |> List.min

            ValueSome
                {
                    Parameters = parameters.Values |> Seq.sortBy firstUse |> Seq.toList
                    UsesThis = usesThis
                    UsesStructThis = usesStructThis
                    UsesBase = usesBase
                    TypeParameters = Seq.toList typeParameters
                    MethodNameEnds = methodNameEnds
                }
        else
            ValueNone

    /// Whether the selection reads a captured mutable local inside a function or other delayed code. That code sees the
    /// assignments made before it runs, where a parameter would keep the value passed to the call.
    let readsMutableLater (expr: SynExpr) (parameters: Parameter list) =
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

    /// Whether the selection assigns to one of the captured values, which a parameter could not carry back.
    let assignsParameter (expr: SynExpr) (parameters: Parameter list) =
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

    /// Positions of identifiers whose type inference cannot tell from their use alone: receivers of a member or indexer
    /// access, operands of an operator and arguments of a method or constructor, which can have overloads.
    let positionsNeedingType (expr: SynExpr) (methodNameEnds: HashSet<pos>) =
        (HashSet<pos>(), [ SyntaxNode.SynExpr expr ])
        ||> SyntaxNodes.fold (fun positions _ node ->
            match node with
            | SyntaxNode.SynExpr(SynExpr.App(funcExpr = method; argExpr = argument)) when methodNameEnds.Contains method.Range.End ->
                for ident in identifierArguments argument do
                    positions.Add ident.idRange.Start |> ignore
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

    let private declared (annotate: Parameter -> bool) (parameter: Parameter) =
        if annotate parameter then
            $"{parameter.Name}: {parameter.Type}"
        else
            parameter.Name

    let curriedParameters (parameters: Parameter list) (annotate: Parameter -> bool) =
        match parameters with
        | [] -> "()"
        | _ ->
            parameters
            |> List.map (fun parameter ->
                if annotate parameter then
                    $"({declared annotate parameter})"
                else
                    parameter.Name)
            |> String.concat " "

    let curriedArguments (parameters: Parameter list) =
        match parameters with
        | [] -> "()"
        | _ -> parameters |> List.map _.Name |> String.concat " "

    let tupledParameters (parameters: Parameter list) (annotate: Parameter -> bool) =
        let declarations = parameters |> List.map (declared annotate) |> String.concat ", "
        $"({declarations})"

    let tupledArguments (parameters: Parameter list) =
        let arguments = parameters |> List.map _.Name |> String.concat ", "
        $"({arguments})"

    /// Whether a function application put where the selection was needs parentheses to stay one argument or the
    /// receiver of an assignment.
    let needsParentheses (target: ExtractionTarget) =
        match target.Expr, target.Path with
        | SynExpr.Paren _, _ when target.Replaced = target.Extracted -> false
        | _, SyntaxNode.SynExpr(SynExpr.App _ | SynExpr.DotGet _ | SynExpr.DotIndexedGet _ | SynExpr.TypeApp _) :: _ -> true
        | expr,
          SyntaxNode.SynExpr(SynExpr.DotSet(targetExpr = receiver) | SynExpr.DotIndexedSet(objectExpr = receiver) | SynExpr.DotNamedIndexedPropertySet(
              targetExpr = receiver)) :: _ -> obj.ReferenceEquals(expr, receiver)
        | _ -> false

    let isInRecursiveModuleLet (path: SyntaxVisitorPath) =
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

    /// Whether the selection is in an inline module-level function or member that is not private. Its body cannot
    /// call anything less accessible than it is.
    let rec isInExposedInline (path: SyntaxVisitorPath) =
        match path with
        | [] -> false
        | SyntaxNode.SynBinding(SynBinding(isInline = isInline) as binding) :: (SyntaxNode.SynModule _ | SyntaxNode.SynMemberDefn _) :: _ ->
            isInline && not (isPrivate binding)
        | _ :: rest -> isInExposedInline rest

    /// Whether a pattern in the member binds the name, which hides a self identifier of that name.
    let private bindsName (name: string) (binding: SynBinding) =
        (false, [ SyntaxNode.SynBinding binding ])
        ||> SyntaxNodes.fold (fun found _ node ->
            match node with
            | SyntaxNode.SynPat(SynPat.Named(ident = SynIdent(ident, _)))
            | SyntaxNode.SynPat(SynPat.LongIdent(longDotId = SynLongIdent(id = [ ident ]))) ->
                found || String.Equals(ident.idText, name, StringComparison.Ordinal)
            | _ -> found)

    /// The member of a class, record or union that contains the selection, outside interface implementations and
    /// object expressions.
    let tryEnclosingMember (path: SyntaxVisitorPath) =
        let rec loop (path: SyntaxVisitorPath) =
            match path with
            | SyntaxNode.SynMemberDefn(SynMemberDefn.Member(memberDefn = binding; range = memberRange)) :: SyntaxNode.SynTypeDefn(SynTypeDefn(
                typeInfo = info)) :: _ -> ValueSome(struct (binding, memberRange, info))
            | (SyntaxNode.SynMemberDefn(SynMemberDefn.Interface _) | SyntaxNode.SynExpr(SynExpr.ObjExpr _)) :: _
            | [] -> ValueNone
            | _ :: rest -> loop rest

        loop path

    /// The start of the new member's declaration and the receiver it is called on: the self identifier of an instance
    /// member, unless a local of the member hides it, or the name of a non-generic type for a static member.
    let tryMemberCallee (binding: SynBinding) (info: SynComponentInfo) =
        match binding, info with
        | SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = [ self; _ ]))), _ when
            not (self.idText.StartsWith("_", StringComparison.Ordinal))
            && not (bindsName self.idText binding)
            ->
            let self = PrettyNaming.NormalizeIdentifierBackticks self.idText
            ValueSome(struct ($"member private {self}.", self))
        | SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = [ _ ]))), SynComponentInfo(typeParams = None) ->
            ValueSome(struct ("static member private ", PrettyNaming.NormalizeIdentifierBackticks (List.last info.LongIdent).idText))
        | _ -> ValueNone

    /// Changes that declare `header = <selection>` right after the member and call it where the selection was.
    let tryMemberChanges (extraction: Extraction) (memberRange: range) (header: string) (call: string) =
        let sourceText = extraction.SourceText
        let target = extraction.Target
        let lineBreak = extraction.LineBreak
        let lines = sourceText.Lines
        let firstLine = lines[Line.toZ memberRange.StartLine]
        let lastLine = lines[Line.toZ memberRange.EndLine]
        let indent = leadingSpaces sourceText firstLine

        tryDeclaration extraction header (indent + extraction.IndentSize)
        |> ValueOption.map (fun declaration ->
            let insertion =
                if lastLine.EndIncludingLineBreak > lastLine.End then
                    TextChange(TextSpan(lastLine.EndIncludingLineBreak, 0), $"{padding indent}{declaration}{lineBreak}")
                else
                    TextChange(TextSpan(lastLine.End, 0), $"{lineBreak}{padding indent}{declaration}")

            [ TextChange(target.Replaced, call); insertion ])

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ExtractFunction"); Shared>]
type internal FSharpExtractFunctionRefactoring [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static let register
        (context: CodeRefactoringContext)
        (sourceText: SourceText)
        (title: string)
        (kind: string)
        (changes: TextChange list)
        =
        let changedDocument =
            cancellableTask {
                TelemetryReporter.ReportSingleEvent(
                    TelemetryEvents.RefactoringActivated,
                    [| "name", box (nameof FSharpExtractFunctionRefactoring); "kind", box kind |]
                )

                return context.Document.WithText(sourceText.WithChanges changes)
            }

        context.RegisterRefactoring(CodeAction.Create(title, changedDocument, title))

    override _.ComputeRefactoringsAsync context =
        cancellableTask {
            let document = context.Document

            if not document.IsFSharpSignatureFile then
                let! cancellationToken = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync cancellationToken
                let! parseResults = document.GetFSharpParseResultsAsync(nameof FSharpExtractFunctionRefactoring)

                let target = trySelectedTarget sourceText parseResults.ParseTree context.Span

                match target with
                | ValueNone -> ()
                | ValueSome target ->
                    match FunctionExtraction.enclosingScope target.Path with
                    | None -> ()
                    | Some scope ->
                        let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync(nameof FSharpExtractFunctionRefactoring)

                        let selection =
                            Range.mkRange
                                document.FilePath
                                (positionOf sourceText target.Extracted.Start)
                                (positionOf sourceText target.Extracted.End)

                        match FunctionExtraction.tryCaptures checkResults selection scope cancellationToken with
                        | ValueSome captures when
                            not (FunctionExtraction.assignsParameter target.Expr captures.Parameters)
                            && not (FunctionExtraction.readsMutableLater target.Expr captures.Parameters)
                            ->
                            let parameters = captures.Parameters
                            let! options = document.GetOptionsAsync cancellationToken

                            let indentSize =
                                options.GetOption(FormattingOptions.IndentationSize, FSharpConstants.FSharpLanguageName)

                            let! defines, langVersion = document.GetFsharpParsingOptionsAsync(nameof FSharpExtractFunctionRefactoring)

                            let literalLines =
                                linesInsideLiterals sourceText parseResults.ParseTree defines langVersion

                            let names = usedNames parseResults.ParseTree

                            let extraction =
                                {
                                    Context = context
                                    Provider = nameof FSharpExtractFunctionRefactoring
                                    SourceText = sourceText
                                    Target = target
                                    CheckResults = checkResults
                                    IndentSize = indentSize
                                    LiteralLines = literalLines
                                    LineBreak = lineBreakOf sourceText
                                    Names = names
                                }

                            let needingType =
                                FunctionExtraction.positionsNeedingType target.Expr captures.MethodNameEnds

                            let setting = document.Project.FSharpExtractFunctionParameterAnnotations
                            let inExposedInline = FunctionExtraction.isInExposedInline target.Path

                            let annotate (parameter: FunctionExtraction.Parameter) =
                                match setting with
                                | ParameterAnnotationSetting.Always -> true
                                | ParameterAnnotationSetting.WhenNeeded ->
                                    parameter.Uses |> List.exists (fun m -> needingType.Contains m.Start)
                                | ParameterAnnotationSetting.Never -> false

                            let functionName = uniqueName "extractedFunction" names

                            let header =
                                $"{functionName} {FunctionExtraction.curriedParameters parameters annotate}"

                            let application = $"{functionName} {FunctionExtraction.curriedArguments parameters}"

                            let call =
                                if FunctionExtraction.needsParentheses target then
                                    $"({application})"
                                else
                                    application

                            if not (captures.UsesBase || captures.UsesStructThis) then
                                match anchorsOf target.Expr target.Path with
                                | anchor :: _ ->
                                    match tryDeclareInFront extraction anchor $"let {header}" call with
                                    | ValueSome changes -> register context sourceText (SR.ExtractToLocalFunction()) "local" changes
                                    | ValueNone -> ()
                                | [] -> ()

                            if
                                not (
                                    captures.UsesThis
                                    || captures.UsesBase
                                    || inExposedInline
                                    || not captures.TypeParameters.IsEmpty
                                    || FunctionExtraction.isInRecursiveModuleLet target.Path
                                )
                            then
                                match tryDeclareInFrontOfModuleLet extraction [] $"let private {header}" call with
                                | ValueSome changes -> register context sourceText (SR.ExtractToModuleFunction()) "module" changes
                                | ValueNone -> ()

                            match FunctionExtraction.tryEnclosingMember target.Path with
                            | ValueSome(struct (binding, memberRange, info)) when
                                not inExposedInline
                                && captures.TypeParameters
                                   |> List.forall (fun declaration -> not (Range.rangeContainsRange memberRange declaration))
                                ->
                                match FunctionExtraction.tryMemberCallee binding info with
                                | ValueSome(struct (prefix, receiver)) ->
                                    let memberName = uniqueName "ExtractedMethod" names

                                    let memberHeader =
                                        $"{prefix}{memberName}{FunctionExtraction.tupledParameters parameters annotate}"

                                    let memberCall =
                                        $"{receiver}.{memberName}{FunctionExtraction.tupledArguments parameters}"

                                    match FunctionExtraction.tryMemberChanges extraction memberRange memberHeader memberCall with
                                    | ValueSome changes -> register context sourceText (SR.ExtractToPrivateMember()) "member" changes
                                    | ValueNone -> ()
                                | ValueNone -> ()
                            | _ -> ()
                        | _ -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
