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
            Uses: range list
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
        fullType.HasTypeDefinition
        && (fullType.TypeDefinition.IsByRef
            || fullType.TypeDefinition.Attributes
               |> Seq.exists (fun attribute ->
                   String.Equals(attribute.AttributeType.CompiledName, "IsByRefLikeAttribute", StringComparison.Ordinal)))

    /// The values the selection reads from its scope, in order of first use, and whether it uses this or base;
    /// ValueNone when a captured value has no type or a byref-like one.
    let tryCaptures (checkResults: FSharpCheckFileResults) (selection: range) (scope: range) (cancellationToken: CancellationToken) =
        let parameters = Dictionary<range, Parameter>(Range.comparer)
        let mutable usesThis = false
        let mutable usesBase = false
        let mutable capturable = true

        for symbolUse in checkResults.GetAllUsesOfAllSymbolsInFile cancellationToken do
            if
                not symbolUse.IsFromDefinition
                && Range.rangeContainsRange selection symbolUse.Range
            then
                match symbolUse.Symbol with
                | :? FSharpMemberOrFunctionOrValue as value when value.IsMemberThisValue || value.IsConstructorThisValue -> usesThis <- true
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
                        | (false, _), Some fullType when not (isByRefLike fullType) ->
                            parameters[declaration] <-
                                {
                                    Name = value.DisplayName
                                    Type = fullType.FormatWithConstraints symbolUse.DisplayContext
                                    Uses = [ symbolUse.Range ]
                                }
                        | (false, _), _ -> capturable <- false
                    | _ -> ()
                | _ -> ()

        if capturable then
            let firstUse (parameter: Parameter) =
                parameter.Uses
                |> List.map (fun m -> struct (m.StartLine, m.StartColumn))
                |> List.min

            ValueSome(struct (parameters.Values |> Seq.sortBy firstUse |> List.ofSeq, usesThis, usesBase))
        else
            ValueNone

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

    /// Positions of identifiers whose type inference cannot tell from their use alone: receivers of a member or indexer
    /// access and operands of an operator.
    let positionsNeedingType (expr: SynExpr) =
        (HashSet<pos>(), [ SyntaxNode.SynExpr expr ])
        ||> SyntaxNodes.fold (fun positions _ node ->
            match node with
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

    /// Whether a function application put where the selection was needs parentheses to stay one argument.
    let needsParentheses (target: ExtractionTarget) =
        match target.Expr, target.Path with
        | SynExpr.Paren _, _ when target.Replaced = target.Content -> false
        | _, SyntaxNode.SynExpr(SynExpr.App _ | SynExpr.DotGet _ | SynExpr.DotIndexedGet _ | SynExpr.TypeApp _) :: _ -> true
        | _ -> false

    let isInRecursiveModuleLet (path: SyntaxVisitorPath) =
        path
        |> List.exists (function
            | SyntaxNode.SynModule(SynModuleDecl.Let(isRecursive = true)) -> true
            | _ -> false)

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
    /// member, or the name of a non-generic type for a static member.
    let tryMemberCallee (binding: SynBinding) (info: SynComponentInfo) =
        match binding, info with
        | SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = [ self; _ ]))), _ when
            not (self.idText.StartsWith("_", StringComparison.Ordinal))
            ->
            ValueSome(struct ($"member private {self.idText}.", self.idText))
        | SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = [ _ ]))), SynComponentInfo(typeParams = None) ->
            ValueSome(struct ("static member private ", (List.last info.LongIdent).idText))
        | _ -> ValueNone

    /// Changes that declare `header = <selection>` right after the member and call it where the selection was.
    let tryMemberChanges
        (sourceText: SourceText)
        (target: ExtractionTarget)
        (memberRange: range)
        (header: string)
        (call: string)
        (indentSize: int)
        (literalLines: HashSet<int>)
        =
        let lines = sourceText.Lines
        let firstLine = lines[Line.toZ memberRange.StartLine]
        let lastLine = lines[Line.toZ memberRange.EndLine]
        let indent = leadingSpaces sourceText firstLine
        let lineBreak = lineBreakOf sourceText

        tryDeclaration sourceText target.Content header (indent + indentSize) literalLines lineBreak
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

            if not (context.Span.IsEmpty || document.IsFSharpSignatureFile) then
                let! cancellationToken = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync cancellationToken
                let! parseResults = document.GetFSharpParseResultsAsync(nameof FSharpExtractFunctionRefactoring)

                match tryExtractionTarget sourceText parseResults.ParseTree context.Span with
                | ValueNone -> ()
                | ValueSome target ->
                    match FunctionExtraction.enclosingScope target.Path with
                    | None -> ()
                    | Some scope ->
                        let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync(nameof FSharpExtractFunctionRefactoring)

                        let selection =
                            Range.mkRange
                                document.FilePath
                                (positionOf sourceText target.Content.Start)
                                (positionOf sourceText target.Content.End)

                        match FunctionExtraction.tryCaptures checkResults selection scope cancellationToken with
                        | ValueSome(struct (parameters, usesThis, usesBase)) when
                            not (FunctionExtraction.assignsParameter target.Expr parameters)
                            ->
                            let! options = document.GetOptionsAsync cancellationToken

                            let indentSize =
                                options.GetOption(FormattingOptions.IndentationSize, FSharpConstants.FSharpLanguageName)

                            let literalLines = linesInsideLiterals parseResults.ParseTree
                            let names = usedNames parseResults.ParseTree
                            let needingType = FunctionExtraction.positionsNeedingType target.Expr
                            let setting = document.Project.FSharpExtractFunctionParameterAnnotations

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

                            if not usesBase then
                                match anchorsOf target.Expr target.Path with
                                | anchor :: _ ->
                                    match tryDeclareInFront sourceText target anchor $"let {header}" call indentSize literalLines with
                                    | ValueSome changes -> register context sourceText (SR.ExtractToLocalFunction()) "local" changes
                                    | ValueNone -> ()
                                | [] -> ()

                            if not (usesThis || usesBase || FunctionExtraction.isInRecursiveModuleLet target.Path) then
                                match
                                    tryDeclareInFrontOfModuleLet sourceText target [] $"let private {header}" call indentSize literalLines
                                with
                                | ValueSome changes -> register context sourceText (SR.ExtractToModuleFunction()) "module" changes
                                | ValueNone -> ()

                            match FunctionExtraction.tryEnclosingMember target.Path with
                            | ValueSome(struct (binding, memberRange, info)) ->
                                match FunctionExtraction.tryMemberCallee binding info with
                                | ValueSome(struct (prefix, receiver)) ->
                                    let memberName = uniqueName "ExtractedMethod" names

                                    let memberHeader =
                                        $"{prefix}{memberName}{FunctionExtraction.tupledParameters parameters annotate}"

                                    let memberCall =
                                        $"{receiver}.{memberName}{FunctionExtraction.tupledArguments parameters}"

                                    match
                                        FunctionExtraction.tryMemberChanges
                                            sourceText
                                            target
                                            memberRange
                                            memberHeader
                                            memberCall
                                            indentSize
                                            literalLines
                                    with
                                    | ValueSome changes -> register context sourceText (SR.ExtractToPrivateMember()) "member" changes
                                    | ValueNone -> ()
                                | ValueNone -> ()
                            | ValueNone -> ()
                        | _ -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
