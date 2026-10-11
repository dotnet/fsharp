// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.StructPropagation

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open CancellableTasks
open StructConversion

[<NoComparison; NoEquality>]
type private Source =
    {
        Document: Document
        Text: SourceText
        Tree: ParsedInput
        Check: FSharpCheckFileResults
    }

/// A place whose type changes form, and so passes the change on to what flows in and out of it.
[<RequireQualifiedAccess; NoComparison; NoEquality>]
type private Slot =
    /// A value, a parameter's value or a record field.
    | Symbol of symbolUse: FSharpSymbolUse * source: Source
    /// The result of a function or a method.
    | Result of functionUse: FSharpSymbolUse * source: Source
    /// A parameter, by its curried argument and, when that argument is a tuple of parameters, its position in it.
    | Parameter of functionUse: FSharpSymbolUse * source: Source * group: int * index: int voption

let private containsRange (outer: range) (inner: range) =
    Position.posGeq inner.Start outer.Start && Position.posGeq outer.End inner.End

let private symbolUseAt (source: Source) (ident: Ident) =
    let line = source.Text.Lines[Line.toZ ident.idRange.EndLine].ToString()
    source.Check.GetSymbolUseAtLocation(ident.idRange.EndLine, ident.idRange.EndColumn, line, [ ident.idText ])

/// Whether the parameter at the position is declared a generic type parameter itself, which other calls may
/// instantiate with another form. A tuple or an anonymous record of generic parts keeps its own form in every call.
let private isGenericParameter (mfv: FSharpMemberOrFunctionOrValue) (group: int) (index: int voption) =
    if group >= mfv.CurriedParameterGroups.Count then
        false
    else
        let parameters = mfv.CurriedParameterGroups[group]

        match index with
        | ValueSome index when index < parameters.Count -> parameters[index].Type.IsGenericParameter
        | ValueSome _ -> false
        | ValueNone -> parameters.Count = 1 && parameters[0].Type.IsGenericParameter

let rec private tryPatternIdent (pat: SynPat) =
    match pat with
    | SynPat.Named(ident = SynIdent(ident, _))
    | SynPat.OptionalVal(ident, _) -> ValueSome ident
    | SynPat.Paren(pat = inner)
    | SynPat.Typed(pat = inner)
    | SynPat.Attrib(pat = inner) -> tryPatternIdent inner
    | _ -> ValueNone

let rec private stripParenPats (pat: SynPat) =
    match pat with
    | SynPat.Paren(pat = inner) -> stripParenPats inner
    | _ -> pat

/// `name = value`, the form of a named argument.
[<return: Struct>]
let private (|NameEqualsValue|_|) (expr: SynExpr) =
    match expr with
    | SynExpr.App(
        isInfix = false
        funcExpr = SynExpr.App(
            isInfix = true; funcExpr = SynExpr.LongIdent(longDotId = SynLongIdent(id = [ operator ])); argExpr = SynExpr.Ident name)
        argExpr = value) when String.Equals(operator.idText, "op_Equality", StringComparison.Ordinal) -> ValueSome(struct (name, value))
    | _ -> ValueNone

/// The name of the function applied by the expression, in parentheses or not, and how many arguments are applied
/// before it.
let rec private tryCallee (expr: SynExpr) (applied: int) =
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

/// The function reference a binding's body is, with the arguments applied to it: `let alias = f` and
/// `let partial = f x` forward to `f`.
let rec private tryForwarded (body: SynExpr) =
    match body with
    | SynExpr.Typed(expr = inner) -> tryForwarded inner
    | _ -> tryCallee body 0

/// The application of a function reference to all of its curried argument groups.
let rec private tryApplication (node: SynExpr) (path: SyntaxVisitorPath) (remaining: int) =
    if remaining = 0 then
        ValueSome(struct (node, path))
    else
        match path with
        | SyntaxNode.SynExpr(SynExpr.TypeApp(expr = inner) | SynExpr.Paren(expr = inner) as wrapper) :: rest when isSame inner node ->
            tryApplication wrapper rest remaining
        | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = funcExpr) as app) :: rest when isSame funcExpr node ->
            tryApplication app rest (remaining - 1)
        | _ -> ValueNone

/// The argument a function reference is applied to in the given curried group.
let rec private tryArgument (node: SynExpr) (path: SyntaxVisitorPath) (group: int) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.TypeApp(expr = inner) | SynExpr.Paren(expr = inner) as wrapper) :: rest when isSame inner node ->
        tryArgument wrapper rest group
    | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = funcExpr; argExpr = argument) as app) :: rest when isSame funcExpr node ->
        if group = 0 then
            ValueSome(struct (argument, SyntaxNode.SynExpr app :: rest))
        else
            tryArgument app rest (group - 1)
    | _ -> ValueNone

/// The name a function reference is bound to without being applied to all of its arguments, and how many it is
/// applied to: `let alias = f` and `let partial = f x`.
let rec private tryForwardingBinding (node: SynExpr) (path: SyntaxVisitorPath) (applied: int) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.TypeApp(expr = inner) | SynExpr.Paren(expr = inner) | SynExpr.Typed(expr = inner) as wrapper) :: rest when
        isSame inner node
        ->
        tryForwardingBinding wrapper rest applied
    | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = funcExpr) as app) :: rest when isSame funcExpr node ->
        tryForwardingBinding app rest (applied + 1)
    | SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) :: _ when isSame body node ->
        tryPatternIdent headPat |> ValueOption.map (fun name -> struct (name, applied))
    | _ -> ValueNone

/// The lambda an expression is, through parentheses and a type annotation: its parameter patterns, its body and the
/// path to them.
let rec private tryLambda (expr: SynExpr) (path: SyntaxVisitorPath) =
    match expr with
    | SynExpr.Paren(expr = inner)
    | SynExpr.Typed(expr = inner) -> tryLambda inner (SyntaxNode.SynExpr expr :: path)
    | SynExpr.Lambda(parsedData = Some(args, body)) -> ValueSome(struct (args, body, SyntaxNode.SynExpr expr :: path))
    | _ -> ValueNone

/// The name a lambda is bound to, through parentheses and a type annotation: `let f = fun x -> x`.
let rec private tryLambdaOwner (lambda: SynExpr) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) | SynExpr.Typed(expr = inner) as wrapper) :: rest when isSame inner lambda ->
        tryLambdaOwner wrapper rest
    | SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) :: _ when isSame body lambda -> tryPatternIdent headPat
    | _ -> ValueNone

/// The function and the position of the parameter a pattern in a binding's head, or among the parameters of the
/// lambda a name is bound to, declares.
let rec private tryParameterPosition (pat: SynPat) (path: SyntaxVisitorPath) =
    let positionIn (args: SynPat list) (name: Ident) =
        args
        |> List.indexed
        |> List.tryFindV (fun (_, arg) -> containsRange arg.Range pat.Range)
        |> ValueOption.map (fun (group, arg) ->
            let index =
                match stripParenPats arg with
                | SynPat.Tuple(elementPats = elements) ->
                    match
                        elements
                        |> List.tryFindIndex (fun element -> containsRange element.Range pat.Range)
                    with
                    | Some index -> ValueSome index
                    | None -> ValueNone
                | _ -> ValueNone

            struct (name, group, index))

    match path with
    | SyntaxNode.SynPat(SynPat.LongIdent(longDotId = SynLongIdent(id = ids); argPats = SynArgPats.Pats args)) :: SyntaxNode.SynBinding _ :: _ ->
        match List.tryLast ids with
        | Some name -> positionIn args name
        | None -> ValueNone
    | SyntaxNode.SynExpr(SynExpr.Lambda(parsedData = Some(args, _)) as lambda) :: rest ->
        match tryLambdaOwner lambda rest with
        | ValueSome name -> positionIn args name
        | ValueNone -> ValueNone
    | _ :: rest -> tryParameterPosition pat rest
    | [] -> ValueNone

/// The expression a pattern takes apart, through parentheses and a type annotation: the right-hand side of its
/// binding or the matched expression.
let rec private tryMatchedExpression (pat: SynPat) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynPat(SynPat.Paren _ | SynPat.Typed _ as wrapper) :: rest -> tryMatchedExpression wrapper rest
    | SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) :: _ when isSame headPat pat -> ValueSome(struct (body, path))
    | SyntaxNode.SynMatchClause(SynMatchClause(pat = clausePat)) :: SyntaxNode.SynExpr(SynExpr.Match(expr = scrutinee) as matchExpr) :: rest when
        isSame clausePat pat
        ->
        ValueSome(struct (scrutinee, SyntaxNode.SynExpr matchExpr :: rest))
    | _ -> ValueNone

/// The type annotations on a pattern, through the parentheses around it: `((a, b): int * int)`.
let rec private annotationsOn (pat: SynPat) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynPat(SynPat.Paren(pat = inner) as paren) :: rest when isSame inner pat -> annotationsOn paren rest
    | SyntaxNode.SynPat(SynPat.Typed(pat = inner; targetType = annotation) as typed) :: rest when isSame inner pat ->
        annotation :: annotationsOn typed rest
    | _ -> []

/// The function and the curried group of which the pattern is the whole argument, through parentheses and a type
/// annotation or (a struct tuple) with neither. The function can be a name bound to a lambda.
let private tryWholeArgument (pat: SynPat) (path: SyntaxVisitorPath) =
    let rec outermost (pat: SynPat) (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynPat(SynPat.Paren(pat = inner) | SynPat.Typed(pat = inner) as wrapper) :: rest when isSame inner pat ->
            outermost wrapper rest
        | _ -> struct (pat, path)

    let struct (argument, headPath) = outermost pat path

    match headPath with
    | SyntaxNode.SynPat(SynPat.LongIdent(longDotId = SynLongIdent(id = ids); argPats = SynArgPats.Pats args)) :: SyntaxNode.SynBinding _ :: _ ->
        match List.tryFindIndex (isSame argument) args, List.tryLast ids with
        | Some group, Some name -> ValueSome(struct (name, group))
        | _ -> ValueNone
    | SyntaxNode.SynExpr(SynExpr.Lambda(parsedData = Some(args, _)) as lambda) :: rest ->
        match List.tryFindIndex (isSame argument) args, tryLambdaOwner lambda rest with
        | Some group, ValueSome name -> ValueSome(struct (name, group))
        | _ -> ValueNone
    | _ -> ValueNone

/// The expression node a symbol use stands for: an identifier, or the last part of a dotted name.
let private tryUseNode (tree: ParsedInput) (useRange: range) =
    let isUse (ident: Ident) =
        Position.posEq ident.idRange.Start useRange.Start
        && Position.posEq ident.idRange.End useRange.End

    (useRange.Start, tree)
    ||> ParsedInput.tryPickLast (fun path node ->
        match node with
        | SyntaxNode.SynExpr(SynExpr.Ident ident as expr) when isUse ident -> Some(expr, path)
        | SyntaxNode.SynExpr(SynExpr.LongIdent(longDotId = SynLongIdent(id = ids)) as expr)
        | SyntaxNode.SynExpr(SynExpr.DotGet(longDotId = SynLongIdent(id = ids)) as expr) when
            // A dotted use can cover the whole name, `r.Field`, not only its last part.
            List.tryLast ids
            |> Option.exists (fun ident -> Position.posEq ident.idRange.End useRange.End)
            ->
            Some(expr, path)
        | _ -> None)

/// The value given to a record field whose name is at the use, in a record construction or copy-and-update.
let private tryRecordFieldValue (tree: ParsedInput) (useRange: range) =
    (useRange.Start, tree)
    ||> ParsedInput.tryPickLast (fun path node ->
        match node with
        | SyntaxNode.SynExpr(SynExpr.Record(recordFields = fields) as record) ->
            fields
            |> List.tryPick (function
                | SynExprRecordFieldOrSpread.Field(field = SynExprRecordField(fieldName = (SynLongIdent(id = ids), _); expr = Some value)) when
                    List.tryLast ids
                    |> Option.exists (fun ident -> Position.posEq ident.idRange.Start useRange.Start)
                    ->
                    Some(value, SyntaxNode.SynExpr record :: path)
                | _ -> None)
        | _ -> None)

/// The body of the binding that gives the name declared at the range its value, `let name = body`, and the path to it.
let private tryNamedBinding (tree: ParsedInput) (declaration: range) =
    (ValueNone, tree)
    ||> ParsedInput.fold (fun found path node ->
        match found, node with
        | ValueNone, SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) when
            tryPatternIdent headPat
            |> ValueOption.exists (fun ident -> Position.posEq ident.idRange.Start declaration.Start)
            ->
            ValueSome(struct (body, node :: path))
        | _ -> found)

/// The pattern declaring a parameter of the function declared at the range, a function or a name bound to a lambda:
/// its whole curried argument, or one element of an argument that is a tuple of parameters.
let private tryParameterPattern (tree: ParsedInput) (declaration: range) (group: int) (index: int voption) =
    let pathTo (pat: SynPat) (parentPath: SyntaxVisitorPath) =
        match pat with
        | SynPat.Paren _ -> SyntaxNode.SynPat pat :: parentPath
        | _ -> parentPath

    let parameterIn (args: SynPat list) (argsPath: SyntaxVisitorPath) =
        if group >= args.Length then
            ValueNone
        else
            let argument = List.item group args
            let argumentPath = pathTo argument argsPath

            match stripParenPats argument, index with
            | SynPat.Tuple(elementPats = elements) as tuple, ValueSome index when index < elements.Length ->
                let element = List.item index elements
                ValueSome(struct (stripParenPats element, pathTo element (SyntaxNode.SynPat tuple :: argumentPath)))
            | _, ValueSome _ -> ValueNone
            | parameter, ValueNone -> ValueSome(struct (parameter, argumentPath))

    (ValueNone, tree)
    ||> ParsedInput.fold (fun found path node ->
        match found, node with
        | ValueNone,
          SyntaxNode.SynBinding(SynBinding(
              headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = ids); argPats = SynArgPats.Pats args) as headPat)) when
            List.tryLast ids
            |> Option.exists (fun ident -> Position.posEq ident.idRange.Start declaration.Start)
            ->
            parameterIn args (SyntaxNode.SynPat headPat :: node :: path)
        | _ -> found)
    |> ValueOption.orElseWith (fun () ->
        tryNamedBinding tree declaration
        |> ValueOption.bind (fun struct (body, bodyPath) -> tryLambda body bodyPath)
        |> ValueOption.bind (fun struct (args, _, argsPath) -> parameterIn args argsPath))

let private hasName (name: string) (parameter: FSharpParameter) =
    match parameter.Name with
    | Some parameterName -> String.Equals(parameterName, name, StringComparison.Ordinal)
    | None -> false

/// The name an argument can give for the parameter of a method at the position.
let private tryParameterName (symbol: FSharpSymbol) (group: int) (index: int voption) =
    match symbol with
    | :? FSharpMemberOrFunctionOrValue as mfv when mfv.IsMember && group < mfv.CurriedParameterGroups.Count ->
        let parameters = mfv.CurriedParameterGroups[group]

        match index with
        | ValueSome index when index < parameters.Count -> parameters[index].Name
        | ValueNone when parameters.Count = 1 -> parameters[0].Name
        | _ -> None
    | _ -> None

/// The value an argument gives a parameter: the part of an argument list given for it by name, else the one at its
/// position, or the whole argument when the parameter is not one of a tuple of parameters.
let private tryArgumentValue (parameterName: string option) (index: int voption) (argument: SynExpr) (argumentPath: SyntaxVisitorPath) =
    let struct (elements, elementsPath) =
        match argument with
        | SynExpr.Paren(expr = SynExpr.Tuple(exprs = exprs) as tuple) ->
            struct (exprs, SyntaxNode.SynExpr tuple :: SyntaxNode.SynExpr argument :: argumentPath)
        | SynExpr.Paren(expr = inner) -> struct ([ inner ], SyntaxNode.SynExpr argument :: argumentPath)
        | _ -> struct ([], argumentPath)

    let named =
        match parameterName with
        | Some parameterName ->
            elements
            |> Seq.tryPickV (fun element ->
                match element with
                | NameEqualsValue(struct (name, value)) when String.Equals(name.idText, parameterName, StringComparison.Ordinal) ->
                    ValueSome(struct (value, SyntaxNode.SynExpr element :: elementsPath))
                | _ -> ValueNone)
        | None -> ValueNone

    match named, index, argument with
    | ValueSome _, _, _ -> named
    | ValueNone, ValueSome index, SynExpr.Paren(expr = SynExpr.Tuple(exprs = exprs)) when index < exprs.Length ->
        ValueSome(struct (List.item index exprs, elementsPath))
    | ValueNone, ValueSome _, _ -> ValueNone
    | ValueNone, ValueNone, _ -> ValueSome(struct (argument, argumentPath))

/// How many curried arguments a function takes: its parameter groups or, for a function value, the arrows of its type.
let private groupCount (mfv: FSharpMemberOrFunctionOrValue) =
    let rec arrows (ty: FSharpType) =
        if ty.IsFunctionType then
            1 + arrows ty.GenericArguments[1]
        else
            0

    if mfv.CurriedParameterGroups.Count > 0 then
        mfv.CurriedParameterGroups.Count
    else
        match mfv.FullTypeSafe with
        | Some ty -> max 1 (arrows ty)
        | None -> 1

type private Engine(solution: Solution, kind: StructKind, toStruct: bool, userOpName: string) =
    let sources = Dictionary<DocumentId, Source>()
    let changes = Dictionary<DocumentId, ResizeArray<TextChange>>()
    let visited = HashSet<string>(StringComparer.Ordinal)
    let pending = Queue<Slot>()
    let mutable failed = false

    let annotationChanges (sourceText: SourceText) (annotation: SynType) =
        match stripParenTypes annotation with
        | ty when kind.IsType ty -> kind.TypeChanges sourceText toStruct true ty
        | _ -> []

    let isDeclaration (useRange: range) (declaration: range) =
        String.Equals(useRange.FileName, declaration.FileName, StringComparison.OrdinalIgnoreCase)
        && Position.posEq useRange.Start declaration.Start

    let tryDeclarationDocument (symbol: FSharpSymbol) =
        match symbol.DeclarationLocation with
        | Some declaration ->
            solution.TryGetDocumentFromPath declaration.FileName
            |> ValueOption.map (fun document -> struct (declaration, document))
        | None -> ValueNone

    let keyOf (slotKind: string) (symbol: FSharpSymbol) =
        symbol.DeclarationLocation
        |> Option.map (fun m -> $"{slotKind}|{m.FileName}|{m.StartLine}|{m.StartColumn}")

    member _.Load(document: Document) =
        cancellableTask {
            match sources.TryGetValue document.Id with
            | true, source -> return source
            | _ ->
                let! cancellationToken = CancellableTask.getCancellationToken ()
                let! text = document.GetTextAsync cancellationToken
                let! parseResults, checkResults = document.GetFSharpParseAndCheckResultsAsync userOpName

                let source =
                    {
                        Document = document
                        Text = text
                        Tree = parseResults.ParseTree
                        Check = checkResults
                    }

                sources[document.Id] <- source
                return source
        }

    member _.Add (source: Source) (newChanges: TextChange list) =
        let documentChanges =
            match changes.TryGetValue source.Document.Id with
            | true, documentChanges -> documentChanges
            | _ ->
                let documentChanges = ResizeArray()
                changes[source.Document.Id] <- documentChanges
                documentChanges

        for change in newChanges do
            let isKnown =
                documentChanges
                |> Seq.exists (fun known ->
                    known.Span = change.Span
                    && String.Equals(known.NewText, change.NewText, StringComparison.Ordinal))

            if not isKnown then
                documentChanges.Add change

    member this.AddOrFail (source: Source) (result: TextChange list voption) =
        match result with
        | ValueSome newChanges -> this.Add source newChanges
        | ValueNone -> failed <- true

    member _.Enqueue (key: string option) (slot: Slot) =
        match key with
        | Some key when visited.Add key -> pending.Enqueue slot
        | _ -> ()

    member this.EnqueueSymbol (source: Source) (ident: Ident) =
        match symbolUseAt source ident with
        | Some symbolUse when (tryDeclarationDocument symbolUse.Symbol).IsSome ->
            let isValue =
                match symbolUse.Symbol with
                | :? FSharpField as field -> not field.FieldType.IsGenericParameter
                | :? FSharpMemberOrFunctionOrValue as mfv ->
                    not mfv.IsFunction
                    && not mfv.IsMember
                    && not (mfv.FullTypeSafe |> Option.exists _.IsGenericParameter)
                | _ -> false

            if isValue then
                this.Enqueue (keyOf "S" symbolUse.Symbol) (Slot.Symbol(symbolUse, source))
        | _ -> ()

    member this.EnqueueResult (source: Source) (name: Ident) =
        match symbolUseAt source name with
        | Some functionUse when (tryDeclarationDocument functionUse.Symbol).IsSome ->
            match functionUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv when
                (mfv.IsFunction || mfv.IsMember)
                && not mfv.ReturnParameter.Type.IsGenericParameter
                ->
                this.Enqueue (keyOf "R" functionUse.Symbol) (Slot.Result(functionUse, source))
            | _ -> ()
        | _ -> ()

    member this.EnqueueParameter (source: Source) (functionName: Ident) (group: int) (index: int voption) =
        match symbolUseAt source functionName with
        | Some functionUse when (tryDeclarationDocument functionUse.Symbol).IsSome ->
            match functionUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv when (mfv.IsFunction || mfv.IsMember) && not (isGenericParameter mfv group index) ->
                let position =
                    match index with
                    | ValueSome index -> $"{group}|{index}"
                    | ValueNone -> $"{group}"

                this.Enqueue
                    (keyOf "P" functionUse.Symbol |> Option.map (fun key -> $"{key}|{position}"))
                    (Slot.Parameter(functionUse, source, group, index))
            | _ -> ()
        | _ -> ()

    /// Enqueues the parameter of a method that an argument names: `M(name = value)`.
    member this.EnqueueNamedParameter (source: Source) (methodName: Ident) (group: int) (parameterName: Ident) =
        match symbolUseAt source methodName with
        | Some methodUse ->
            match methodUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv when mfv.IsMember && group < mfv.CurriedParameterGroups.Count ->
                let parameters = mfv.CurriedParameterGroups[group]

                match parameters |> Seq.tryFindIndexV (hasName parameterName.idText) with
                | ValueSome _ when parameters.Count = 1 -> this.EnqueueParameter source methodName group ValueNone
                | ValueSome index -> this.EnqueueParameter source methodName group (ValueSome index)
                | ValueNone -> ()
            | _ -> ()
        | None -> ()

    /// The function a binding's body forwards to, with the arguments the body applies to it: a function reference
    /// applied to fewer arguments than the function takes.
    member _.TryForwarded (source: Source) (body: SynExpr) =
        match tryForwarded body with
        | ValueSome(struct (callee, applied)) ->
            match symbolUseAt source callee with
            | Some calleeUse ->
                match calleeUse.Symbol with
                | :? FSharpMemberOrFunctionOrValue as mfv when (mfv.IsFunction || mfv.IsMember) && applied < groupCount mfv ->
                    ValueSome(struct (callee, applied))
                | _ -> ValueNone
            | None -> ValueNone
        | ValueNone -> ValueNone

    /// A value of the changing form flows into a pattern.
    member this.IntoPattern (source: Source) (pat: SynPat) (path: SyntaxVisitorPath) =
        match pat with
        | SynPat.Paren(pat = inner) -> this.IntoPattern source inner (SyntaxNode.SynPat pat :: path)
        | SynPat.Typed(pat = inner; targetType = annotation) ->
            this.Add source (annotationChanges source.Text annotation)
            this.IntoPattern source inner (SyntaxNode.SynPat pat :: path)
        | SynPat.Named(ident = SynIdent(ident, _))
        | SynPat.LongIdent(longDotId = SynLongIdent(id = [ ident ]); argPats = SynArgPats.Pats []) -> this.EnqueueSymbol source ident
        | _ when kind.IsPat pat path -> this.AddOrFail source (kind.PatChanges source.Text toStruct pat path)
        | _ -> ()

    /// The value of the node now has the changing form: pass that on to where it goes.
    member this.FlowOut (source: Source) (node: SynExpr) (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) as paren) :: rest when isSame inner node -> this.FlowOut source paren rest
        | SyntaxNode.SynExpr(SynExpr.Typed(expr = inner; targetType = annotation) as typed) :: rest when isSame inner node ->
            this.Add source (annotationChanges source.Text annotation)
            this.FlowOut source typed rest
        | SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) :: _ when isSame body node ->
            match headPat with
            | SynPat.LongIdent(longDotId = SynLongIdent(id = ids); argPats = SynArgPats.Pats(_ :: _)) ->
                match List.tryLast ids with
                | Some name -> this.EnqueueResult source name
                | None -> ()
            | _ -> this.IntoPattern source headPat path
        | SyntaxNode.SynExpr(SynExpr.Sequential(expr2 = last) as container) :: rest when isSame last node ->
            this.FlowOut source container rest
        | SyntaxNode.SynExpr(SynExpr.LetOrUse letOrUse as container) :: rest when isSame letOrUse.Body node ->
            this.FlowOut source container rest
        | SyntaxNode.SynExpr(SynExpr.IfThenElse(thenExpr = thenExpr; elseExpr = Some elseExpr) as container) :: rest when
            isSame thenExpr node || isSame elseExpr node
            ->
            this.Retarget source container rest
            this.FlowOut source container rest
        | SyntaxNode.SynExpr(SynExpr.TryFinally(tryExpr = body) as container) :: rest when isSame body node ->
            this.FlowOut source container rest
        | SyntaxNode.SynExpr(SynExpr.TryWith(tryExpr = body) as container) :: rest when isSame body node ->
            this.Retarget source container rest
            this.FlowOut source container rest
        | SyntaxNode.SynMatchClause(SynMatchClause(resultExpr = result)) :: SyntaxNode.SynExpr(SynExpr.Match _ | SynExpr.TryWith _ as container) :: rest when
            isSame result node
            ->
            this.Retarget source container rest
            this.FlowOut source container rest
        | SyntaxNode.SynExpr(SynExpr.Lambda(parsedData = Some(_, body)) as lambda) :: rest when isSame body node ->
            match tryLambdaOwner lambda rest with
            | ValueSome name -> this.EnqueueResult source name
            | ValueNone -> ()
        | SyntaxNode.SynExpr(NameEqualsValue(struct (parameterName, value))) :: rest when isSame value node ->
            match rest with
            | SyntaxNode.SynExpr(SynExpr.Tuple _) :: SyntaxNode.SynExpr(SynExpr.Paren _ as paren) :: SyntaxNode.SynExpr(SynExpr.App(
                isInfix = false; funcExpr = funcExpr; argExpr = argument)) :: _
            | SyntaxNode.SynExpr(SynExpr.Paren _ as paren) :: SyntaxNode.SynExpr(SynExpr.App(
                isInfix = false; funcExpr = funcExpr; argExpr = argument)) :: _ when isSame argument paren ->
                match tryCallee funcExpr 0 with
                | ValueSome(struct (methodName, group)) -> this.EnqueueNamedParameter source methodName group parameterName
                | ValueNone -> ()
            | _ -> ()
        | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = funcExpr; argExpr = argument)) :: _ when isSame argument node ->
            match tryCallee funcExpr 0 with
            | ValueSome(struct (name, group)) -> this.EnqueueParameter source name group ValueNone
            | ValueNone -> ()
        | SyntaxNode.SynExpr(SynExpr.Tuple(exprs = exprs)) :: SyntaxNode.SynExpr(SynExpr.Paren _ as paren) :: SyntaxNode.SynExpr(SynExpr.App(
            isInfix = false; funcExpr = funcExpr; argExpr = argument)) :: _ when isSame argument paren ->
            match tryCallee funcExpr 0, List.tryFindIndex (isSame node) exprs with
            | ValueSome(struct (name, group)), Some index -> this.EnqueueParameter source name group (ValueSome index)
            | _ -> ()
        | SyntaxNode.SynExpr(SynExpr.Record(recordFields = fields)) :: _ ->
            for field in fields do
                match field with
                | SynExprRecordFieldOrSpread.Field(field = SynExprRecordField(fieldName = (SynLongIdent(id = ids), _); expr = Some value)) when
                    isSame value node
                    ->
                    match List.tryLast ids with
                    | Some name -> this.EnqueueSymbol source name
                    | None -> ()
                | _ -> ()
        | SyntaxNode.SynExpr(SynExpr.Match(expr = scrutinee; clauses = clauses) as matchExpr) :: rest when isSame scrutinee node ->
            for SynMatchClause(pat = pat) as clause in clauses do
                this.IntoPattern source pat (SyntaxNode.SynMatchClause clause :: SyntaxNode.SynExpr matchExpr :: rest)
        | _ -> ()

    /// The expression must now produce the changing form: change what it is built from.
    member this.Retarget (source: Source) (expr: SynExpr) (path: SyntaxVisitorPath) =
        let childPath = SyntaxNode.SynExpr expr :: path

        match expr with
        | SynExpr.Paren(expr = inner) -> this.Retarget source inner childPath
        | SynExpr.Typed(expr = inner; targetType = annotation) ->
            this.Add source (annotationChanges source.Text annotation)
            this.Retarget source inner childPath
        | _ when kind.IsExpr (callsMethod source.Text source.Check) expr path ->
            this.AddOrFail source (kind.ExprChanges source.Text toStruct expr path)
        | SynExpr.Ident ident -> this.EnqueueSymbol source ident
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = ids))
        | SynExpr.DotGet(longDotId = SynLongIdent(id = ids)) ->
            match List.tryLast ids with
            | Some ident -> this.EnqueueSymbol source ident
            | None -> ()
        | SynExpr.App(isInfix = false) ->
            match tryCallee expr 0 with
            | ValueSome(struct (name, _)) -> this.EnqueueResult source name
            | ValueNone -> ()
        | SynExpr.IfThenElse(thenExpr = thenExpr; elseExpr = Some elseExpr) ->
            this.Retarget source thenExpr childPath
            this.Retarget source elseExpr childPath
        | SynExpr.Match(clauses = clauses) ->
            for SynMatchClause(resultExpr = result) as clause in clauses do
                this.Retarget source result (SyntaxNode.SynMatchClause clause :: childPath)
        | SynExpr.TryFinally(tryExpr = body) -> this.Retarget source body childPath
        | SynExpr.TryWith(tryExpr = body; withCases = handlers) ->
            this.Retarget source body childPath

            for SynMatchClause(resultExpr = result) as handler in handlers do
                this.Retarget source result (SyntaxNode.SynMatchClause handler :: childPath)
        | SynExpr.Sequential(expr2 = last) -> this.Retarget source last childPath
        | SynExpr.LetOrUse letOrUse -> this.Retarget source letOrUse.Body childPath
        | _ -> ()

    /// Changes the declaration of a value, a parameter or a record field.
    member this.Define (source: Source) (declaration: range) =
        let isDeclared (ident: Ident) =
            Position.posEq ident.idRange.Start declaration.Start

        ((), source.Tree)
        ||> ParsedInput.fold (fun () path node ->
            match node with
            | SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) when
                tryPatternIdent headPat |> ValueOption.exists isDeclared
                ->
                this.Retarget source body (node :: path)
            | SyntaxNode.SynExpr(SynExpr.Match(expr = scrutinee; clauses = clauses)) when
                clauses
                |> List.exists (fun (SynMatchClause(pat = pat)) -> tryPatternIdent pat |> ValueOption.exists isDeclared)
                ->
                this.Retarget source scrutinee (node :: path)
            | SyntaxNode.SynPat(SynPat.Typed(pat = inner; targetType = annotation)) when
                tryPatternIdent inner |> ValueOption.exists isDeclared
                ->
                this.Add source (annotationChanges source.Text annotation)
            | SyntaxNode.SynTypeDefn(SynTypeDefn(
                typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Record(recordFieldsAndSpreads = fields), _))) ->
                for field in fields do
                    match field with
                    | SynFieldOrSpread.Field(SynField(idOpt = Some ident; fieldType = annotation)) when isDeclared ident ->
                        this.Add source (annotationChanges source.Text annotation)
                    | _ -> ()
            | _ -> ())

    /// Changes the declared or inferred result of a function, of a name bound to a lambda, or of the function a name
    /// forwards to.
    member this.DefineResult (source: Source) (declaration: range) =
        ((), source.Tree)
        ||> ParsedInput.fold (fun () path node ->
            match node with
            | SyntaxNode.SynBinding(SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = ids)); expr = body)) when
                List.tryLast ids
                |> Option.exists (fun ident -> Position.posEq ident.idRange.Start declaration.Start)
                ->
                this.Retarget source body (node :: path)
            | _ -> ())

        match tryNamedBinding source.Tree declaration with
        | ValueSome(struct (body, bodyPath)) ->
            match tryLambda body bodyPath with
            | ValueSome(struct (_, result, resultPath)) -> this.Retarget source result resultPath
            | ValueNone ->
                match this.TryForwarded source body with
                | ValueSome(struct (callee, _)) -> this.EnqueueResult source callee
                | ValueNone -> ()
        | ValueNone -> ()

    member this.ProcessSymbol (symbolUse: FSharpSymbolUse) (source: Source) =
        cancellableTask {
            match tryDeclarationDocument symbolUse.Symbol with
            | ValueSome(struct (declaration, document)) ->
                let! definition = this.Load document
                this.Define definition declaration
                let! uses = SymbolHelpers.getSymbolUses symbolUse source.Document source.Check

                for useDocument, useRange in uses do
                    if not (isDeclaration useRange declaration) then
                        let! useSource = this.Load useDocument

                        match tryRecordFieldValue useSource.Tree useRange with
                        | Some(value, path) -> this.Retarget useSource value path
                        | None ->
                            match tryUseNode useSource.Tree useRange with
                            | Some(node, path) -> this.FlowOut useSource node path
                            | None -> ()
            | ValueNone -> ()
        }

    member this.ProcessResult (functionUse: FSharpSymbolUse) (source: Source) =
        cancellableTask {
            match tryDeclarationDocument functionUse.Symbol, functionUse.Symbol with
            | ValueSome(struct (declaration, document)), (:? FSharpMemberOrFunctionOrValue as mfv) ->
                let! definition = this.Load document
                this.DefineResult definition declaration
                let groups = groupCount mfv
                let! uses = SymbolHelpers.getSymbolUses functionUse source.Document source.Check

                for useDocument, useRange in uses do
                    if not (isDeclaration useRange declaration) then
                        let! useSource = this.Load useDocument

                        match tryUseNode useSource.Tree useRange with
                        | Some(node, path) ->
                            match tryApplication node path groups with
                            | ValueSome(struct (application, rest)) -> this.FlowOut useSource application rest
                            | ValueNone ->
                                match tryForwardingBinding node path 0 with
                                | ValueSome(struct (name, applied)) when applied < groups -> this.EnqueueResult useSource name
                                | _ -> ()
                        | None -> ()
            | _ -> ()
        }

    member this.ProcessParameter (functionUse: FSharpSymbolUse) (source: Source) (group: int) (index: int voption) =
        cancellableTask {
            match tryDeclarationDocument functionUse.Symbol with
            | ValueSome(struct (declaration, document)) ->
                let! definition = this.Load document

                let isFound =
                    match tryParameterPattern definition.Tree declaration group index with
                    | ValueSome(struct (SynPat.Tuple _ as parameter, parameterPath)) when not (kind.IsPat parameter parameterPath) -> false
                    | ValueSome(struct (parameter, parameterPath)) ->
                        match parameter with
                        | SynPat.Typed _ -> this.IntoPattern definition parameter parameterPath
                        | _ when kind.IsPat parameter parameterPath ->
                            this.AddOrFail definition (kind.PatChanges definition.Text toStruct parameter parameterPath)
                        | _ -> ()

                        match tryPatternIdent parameter with
                        | ValueSome ident -> this.EnqueueSymbol definition ident
                        | ValueNone -> ()

                        true
                    | ValueNone ->
                        match tryNamedBinding definition.Tree declaration with
                        | ValueSome(struct (body, _)) ->
                            match this.TryForwarded definition body with
                            | ValueSome(struct (callee, applied)) ->
                                this.EnqueueParameter definition callee (group + applied) index
                                true
                            | ValueNone -> false
                        | ValueNone -> false

                if isFound then
                    let parameterName = tryParameterName functionUse.Symbol group index
                    let! uses = SymbolHelpers.getSymbolUses functionUse source.Document source.Check

                    for useDocument, useRange in uses do
                        if not (isDeclaration useRange declaration) then
                            let! useSource = this.Load useDocument

                            match tryUseNode useSource.Tree useRange with
                            | Some(node, path) ->
                                match tryArgument node path group with
                                | ValueSome(struct (argument, argumentPath)) ->
                                    match tryArgumentValue parameterName index argument argumentPath with
                                    | ValueSome(struct (value, valuePath)) -> this.Retarget useSource value valuePath
                                    | ValueNone -> ()
                                | ValueNone ->
                                    match tryForwardingBinding node path 0 with
                                    | ValueSome(struct (name, applied)) when applied <= group ->
                                        this.EnqueueParameter useSource name (group - applied) index
                                    | _ -> ()
                            | None -> ()
            | ValueNone -> ()
        }

    member this.Seed (source: Source) (caretNode: CaretNode) =
        match caretNode with
        | CaretNode.Expr(node, path) ->
            this.AddOrFail source (kind.ExprChanges source.Text toStruct node path)
            this.FlowOut source node path
        | CaretNode.Pat(node, path) ->
            this.AddOrFail source (kind.PatChanges source.Text toStruct node path)

            for annotation in annotationsOn node path do
                this.Add source (annotationChanges source.Text annotation)

            match tryMatchedExpression node path, tryWholeArgument node path with
            | ValueSome(struct (matched, matchedPath)), _ -> this.Retarget source matched matchedPath
            | ValueNone, ValueSome(struct (name, group)) -> this.EnqueueParameter source name group ValueNone
            | ValueNone, ValueNone -> ()
        | CaretNode.Type(node, annotated, isWholeAnnotation) ->
            this.Add source (kind.TypeChanges source.Text toStruct isWholeAnnotation node)

            if isWholeAnnotation then
                match annotated with
                | Annotated.Pattern(SynPat.Typed(pat = inner) as pat, path) ->
                    match tryPatternIdent inner with
                    | ValueSome ident -> this.EnqueueSymbol source ident
                    | ValueNone ->
                        this.IntoPattern source inner (SyntaxNode.SynPat pat :: path)

                        match tryMatchedExpression pat path with
                        | ValueSome(struct (matched, matchedPath)) -> this.Retarget source matched matchedPath
                        | ValueNone -> ()

                    match tryParameterPosition pat path with
                    | ValueSome(struct (name, group, index)) -> this.EnqueueParameter source name group index
                    | ValueNone -> ()
                | Annotated.Pattern _ -> ()
                | Annotated.Expression(expr, path) ->
                    this.Retarget source expr path
                    this.FlowOut source expr path
                | Annotated.Return(SynBinding(
                    headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = ids); argPats = SynArgPats.Pats(_ :: _)))) ->
                    match List.tryLast ids with
                    | Some name -> this.EnqueueResult source name
                    | None -> ()
                | Annotated.Return(SynBinding(headPat = headPat)) ->
                    match tryPatternIdent headPat with
                    | ValueSome ident -> this.EnqueueSymbol source ident
                    | ValueNone -> ()
                | Annotated.Field(SynField(idOpt = Some ident)) -> this.EnqueueSymbol source ident
                | Annotated.Field _ -> ()

    /// The solution with every change of the chain, or ValueNone when a part cannot change or changes collide.
    member this.Run(document: Document, caretNode: CaretNode) =
        cancellableTask {
            let! source = this.Load document
            this.Seed source caretNode

            while pending.Count > 0 && not failed do
                match pending.Dequeue() with
                | Slot.Symbol(symbolUse, slotSource) -> do! this.ProcessSymbol symbolUse slotSource
                | Slot.Result(functionUse, slotSource) -> do! this.ProcessResult functionUse slotSource
                | Slot.Parameter(functionUse, slotSource, group, index) -> do! this.ProcessParameter functionUse slotSource group index

            let mutable result = ValueSome solution

            for KeyValue(documentId, documentChanges) in changes do
                let ordered = documentChanges |> Seq.sortBy _.Span.Start |> Seq.toArray

                let collides =
                    ordered
                    |> Array.pairwise
                    |> Array.exists (fun (first, second) -> first.Span.End > second.Span.Start)

                match result with
                | ValueSome current when not collides && not failed ->
                    result <- ValueSome(current.WithDocumentText(documentId, sources[documentId].Text.WithChanges ordered))
                | _ -> result <- ValueNone

            return if failed then ValueNone else result
        }

let private hasSignatureFile (document: Document) =
    let signaturePath = document.FilePath + "i"

    document.Project.Documents
    |> Seq.exists (fun d -> String.Equals(d.FilePath, signaturePath, StringComparison.OrdinalIgnoreCase))

let private isInQuotation (caretNode: CaretNode) =
    let path =
        match caretNode with
        | CaretNode.Expr(path = path)
        | CaretNode.Pat(path = path)
        | CaretNode.Type(annotated = Annotated.Pattern(path = path))
        | CaretNode.Type(annotated = Annotated.Expression(path = path)) -> path
        | CaretNode.Type _ -> []

    path
    |> List.exists (function
        | SyntaxNode.SynExpr(SynExpr.Quote _) -> true
        | _ -> false)

/// Offers to change the node of the kind under the caret to its other form, together with everything its value flows
/// through.
let registerConversion
    (context: CodeRefactoringContext)
    (kind: StructKind)
    (toStructTitle: unit -> string)
    (toReferenceTitle: unit -> string)
    (userOpName: string)
    =
    cancellableTask {
        let document = context.Document

        if not (document.IsFSharpSignatureFile || hasSignatureFile document) then
            let! cancellationToken = CancellableTask.getCancellationToken ()
            let! sourceText = document.GetTextAsync cancellationToken
            let! parseResults = document.GetFSharpParseResultsAsync userOpName

            let caret =
                let linePosition = sourceText.Lines.GetLinePosition context.Span.Start
                Position.mkPos (Line.fromZ linePosition.Line) linePosition.Character

            // Telling an argument list from a value takes resolving the call; skip that when no node of the kind is near.
            let! found =
                match tryCaretNode kind (fun _ -> false) caret parseResults.ParseTree with
                | ValueNone -> CancellableTask.singleton ValueNone
                | ValueSome _ ->
                    cancellableTask {
                        let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync userOpName
                        return tryCaretNode kind (callsMethod sourceText checkResults) caret parseResults.ParseTree
                    }

            match found with
            | ValueSome caretNode when not (isInQuotation caretNode) && canChange kind sourceText caretNode ->
                let isStruct = kind.IsStruct caretNode

                let title = if isStruct then toReferenceTitle () else toStructTitle ()

                let changedSolution =
                    cancellableTask {
                        let! converted = Engine(document.Project.Solution, kind, not isStruct, userOpName).Run(document, caretNode)

                        return
                            match converted with
                            | ValueSome solution -> solution
                            | ValueNone -> document.Project.Solution
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
    }
