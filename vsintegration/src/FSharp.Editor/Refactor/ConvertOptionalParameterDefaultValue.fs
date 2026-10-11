// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Features
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
        /// `[<Optional; DefaultParameterValue(c)>] x: T`, the parameter having no other attribute.
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

    /// Whether the long identifier resolves to a value or union case of the FSharp.Core entity with the compiled full name.
    let isFSharpCoreSymbol
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
            | Some symbolUse ->
                let declaringEntityName =
                    match symbolUse.Symbol with
                    | :? FSharpUnionCase as case -> case.DeclaringEntity.TryFullName
                    | :? FSharpMemberOrFunctionOrValue as value -> value.DeclaringEntity |> Option.bind _.TryFullName
                    | _ -> None

                match declaringEntityName with
                | Some name ->
                    String.Equals(name, entityFullName, StringComparison.Ordinal)
                    && String.Equals(symbolUse.Symbol.Assembly.SimpleName, "FSharp.Core", StringComparison.Ordinal)
                | None -> false

    let private operators = "Microsoft.FSharp.Core.Operators"

    /// How to write a value or union case of FSharp.Core at the position: by its name where that resolves to it,
    /// and with its namespace and its module or type where the name means something else.
    let private coreName
        (checkResults: FSharpCheckFileResults)
        (compiledContainer: string)
        (container: string)
        (name: string)
        (position: pos)
        =
        let isNamed (symbolName: string) =
            String.Equals(symbolName, name, StringComparison.Ordinal)

        let isInScope (symbol: FSharpSymbol) =
            checkResults.IsRelativeNameResolvableFromSymbol(position, [], symbol)

        let entity =
            checkResults.ProjectContext.GetReferencedAssemblies()
            |> List.tryPick (fun assembly ->
                if String.Equals(assembly.SimpleName, "FSharp.Core", StringComparison.Ordinal) then
                    assembly.Contents.FindEntityByPath [ "Microsoft"; "FSharp"; "Core"; compiledContainer ]
                else
                    None)

        let resolves =
            match entity with
            | Some entity when entity.IsFSharpUnion ->
                entity.UnionCases
                |> Seq.exists (fun case -> isNamed case.Name && isInScope case)
            | Some entity ->
                entity.MembersFunctionsAndValues
                |> Seq.exists (fun value -> isNamed value.DisplayName && isInScope value)
            | None -> false

        if resolves then
            name
        else
            $"Microsoft.FSharp.Core.{container}.{name}"

    [<return: Struct>]
    let private (|SingleIdent|_|) (expr: SynExpr) =
        match expr with
        | SynExpr.Ident ident
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ ident ])) -> ValueSome ident
        | _ -> ValueNone

    /// The identifiers of a name, qualified or not, and the last of them.
    [<return: Struct>]
    let private (|Name|_|) (expr: SynExpr) =
        match expr with
        | SynExpr.Ident ident -> ValueSome(struct ([ ident ], ident))
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = ids)) ->
            match List.tryLast ids with
            | Some last -> ValueSome(struct (ids, last))
            | None -> ValueNone
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

            let listed = attributes |> List.collect _.Attributes

            if not (List.exists isOptional listed && List.forall isOptionalAttribute listed) then
                ValueNone
            else
                match List.tryFind isDefaultParameterValue listed with
                | None -> ValueSome(Form.DotNet(attributes, ValueNone))
                | Some attribute ->
                    match attribute.ArgExpr with
                    | SynExpr.Paren(expr = SynExpr.Const _ as constant) -> ValueSome(Form.DotNet(attributes, ValueSome constant))
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

    /// Whether the constant can be the `DefaultParameterValue` of a parameter of the type with the full name.
    let isConstantOfType (typeFullName: string option) (constant: SynConst) =
        match typeFullName, constant with
        | Some "System.Int32", SynConst.Int32 _
        | Some "System.Int64", SynConst.Int64 _
        | Some "System.Int16", SynConst.Int16 _
        | Some "System.SByte", SynConst.SByte _
        | Some "System.Byte", SynConst.Byte _
        | Some "System.UInt16", SynConst.UInt16 _
        | Some "System.UInt32", SynConst.UInt32 _
        | Some "System.UInt64", SynConst.UInt64 _
        | Some "System.Double", SynConst.Double _
        | Some "System.Single", SynConst.Single _
        | Some "System.Boolean", SynConst.Bool _
        | Some "System.Char", SynConst.Char _
        | Some "System.String", SynConst.String _ -> true
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
    let private tryDefaultArgUse (isFSharpCore: string -> Ident list -> bool) (parseTree: ParsedInput) (useRange: range) =
        match tryUseNode parseTree useRange with
        | Some(node,
               SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = Name(func, name); argExpr = arg) as inner) :: SyntaxNode.SynExpr(SynExpr.App(
                   isInfix = false; funcExpr = funcExpr; argExpr = (SynExpr.Const _ as defaultValue)) as application) :: rest) when
            isSame arg node
            && hasText "defaultArg" name
            && isSame funcExpr inner
            && isFSharpCore operators func
            ->
            ValueSome(struct (application, defaultValue, rest))
        | _ -> ValueNone

    /// The whole line of `let x = defaultArg x c` when that line does nothing but rebind the parameter immutably.
    let private tryShadowingLine (sourceText: SourceText) (name: string) (application: SynExpr) (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynBinding(SynBinding(
            isMutable = false; headPat = SynPat.Named(ident = SynIdent(ident, _)); expr = rhs; trivia = trivia)) :: SyntaxNode.SynExpr(SynExpr.LetOrUse letOrUse) :: _ when
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

    let rec private hasOptionalValBefore (position: pos) (pat: SynPat) =
        match pat with
        | SynPat.OptionalVal(range = m) -> Position.posLt m.Start position
        | SynPat.Paren(pat = inner)
        | SynPat.Typed(pat = inner)
        | SynPat.Attrib(pat = inner) -> hasOptionalValBefore position inner
        | SynPat.Tuple(elementPats = pats)
        | SynPat.LongIdent(argPats = SynArgPats.Pats pats) -> pats |> List.exists (hasOptionalValBefore position)
        | _ -> false

    let private interopServices = [ "System"; "Runtime"; "InteropServices" ]

    let rec private tryContainerRange (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynModuleOrNamespace moduleOrNamespace :: _ -> ValueSome moduleOrNamespace.Range
        | SyntaxNode.SynModule(SynModuleDecl.NestedModule(range = m)) :: _ -> ValueSome m
        | _ :: rest -> tryContainerRange rest
        | [] -> ValueNone

    /// Whether an `open System.Runtime.InteropServices` in a module or namespace enclosing the position precedes it.
    let private hasInteropServicesOpenAt (position: pos) (parseTree: ParsedInput) =
        (false, parseTree)
        ||> ParsedInput.fold (fun found path node ->
            found
            || match node with
               | SyntaxNode.SynModule(SynModuleDecl.Open(
                   target = SynOpenDeclTarget.ModuleOrNamespace(longId = SynLongIdent(id = ids)); range = m)) when
                   Position.posLt m.End position
                   && ids.Length = interopServices.Length
                   && List.forall2 hasText interopServices ids
                   ->
                   match tryContainerRange path with
                   | ValueSome container ->
                       Position.posGeq position container.Start
                       && Position.posGeq container.End position
                   | ValueNone -> false
               | _ -> false)

    /// `?x: T` with every use being `defaultArg x c` becomes `[<Optional; DefaultParameterValue(c)>] x: T`,
    /// unless a `?` parameter before it would then precede a non-optional one (FS1212).
    /// The attributes carry their namespace where no `open` of it is in scope.
    let tryToDotNetChanges
        (isFSharpCore: string -> Ident list -> bool)
        (sourceText: SourceText)
        (parseTree: ParsedInput)
        (parameter: Parameter)
        (parameterTypeName: string option)
        (optionalValRange: range)
        (uses: range list)
        =
        let (SynBinding(headPat = headPat)) = parameter.MemberBinding

        let found =
            (ValueSome [], uses)
            ||> List.fold (fun found useRange ->
                match found, tryDefaultArgUse isFSharpCore parseTree useRange with
                | ValueSome found, ValueSome defaultArgUse -> ValueSome(defaultArgUse :: found)
                | _ -> ValueNone)

        let textOf (expr: SynExpr) =
            sourceText.ToString(spanOf sourceText expr.Range)

        let attributes =
            match found with
            | ValueSome [] -> ValueSome [ "Optional" ]
            | ValueSome(struct (_, (SynExpr.Const(constant, _) as defaultValue), _) :: others) when
                isConstantOfType parameterTypeName constant
                && others
                   |> List.forall (fun struct (_, other, _) -> String.Equals(textOf other, textOf defaultValue, StringComparison.Ordinal))
                ->
                ValueSome [ "Optional"; $"DefaultParameterValue({textOf defaultValue})" ]
            | _ -> ValueNone

        match found, attributes with
        | ValueSome found, ValueSome attributes when not (hasOptionalValBefore optionalValRange.Start headPat) ->
            let qualifier =
                if hasInteropServicesOpenAt parameter.MemberName.idRange.Start parseTree then
                    ""
                else
                    String.Join(".", interopServices) + "."

            let attributeText =
                attributes
                |> Seq.map (fun attribute -> qualifier + attribute)
                |> String.concat "; "

            let name = sourceText.ToString(spanOf sourceText parameter.Ident.idRange)

            [
                TextChange(TextSpan((spanOf sourceText optionalValRange).Start, 1), $"[<{attributeText}>] ")

                for struct (application, _, path) in found do
                    match tryShadowingLine sourceText parameter.Ident.idText application path with
                    | ValueSome line -> TextChange(line, "")
                    | ValueNone -> TextChange(spanOf sourceText application.Range, name)
            ]
            |> List.sortBy _.Span.Start
            |> ValueSome
        | _ -> ValueNone

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

    let rec private startOfWhiteSpaceBefore (sourceText: SourceText) (position: int) =
        if position > 0 && Char.IsWhiteSpace sourceText[position - 1] then
            startOfWhiteSpaceBefore sourceText (position - 1)
        else
            position

    let rec private isOptionalVal (pat: SynPat) =
        match pat with
        | SynPat.OptionalVal _ -> true
        | SynPat.Paren(pat = inner)
        | SynPat.Typed(pat = inner)
        | SynPat.Attrib(pat = inner) -> isOptionalVal inner
        | _ -> false

    let rec private hasRequiredAfter (position: pos) (pat: SynPat) =
        match pat with
        | SynPat.Paren(pat = inner) -> hasRequiredAfter position inner
        | SynPat.Tuple(elementPats = pats) ->
            pats
            |> List.exists (fun pat -> Position.posGt pat.Range.Start position && not (isOptionalVal pat))
        | SynPat.LongIdent(argPats = SynArgPats.Pats pats) -> pats |> List.exists (hasRequiredAfter position)
        | _ -> false

    /// `[<Optional; DefaultParameterValue(c)>] x: T` becomes `?x: T` with `let x = defaultArg x c` starting the body,
    /// or `[<Struct>] ?x: T` with `let x = defaultValueArg x c`, unless a non-optional parameter follows it (FS1212).
    let tryToFSharpChanges
        (checkResults: FSharpCheckFileResults)
        (sourceText: SourceText)
        (indentSize: int)
        (parameter: Parameter)
        (lists: SynAttributeList list)
        (defaultValue: SynExpr voption)
        (asStruct: bool)
        =
        let name = sourceText.ToString(spanOf sourceText parameter.Ident.idRange)

        let defaultText =
            match defaultValue with
            | ValueSome value -> sourceText.ToString(spanOf sourceText value.Range)
            | ValueNone -> "Unchecked.defaultof<_>"

        let struct (prefix, defaultFunction) =
            if asStruct then
                struct ("[<Struct>] ?", "defaultValueArg")
            else
                struct ("?", "defaultArg")

        let (SynBinding(headPat = headPat; expr = body; returnInfo = returnInfo; trivia = trivia)) =
            parameter.MemberBinding

        let body =
            match returnInfo, body with
            | Some _, SynExpr.Typed(expr = inner) -> inner
            | _ -> body

        let defaultFunction =
            coreName checkResults "Operators" "Operators" defaultFunction body.Range.Start

        let declaration = $"let {name} = {defaultFunction} {name} {defaultText}"
        let memberLine = sourceText.Lines[Line.toZ trivia.LeadingKeyword.Range.StartLine]
        let lineBreak = lineBreakOf sourceText memberLine

        let bodyChange =
            match trivia.EqualsRange with
            | Some equals when body.Range.StartLine > equals.EndLine ->
                let line = sourceText.Lines[Line.toZ body.Range.StartLine]
                ValueSome(TextChange(TextSpan(line.Start, 0), $"{String(' ', body.Range.StartColumn)}{declaration}{lineBreak}"))
            | Some _ when body.Range.StartLine = body.Range.EndLine ->
                let indent = String(' ', leadingSpaces memberLine + indentSize)
                let bodyStart = (spanOf sourceText body.Range).Start

                // A comment between `=` and the body stays on the member line.
                ValueSome(
                    TextChange(
                        TextSpan.FromBounds(startOfWhiteSpaceBefore sourceText bodyStart, bodyStart),
                        $"{lineBreak}{indent}{declaration}{lineBreak}{indent}"
                    )
                )
            | _ -> ValueNone

        match bodyChange with
        | ValueSome bodyChange when not (hasRequiredAfter parameter.Ident.idRange.End headPat) ->
            [
                for list in lists do
                    TextChange(listRemoval sourceText list, "")

                TextChange(TextSpan((spanOf sourceText parameter.Ident.idRange).Start, 0), prefix)
                bodyChange
            ]
            |> List.sortBy _.Span.Start
            |> ValueSome
        | _ -> ValueNone

    let private isOperator (name: string) (expr: SynExpr) =
        match expr with
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ operator ])) -> hasText name operator
        | _ -> false

    let rec private functionOf (expr: SynExpr) =
        match expr with
        | SynExpr.App(isInfix = false; funcExpr = funcExpr) -> functionOf funcExpr
        | _ -> expr

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

    /// `Some v` becomes `ValueSome v`, `None` becomes `ValueNone`, and any other option goes through `ValueOption.ofOption`.
    let private valueOptionChanges (checkResults: FSharpCheckFileResults) (sourceText: SourceText) (value: SynExpr) =
        let valueSpan = spanOf sourceText value.Range

        match functionOf value with
        | SingleIdent head when
            (hasText "Some" head || hasText "None" head)
            && isFSharpCoreSymbol checkResults sourceText "Microsoft.FSharp.Core.FSharpOption`1" [ head ]
            ->
            let case = if hasText "Some" head then "ValueSome" else "ValueNone"

            [
                TextChange(
                    spanOf sourceText head.idRange,
                    coreName checkResults "FSharpValueOption`1" "ValueOption" case head.idRange.Start
                )
            ]
        | _ when isAtomic value -> [ TextChange(TextSpan(valueSpan.Start, 0), "ValueOption.ofOption ") ]
        | _ ->
            [
                TextChange(TextSpan(valueSpan.Start, 0), "ValueOption.ofOption (")
                TextChange(TextSpan(valueSpan.End, 0), ")")
            ]

    /// Changes to the `?name = value` arguments of the call whose function ends at the use of the member,
    /// for the parameter becoming a `[<Struct>] ?name`.
    let callSiteChanges
        (checkResults: FSharpCheckFileResults)
        (sourceText: SourceText)
        (parseTree: ParsedInput)
        (name: string)
        (useRange: range)
        =
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
            |> List.collect (valueOptionChanges checkResults sourceText)
        | None -> []

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

    static let hasFullName (fullName: string) (entity: FSharpEntity) =
        match entity.TryFullName with
        | Some name -> String.Equals(name, fullName, StringComparison.Ordinal)
        | None -> false

    static let isAttribute (fullName: string) (attribute: FSharpAttribute) =
        hasFullName fullName attribute.AttributeType

    static let isInteropOptional =
        isAttribute "System.Runtime.InteropServices.OptionalAttribute"

    static let isInteropDefaultParameterValue =
        isAttribute "System.Runtime.InteropServices.DefaultParameterValueAttribute"

    static let isByRefLike (definition: FSharpEntity) =
        definition.IsByRef
        || definition.Attributes
           |> Seq.exists (isAttribute "System.Runtime.CompilerServices.IsByRefLikeAttribute")

    static let tryTypeDefinition (fsharpType: FSharpType) =
        let stripped = fsharpType.StripAbbreviations()

        if stripped.HasTypeDefinition then
            ValueSome stripped.TypeDefinition
        else
            ValueNone

    /// The full name of `T` for the symbol of a `?x: T` parameter.
    static let tryOptionalValueTypeName (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as parameter ->
            let arguments = parameter.FullType.StripAbbreviations().GenericArguments

            match arguments |> Seq.tryHeadV |> ValueOption.bind tryTypeDefinition with
            | ValueSome definition -> definition.TryFullName
            | ValueNone -> None
        | _ -> None

    static let tryParameterSymbol (memberSymbol: FSharpSymbol) (name: string) =
        match memberSymbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            mfv.CurriedParameterGroups
            |> Seq.concat
            |> Seq.tryFindV (fun candidate ->
                match candidate.Name with
                | Some candidateName -> String.Equals(candidateName, name, StringComparison.Ordinal)
                | None -> false)
        | _ -> ValueNone

    /// Whether `?x` can replace the attributes of the parameter: they are the `OptionalAttribute` and at most the
    /// `DefaultParameterValueAttribute` of `System.Runtime.InteropServices`, the default value is one the compiler
    /// passes for a parameter of this type, and an option of the type can exist.
    static let isDotNetOptional (parameter: FSharpParameter) (defaultValue: SynExpr voption) =
        let attributes = parameter.Attributes

        attributes |> Seq.exists isInteropOptional
        && attributes
           |> Seq.forall (fun attribute -> isInteropOptional attribute || isInteropDefaultParameterValue attribute)
        && match tryTypeDefinition parameter.Type, defaultValue with
           | ValueSome definition, _ when isByRefLike definition -> false
           // `[<Optional>]` alone passes `Missing.Value` for an omitted `obj`, not the `Unchecked.defaultof` that `?` would give.
           | ValueSome definition, ValueNone -> not (hasFullName "System.Object" definition)
           | ValueSome definition, ValueSome(SynExpr.Const(constant, _)) ->
               OptionalParameterDefaultValueConversion.isConstantOfType definition.TryFullName constant
           | ValueNone, ValueNone -> true
           | _ -> false

    /// Whether the project and the F# projects that depend on it take a value option for a `?` parameter,
    /// which a caller compiled for an earlier language version cannot pass.
    static let supportsStructOptionalParameters (project: Project) =
        cancellableTask {
            let solution = project.Solution

            let projects =
                [
                    project

                    for id in solution.GetProjectDependencyGraph().GetProjectsThatTransitivelyDependOnThisProject project.Id do
                        let dependent = solution.GetProject id

                        if dependent.IsFSharp then
                            dependent
                ]

            let mutable isSupported = true

            for candidate in projects do
                if isSupported then
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
                                        (OptionalParameterDefaultValueConversion.isFSharpCoreSymbol checkResults sourceText)
                                        sourceText
                                        parseResults.ParseTree
                                        parameter
                                        (tryOptionalValueTypeName parameterUse.Symbol)
                                        optionalValRange
                                        uses
                                with
                                | ValueSome changes ->
                                    let title = SR.UseDotNetOptionalParameter()

                                    let changedDocument =
                                        cancellableTask { return document.WithText(sourceText.WithChanges changes) }

                                    context.RegisterRefactoring(CodeAction.Create(title, changedDocument, title))
                                | ValueNone -> ()
                            | None -> ()
                        | OptionalParameterDefaultValueConversion.Form.DotNet(lists, defaultValue) ->
                            let isConvertible =
                                match tryParameterSymbol memberUse.Symbol parameter.Ident.idText with
                                | ValueSome parameterSymbol -> isDotNetOptional parameterSymbol defaultValue
                                | ValueNone -> false

                            if isConvertible then
                                let! options = document.GetOptionsAsync cancellationToken
                                let! isStructSupported = supportsStructOptionalParameters document.Project

                                let indentSize =
                                    options.GetOption(FormattingOptions.IndentationSize, FSharpConstants.FSharpLanguageName)

                                let tryChanges (asStruct: bool) =
                                    OptionalParameterDefaultValueConversion.tryToFSharpChanges
                                        checkResults
                                        sourceText
                                        indentSize
                                        parameter
                                        lists
                                        defaultValue
                                        asStruct

                                match tryChanges false with
                                | ValueSome changes ->
                                    let title = SR.UseFSharpOptionalParameter()

                                    let changedDocument =
                                        cancellableTask { return document.WithText(sourceText.WithChanges changes) }

                                    context.RegisterRefactoring(CodeAction.Create(title, changedDocument, title))
                                | ValueNone -> ()

                                match tryChanges true with
                                | ValueSome definitionChanges when isStructSupported ->
                                    let title = SR.UseFSharpStructOptionalParameter()

                                    // `?x = Some 1` passes an option to `[<Optional>] x`; a struct `?x` takes a value option instead.
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

                                                let! useParseResults, useCheckResults =
                                                    useDocument.GetFSharpParseAndCheckResultsAsync(
                                                        nameof FSharpConvertOptionalParameterDefaultValueRefactoring
                                                    )

                                                let changes =
                                                    [
                                                        if documentId = document.Id then
                                                            yield! definitionChanges

                                                        for _, useRange in documentUses do
                                                            yield!
                                                                OptionalParameterDefaultValueConversion.callSiteChanges
                                                                    useCheckResults
                                                                    text
                                                                    useParseResults.ParseTree
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
                                | _ -> ()
                    | _ -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
