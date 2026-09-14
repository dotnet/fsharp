// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open CancellableTasks

[<RequireQualifiedAccess>]
module private OptionalParameterDefaultValueConversion =

    [<RequireQualifiedAccess; NoComparison; NoEquality>]
    type Form =
        /// `?x: T`
        | FSharp of optionalValRange: range
        /// `[<Optional; DefaultParameterValue(c)>] x: T`, with the attribute lists holding only those attributes.
        | DotNet of lists: SynAttributeList list * defaultValue: SynExpr voption

    [<NoComparison; NoEquality>]
    type Parameter =
        {
            Ident: Ident
            TypeName: SynType
            Form: Form
            MemberName: Ident
            MemberBinding: SynBinding
        }

    [<RequireQualifiedAccess; NoComparison; NoEquality>]
    type private Search =
        | Searching
        | Found of Parameter voption

    let private spanOf (sourceText: SourceText) (m: range) =
        RoslynHelpers.FSharpRangeToTextSpan(sourceText, m)

    let private hasText (text: string) (ident: Ident) =
        String.Equals(ident.idText, text, StringComparison.Ordinal)

    let private isSame (expr: SynExpr) (other: SynExpr) = obj.ReferenceEquals(expr, other)

    [<return: Struct>]
    let private (|SingleIdent|_|) (expr: SynExpr) =
        match expr with
        | SynExpr.Ident ident
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ ident ])) -> ValueSome ident
        | _ -> ValueNone

    let private isAttribute (names: string list) (attribute: SynAttribute) =
        attribute.Target.IsNone
        && match List.tryLast attribute.TypeName.LongIdent with
           | Some name -> names |> List.exists (fun candidate -> hasText candidate name)
           | None -> false

    let private isOptional = isAttribute [ "Optional"; "OptionalAttribute" ]

    let private isDefaultParameterValue =
        isAttribute [ "DefaultParameterValue"; "DefaultParameterValueAttribute" ]

    let private isStruct = isAttribute [ "Struct"; "StructAttribute" ]

    /// The annotated parameter the pattern declares, with the attributes around it and the range of `?x`, if any.
    let rec private tryShape (attributes: SynAttributes) (pat: SynPat) =
        match pat with
        | SynPat.Paren(pat = inner) -> tryShape attributes inner
        | SynPat.Attrib(pat = inner; attributes = more) -> tryShape [ yield! attributes; yield! more ] inner
        | SynPat.Typed(pat = SynPat.OptionalVal(ident, m); targetType = typeName) ->
            ValueSome(struct (ident, typeName, attributes, ValueSome m))
        | SynPat.Typed(pat = SynPat.Named(ident = SynIdent(ident, _); isThisVal = false); targetType = typeName) ->
            ValueSome(struct (ident, typeName, attributes, ValueNone))
        | _ -> ValueNone

    let private tryForm (attributes: SynAttributes) (optionalValRange: range voption) =
        match optionalValRange with
        | ValueSome _ when attributes |> List.exists (fun list -> List.exists isStruct list.Attributes) -> ValueNone
        | ValueSome m -> ValueSome(Form.FSharp m)
        | ValueNone ->
            let isOptionalAttribute attribute =
                isOptional attribute || isDefaultParameterValue attribute

            let lists =
                attributes
                |> List.filter (fun list -> List.exists isOptionalAttribute list.Attributes)

            let listed = lists |> List.collect _.Attributes

            if not (List.exists isOptional listed && List.forall isOptionalAttribute listed) then
                ValueNone
            else
                match List.tryFind isDefaultParameterValue listed with
                | None -> ValueSome(Form.DotNet(lists, ValueNone))
                | Some attribute ->
                    match attribute.ArgExpr with
                    | SynExpr.Paren(expr = SynExpr.Const _ as constant) -> ValueSome(Form.DotNet(lists, ValueSome constant))
                    | _ -> ValueNone

    let rec private tryMember (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynBinding(SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = ids))) as binding) :: SyntaxNode.SynMemberDefn(SynMemberDefn.Member _) :: _ ->
            match List.tryLast ids with
            | Some name -> ValueSome(struct (name, binding))
            | None -> ValueNone
        | SyntaxNode.SynPat _ :: rest -> tryMember rest
        | _ -> ValueNone

    let tryParameter (caret: pos) (parseTree: ParsedInput) =
        let search =
            (Search.Searching, parseTree)
            ||> ParsedInput.fold (fun search path node ->
                match search, node with
                | Search.Searching, SyntaxNode.SynPat pat when Position.posGeq caret pat.Range.Start && Position.posGeq pat.Range.End caret ->
                    match tryShape [] pat, tryMember path with
                    | ValueSome(struct (ident, typeName, attributes, optionalValRange)), ValueSome(struct (memberName, binding)) ->
                        tryForm attributes optionalValRange
                        |> ValueOption.map (fun form ->
                            {
                                Ident = ident
                                TypeName = typeName
                                Form = form
                                MemberName = memberName
                                MemberBinding = binding
                            })
                        |> Search.Found
                    | _ -> Search.Searching
                | _ -> search)

        match search with
        | Search.Found parameter -> parameter
        | Search.Searching -> ValueNone

    /// Whether the constant can be the `DefaultParameterValue` of a parameter of the named type.
    let private isConstantOfType (typeText: string) (constant: SynConst) =
        match typeText, constant with
        | ("int" | "int32"), SynConst.Int32 _
        | "int64", SynConst.Int64 _
        | "int16", SynConst.Int16 _
        | "sbyte", SynConst.SByte _
        | "byte", SynConst.Byte _
        | "uint16", SynConst.UInt16 _
        | "uint32", SynConst.UInt32 _
        | "uint64", SynConst.UInt64 _
        | ("float" | "double"), SynConst.Double _
        | ("float32" | "single"), SynConst.Single _
        | "bool", SynConst.Bool _
        | "char", SynConst.Char _
        | "string", SynConst.String _ -> true
        | _ -> false

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

    /// `defaultArg x c` around a use of the parameter, with its constant and what contains it.
    let private tryDefaultArgUse (parseTree: ParsedInput) (useRange: range) =
        match tryUseNode parseTree useRange with
        | Some(node,
               SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = SingleIdent func; argExpr = arg) as inner) :: SyntaxNode.SynExpr(SynExpr.App(
                   isInfix = false; funcExpr = funcExpr; argExpr = (SynExpr.Const _ as defaultValue)) as application) :: rest) when
            isSame arg node && hasText "defaultArg" func && isSame funcExpr inner
            ->
            ValueSome(struct (application, defaultValue, rest))
        | _ -> ValueNone

    /// The whole line of `let x = defaultArg x c` when that line does nothing but rebind the parameter.
    let private tryShadowingLine (sourceText: SourceText) (name: string) (application: SynExpr) (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynBinding(SynBinding(headPat = SynPat.Named(ident = SynIdent(ident, _)); expr = rhs; trivia = trivia)) :: SyntaxNode.SynExpr(SynExpr.LetOrUse letOrUse) :: _ when
            isSame rhs application
            && hasText name ident
            && not letOrUse.IsRecursive
            && not letOrUse.IsBang
            && letOrUse.Bindings.Length = 1
            ->
            let keyword = trivia.LeadingKeyword.Range
            let line = sourceText.Lines[Line.toZ keyword.StartLine]
            let lineText = line.ToString()

            if
                application.Range.EndLine = keyword.StartLine
                && String.IsNullOrWhiteSpace(lineText.Substring(0, keyword.StartColumn))
                && String.IsNullOrWhiteSpace(lineText.Substring application.Range.EndColumn)
            then
                ValueSome(TextSpan.FromBounds(line.Start, line.EndIncludingLineBreak))
            else
                ValueNone
        | _ -> ValueNone

    /// `?x: T` with every use being `defaultArg x c` becomes `[<Optional; DefaultParameterValue(c)>] x: T`.
    let tryToDotNetChanges
        (sourceText: SourceText)
        (parseTree: ParsedInput)
        (parameter: Parameter)
        (optionalValRange: range)
        (uses: range list)
        =
        let found =
            (ValueSome [], uses)
            ||> List.fold (fun found useRange ->
                match found, tryDefaultArgUse parseTree useRange with
                | ValueSome found, ValueSome defaultArgUse -> ValueSome(defaultArgUse :: found)
                | _ -> ValueNone)

        let textOf (expr: SynExpr) =
            sourceText.ToString(spanOf sourceText expr.Range)

        let typeText =
            sourceText.ToString(spanOf sourceText parameter.TypeName.Range).Trim()

        let attributeText =
            match found with
            | ValueSome [] -> ValueSome "[<Optional>] "
            | ValueSome(struct (_, (SynExpr.Const(constant, _) as defaultValue), _) :: others) when
                isConstantOfType typeText constant
                && others
                   |> List.forall (fun struct (_, other, _) -> String.Equals(textOf other, textOf defaultValue, StringComparison.Ordinal))
                ->
                ValueSome $"[<Optional; DefaultParameterValue({textOf defaultValue})>] "
            | _ -> ValueNone

        match found, attributeText with
        | ValueSome found, ValueSome attributeText ->
            [
                TextChange(TextSpan((spanOf sourceText optionalValRange).Start, 1), attributeText)

                for struct (application, _, path) in found do
                    match tryShadowingLine sourceText parameter.Ident.idText application path with
                    | ValueSome line -> TextChange(line, "")
                    | ValueNone -> TextChange(spanOf sourceText application.Range, parameter.Ident.idText)
            ]
            |> List.sortBy _.Span.Start
            |> ValueSome
        | _ -> ValueNone

    let private interopServices = [ "System"; "Runtime"; "InteropServices" ]

    let private hasInteropServicesOpen (parseTree: ParsedInput) =
        (false, parseTree)
        ||> ParsedInput.fold (fun found _ node ->
            found
            || match node with
               | SyntaxNode.SynModule(SynModuleDecl.Open(target = SynOpenDeclTarget.ModuleOrNamespace(longId = SynLongIdent(id = ids)))) ->
                   (ids |> List.map _.idText) = interopServices
               | _ -> false)

    let withInteropServicesOpen (parseTree: ParsedInput) (memberName: Ident) (text: SourceText) =
        if hasInteropServicesOpen parseTree then
            text
        else
            let insertionContext =
                FSharp.Compiler.EditorServices.ParsedInput.FindNearestPointToInsertOpenDeclaration
                    memberName.idRange.StartLine
                    parseTree
                    (List.toArray interopServices)
                    FSharp.Compiler.EditorServices.OpenStatementInsertionPoint.TopLevel

            OpenDeclarationHelper.insertOpenDeclaration text insertionContext (String.Join(".", interopServices))
            |> fst

    let private leadingSpaces (line: TextLine) =
        let text = line.ToString()
        text.Length - text.TrimStart(' ').Length

    let private lineBreakOf (sourceText: SourceText) (line: TextLine) =
        match line.EndIncludingLineBreak - line.End with
        | 0 -> Environment.NewLine
        | length -> sourceText.ToString(TextSpan(line.End, length))

    let private listRemoval (sourceText: SourceText) (list: SynAttributeList) =
        let listSpan = spanOf sourceText list.Range
        let line = sourceText.Lines.GetLineFromPosition listSpan.End
        let after = sourceText.ToString(TextSpan.FromBounds(listSpan.End, line.End))
        TextSpan(listSpan.Start, listSpan.Length + after.Length - after.TrimStart().Length)

    /// `[<Optional; DefaultParameterValue(c)>] x: T` becomes `?x: T` with `let x = defaultArg x c` starting the body.
    let tryToFSharpChanges
        (sourceText: SourceText)
        (indentSize: int)
        (parameter: Parameter)
        (lists: SynAttributeList list)
        (defaultValue: SynExpr voption)
        =
        let name = parameter.Ident.idText

        let defaultText =
            match defaultValue with
            | ValueSome value -> sourceText.ToString(spanOf sourceText value.Range)
            | ValueNone -> "Unchecked.defaultof<_>"

        let declaration = $"let {name} = defaultArg {name} {defaultText}"

        let (SynBinding(expr = body; returnInfo = returnInfo; trivia = trivia)) =
            parameter.MemberBinding

        let body =
            match returnInfo, body with
            | Some _, SynExpr.Typed(expr = inner) -> inner
            | _ -> body

        let memberLine = sourceText.Lines[Line.toZ trivia.LeadingKeyword.Range.StartLine]
        let lineBreak = lineBreakOf sourceText memberLine

        let bodyChange =
            match trivia.EqualsRange with
            | Some equals when body.Range.StartLine > equals.EndLine ->
                let line = sourceText.Lines[Line.toZ body.Range.StartLine]
                ValueSome(TextChange(TextSpan(line.Start, 0), $"{String(' ', body.Range.StartColumn)}{declaration}{lineBreak}"))
            | Some equals when body.Range.StartLine = body.Range.EndLine ->
                let indent = String(' ', leadingSpaces memberLine + indentSize)

                ValueSome(
                    TextChange(
                        TextSpan.FromBounds((spanOf sourceText equals).End, (spanOf sourceText body.Range).Start),
                        $"{lineBreak}{indent}{declaration}{lineBreak}{indent}"
                    )
                )
            | _ -> ValueNone

        bodyChange
        |> ValueOption.map (fun bodyChange ->
            [
                for list in lists do
                    TextChange(listRemoval sourceText list, "")

                TextChange(TextSpan((spanOf sourceText parameter.Ident.idRange).Start, 0), "?")
                bodyChange
            ]
            |> List.sortBy _.Span.Start)

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ConvertOptionalParameterDefaultValue"); Shared>]
type internal FSharpConvertOptionalParameterDefaultValueRefactoring [<ImportingConstructor>] () =
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
                let! parseResults = document.GetFSharpParseResultsAsync(nameof FSharpConvertOptionalParameterDefaultValueRefactoring)

                let caret =
                    let linePosition = sourceText.Lines.GetLinePosition context.Span.Start
                    Position.mkPos (Line.fromZ linePosition.Line) linePosition.Character

                match OptionalParameterDefaultValueConversion.tryParameter caret parseResults.ParseTree with
                | ValueNone -> ()
                | ValueSome parameter ->
                    let! _, checkResults =
                        document.GetFSharpParseAndCheckResultsAsync(nameof FSharpConvertOptionalParameterDefaultValueRefactoring)

                    match tryGetSymbolUse checkResults sourceText parameter.MemberName with
                    | Some memberUse when isConvertibleMember memberUse.Symbol ->
                        match parameter.Form with
                        | OptionalParameterDefaultValueConversion.Form.FSharp optionalValRange ->
                            match tryGetSymbolUse checkResults sourceText parameter.Ident with
                            | Some parameterUse ->
                                let memberRange = parameter.MemberBinding.RangeOfBindingWithRhs

                                let uses =
                                    checkResults.GetUsesOfSymbolInFile(parameterUse.Symbol, cancellationToken = cancellationToken)
                                    // Named arguments at call sites are uses of the parameter too; only the body matters here.
                                    |> Seq.filter (fun symbolUse ->
                                        not symbolUse.IsFromDefinition
                                        && Position.posGeq symbolUse.Range.Start memberRange.Start
                                        && Position.posGeq memberRange.End symbolUse.Range.End)
                                    |> Seq.map _.Range
                                    |> List.ofSeq

                                match
                                    OptionalParameterDefaultValueConversion.tryToDotNetChanges
                                        sourceText
                                        parseResults.ParseTree
                                        parameter
                                        optionalValRange
                                        uses
                                with
                                | ValueSome changes ->
                                    let title = SR.UseDotNetOptionalParameter()

                                    let changedDocument =
                                        cancellableTask {
                                            let changed =
                                                sourceText.WithChanges changes
                                                |> OptionalParameterDefaultValueConversion.withInteropServicesOpen
                                                    parseResults.ParseTree
                                                    parameter.MemberName

                                            return document.WithText changed
                                        }

                                    context.RegisterRefactoring(CodeAction.Create(title, changedDocument, title))
                                | ValueNone -> ()
                            | None -> ()
                        | OptionalParameterDefaultValueConversion.Form.DotNet(lists, defaultValue) ->
                            let! options = document.GetOptionsAsync cancellationToken

                            let indentSize =
                                options.GetOption(FormattingOptions.IndentationSize, FSharpConstants.FSharpLanguageName)

                            match
                                OptionalParameterDefaultValueConversion.tryToFSharpChanges
                                    sourceText
                                    indentSize
                                    parameter
                                    lists
                                    defaultValue
                            with
                            | ValueSome changes ->
                                let title = SR.UseFSharpOptionalParameter()

                                let changedDocument =
                                    cancellableTask { return document.WithText(sourceText.WithChanges changes) }

                                context.RegisterRefactoring(CodeAction.Create(title, changedDocument, title))
                            | ValueNone -> ()
                    | _ -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
