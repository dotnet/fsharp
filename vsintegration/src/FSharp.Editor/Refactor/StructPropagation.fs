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

[<RequireQualifiedAccess; NoComparison; NoEquality>]
type private Slot =
    | Value of symbolUse: FSharpSymbolUse * source: Source
    | Result of functionUse: FSharpSymbolUse * source: Source
    | Parameter of functionUse: FSharpSymbolUse * source: Source * group: int * index: int voption

let private symbolUseAt (source: Source) (ident: Ident) =
    symbolUseAtIdent source.Text source.Check ident

let private startsAt (position: pos) (ident: Ident) =
    Position.posEq ident.idRange.Start position

let private endsAt (position: pos) (ident: Ident) =
    Position.posEq ident.idRange.End position

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

[<return: Struct>]
let private (|NameEqualsValue|_|) (expr: SynExpr) =
    match expr with
    | SynExpr.App(
        isInfix = false
        funcExpr = SynExpr.App(
            isInfix = true; funcExpr = SynExpr.LongIdent(longDotId = SynLongIdent(id = [ operator ])); argExpr = SynExpr.Ident name)
        argExpr = value) when String.Equals(operator.idText, "op_Equality", StringComparison.Ordinal) -> ValueSome(struct (name, value))
    | _ -> ValueNone

[<return: Struct>]
let private (|FunctionHead|_|) (pat: SynPat) =
    match pat with
    | SynPat.LongIdent(longDotId = SynLongIdent(id = ids); argPats = SynArgPats.Pats(_ :: _)) ->
        match List.tryLast ids with
        | Some name -> ValueSome name
        | None -> ValueNone
    | _ -> ValueNone

let rec private tryForwardedFunction (body: SynExpr) =
    match body with
    | SynExpr.Typed(expr = inner) -> tryForwardedFunction inner
    | _ -> tryCallee body 0

let rec private tryFullApplication (node: SynExpr) (path: SyntaxVisitorPath) (remaining: int) =
    if remaining = 0 then
        ValueSome(struct (node, path))
    else
        match path with
        | SyntaxNode.SynExpr(SynExpr.TypeApp(expr = inner) | SynExpr.Paren(expr = inner) as wrapper) :: rest when isSame inner node ->
            tryFullApplication wrapper rest remaining
        | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = funcExpr) as app) :: rest when isSame funcExpr node ->
            tryFullApplication app rest (remaining - 1)
        | _ -> ValueNone

let rec private tryArgumentInGroup (node: SynExpr) (path: SyntaxVisitorPath) (group: int) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.TypeApp(expr = inner) | SynExpr.Paren(expr = inner) as wrapper) :: rest when isSame inner node ->
        tryArgumentInGroup wrapper rest group
    | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = funcExpr; argExpr = argument) as app) :: rest when isSame funcExpr node ->
        if group = 0 then
            ValueSome(struct (argument, SyntaxNode.SynExpr app :: rest))
        else
            tryArgumentInGroup app rest (group - 1)
    | _ -> ValueNone

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

let rec private tryLambda (expr: SynExpr) (path: SyntaxVisitorPath) =
    match expr with
    | SynExpr.Paren(expr = inner)
    | SynExpr.Typed(expr = inner) -> tryLambda inner (SyntaxNode.SynExpr expr :: path)
    | SynExpr.Lambda(parsedData = Some(args, body)) -> ValueSome(struct (args, body, SyntaxNode.SynExpr expr :: path))
    | _ -> ValueNone

let rec private tryLambdaOwner (lambda: SynExpr) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) | SynExpr.Typed(expr = inner) as wrapper) :: rest when isSame inner lambda ->
        tryLambdaOwner wrapper rest
    | SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) :: _ when isSame body lambda -> tryPatternIdent headPat
    | _ -> ValueNone

let rec private tryParameterPosition (pat: SynPat) (path: SyntaxVisitorPath) =
    let containsPat (outer: SynPat) =
        Range.rangeContainsRange outer.Range pat.Range

    let tupleElementIndex (arg: SynPat) =
        match stripParenPats arg with
        | SynPat.Tuple(elementPats = elements) -> elements |> Seq.tryFindIndexV containsPat
        | _ -> ValueNone

    let positionIn (args: SynPat list) (name: Ident) =
        args
        |> List.indexed
        |> List.tryFindV (fun (_, arg) -> containsPat arg)
        |> ValueOption.map (fun (group, arg) -> struct (name, group, tupleElementIndex arg))

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

let rec private tryMatchedExpression (pat: SynPat) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynPat(SynPat.Paren _ | SynPat.Typed _ as wrapper) :: rest -> tryMatchedExpression wrapper rest
    | SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) :: _ when isSame headPat pat -> ValueSome(struct (body, path))
    | SyntaxNode.SynMatchClause(SynMatchClause(pat = clausePat)) :: SyntaxNode.SynExpr(SynExpr.Match(expr = scrutinee) as matchExpr) :: rest when
        isSame clausePat pat
        ->
        ValueSome(struct (scrutinee, SyntaxNode.SynExpr matchExpr :: rest))
    | _ -> ValueNone

let rec private annotationsOn (pat: SynPat) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynPat(SynPat.Paren(pat = inner) as paren) :: rest when isSame inner pat -> annotationsOn paren rest
    | SyntaxNode.SynPat(SynPat.Typed(pat = inner; targetType = annotation) as typed) :: rest when isSame inner pat ->
        annotation :: annotationsOn typed rest
    | _ -> []

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

let private tryUseNode (tree: ParsedInput) (useRange: range) =
    let isUse (ident: Ident) =
        startsAt useRange.Start ident && endsAt useRange.End ident

    let endsWithUse (ids: Ident list) =
        List.tryLast ids |> Option.exists (endsAt useRange.End)

    (useRange.Start, tree)
    ||> ParsedInput.tryPickLast (fun path node ->
        match node with
        | SyntaxNode.SynExpr(SynExpr.Ident ident as expr) when isUse ident -> Some(expr, path)
        | SyntaxNode.SynExpr(SynExpr.LongIdent(longDotId = SynLongIdent(id = ids)) as expr)
        | SyntaxNode.SynExpr(SynExpr.DotGet(longDotId = SynLongIdent(id = ids)) as expr) when endsWithUse ids -> Some(expr, path)
        | _ -> None)

let private tryRecordFieldValue (tree: ParsedInput) (useRange: range) =
    let tryFieldValue (path: SyntaxVisitorPath) (record: SynExpr) (fields: SynExprRecordFieldOrSpread list) =
        fields
        |> List.tryPick (function
            | SynExprRecordFieldOrSpread.Field(field = SynExprRecordField(fieldName = (SynLongIdent(id = ids), _); expr = Some value)) when
                List.tryLast ids |> Option.exists (startsAt useRange.Start)
                ->
                Some(value, SyntaxNode.SynExpr record :: path)
            | _ -> None)

    (useRange.Start, tree)
    ||> ParsedInput.tryPickLast (fun path node ->
        match node with
        | SyntaxNode.SynExpr(SynExpr.Record(recordFields = fields) as record) -> tryFieldValue path record fields
        | _ -> None)

let private tryNamedBinding (tree: ParsedInput) (declaration: range) =
    (ValueNone, tree)
    ||> ParsedInput.fold (fun found path node ->
        match found, node with
        | ValueNone, SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) when
            tryPatternIdent headPat |> ValueOption.exists (startsAt declaration.Start)
            ->
            ValueSome(struct (body, node :: path))
        | _ -> found)

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
            List.tryLast ids |> Option.exists (startsAt declaration.Start)
            ->
            parameterIn args (SyntaxNode.SynPat headPat :: node :: path)
        | _ -> found)
    |> ValueOption.orElseWith (fun () ->
        tryNamedBinding tree declaration
        |> ValueOption.bind (fun struct (body, bodyPath) -> tryLambda body bodyPath)
        |> ValueOption.bind (fun struct (args, _, argsPath) -> parameterIn args argsPath))

let private hasName (name: string) (parameter: FSharpParameter) =
    parameter.Name
    |> Option.exists (fun parameterName -> String.Equals(parameterName, name, StringComparison.Ordinal))

let private tryParameterName (symbol: FSharpSymbol) (group: int) (index: int voption) =
    match symbol with
    | :? FSharpMemberOrFunctionOrValue as mfv when mfv.IsMember && group < mfv.CurriedParameterGroups.Count ->
        let parameters = mfv.CurriedParameterGroups[group]

        match index with
        | ValueSome index when index < parameters.Count -> parameters[index].Name
        | ValueNone when parameters.Count = 1 -> parameters[0].Name
        | _ -> None
    | _ -> None

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

let private isValueSymbol (symbol: FSharpSymbol) =
    match symbol with
    | :? FSharpField as field -> not field.FieldType.IsGenericParameter
    | :? FSharpMemberOrFunctionOrValue as mfv ->
        not mfv.IsFunction
        && not mfv.IsMember
        && not (mfv.FullTypeSafe |> Option.exists _.IsGenericParameter)
    | _ -> false

let private tryPartiallyAppliedFunction (source: Source) (body: SynExpr) =
    let isPartiallyApplied struct (callee: Ident, applied: int) =
        match symbolUseAt source callee |> Option.map _.Symbol with
        | Some(:? FSharpMemberOrFunctionOrValue as mfv) -> (mfv.IsFunction || mfv.IsMember) && applied < groupCount mfv
        | _ -> false

    tryForwardedFunction body |> ValueOption.filter isPartiallyApplied

type private Engine(solution: Solution, kind: StructKind, toStruct: bool, userOpName: string) =
    let sources = Dictionary<DocumentId, Source>()
    let changes = Dictionary<DocumentId, ResizeArray<TextChange>>()
    let visited = HashSet<string>(StringComparer.Ordinal)
    let pending = Queue<Slot>()
    let mutable failed = false

    let annotationChanges (sourceText: SourceText) (annotation: SynType) =
        kind.TypeChanges sourceText toStruct true (stripParenTypes annotation)

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

    let tryFunctionUse (source: Source) (name: Ident) =
        match symbolUseAt source name with
        | Some functionUse when (tryDeclarationDocument functionUse.Symbol).IsSome ->
            match functionUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv when mfv.IsFunction || mfv.IsMember -> ValueSome(struct (functionUse, mfv))
            | _ -> ValueNone
        | _ -> ValueNone

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

    member this.AddAnnotation (source: Source) (annotation: SynType) =
        this.Add source (annotationChanges source.Text annotation)

    member _.Enqueue (key: string option) (slot: Slot) =
        match key with
        | Some key when visited.Add key -> pending.Enqueue slot
        | _ -> ()

    member this.EnqueueValue (source: Source) (ident: Ident) =
        match symbolUseAt source ident with
        | Some symbolUse when
            (tryDeclarationDocument symbolUse.Symbol).IsSome
            && isValueSymbol symbolUse.Symbol
            ->
            this.Enqueue (keyOf "S" symbolUse.Symbol) (Slot.Value(symbolUse, source))
        | _ -> ()

    member this.EnqueueResult (source: Source) (name: Ident) =
        match tryFunctionUse source name with
        | ValueSome(struct (functionUse, mfv)) when not mfv.ReturnParameter.Type.IsGenericParameter ->
            this.Enqueue (keyOf "R" functionUse.Symbol) (Slot.Result(functionUse, source))
        | _ -> ()

    member this.EnqueueParameter (source: Source) (functionName: Ident) (group: int) (index: int voption) =
        match tryFunctionUse source functionName with
        | ValueSome(struct (functionUse, mfv)) when not (isGenericParameter mfv group index) ->
            let position =
                match index with
                | ValueSome index -> $"{group}|{index}"
                | ValueNone -> $"{group}"

            this.Enqueue
                (keyOf "P" functionUse.Symbol |> Option.map (fun key -> $"{key}|{position}"))
                (Slot.Parameter(functionUse, source, group, index))
        | _ -> ()

    member this.EnqueueNamedParameter (source: Source) (methodName: Ident) (group: int) (parameterName: Ident) =
        match symbolUseAt source methodName |> Option.map _.Symbol with
        | Some(:? FSharpMemberOrFunctionOrValue as mfv) when mfv.IsMember && group < mfv.CurriedParameterGroups.Count ->
            let parameters = mfv.CurriedParameterGroups[group]

            match parameters |> Seq.tryFindIndexV (hasName parameterName.idText) with
            | ValueSome _ when parameters.Count = 1 -> this.EnqueueParameter source methodName group ValueNone
            | ValueSome index -> this.EnqueueParameter source methodName group (ValueSome index)
            | ValueNone -> ()
        | _ -> ()

    member this.IntoPattern (source: Source) (pat: SynPat) (path: SyntaxVisitorPath) =
        match pat with
        | SynPat.Paren(pat = inner) -> this.IntoPattern source inner (SyntaxNode.SynPat pat :: path)
        | SynPat.Typed(pat = inner; targetType = annotation) ->
            this.AddAnnotation source annotation
            this.IntoPattern source inner (SyntaxNode.SynPat pat :: path)
        | SynPat.Named(ident = SynIdent(ident, _))
        | SynPat.LongIdent(longDotId = SynLongIdent(id = [ ident ]); argPats = SynArgPats.Pats []) -> this.EnqueueValue source ident
        | _ when kind.IsPat pat path -> this.AddOrFail source (kind.PatChanges source.Text toStruct pat path)
        | _ -> ()

    member this.FlowOut (source: Source) (node: SynExpr) (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) as paren) :: rest when isSame inner node -> this.FlowOut source paren rest
        | SyntaxNode.SynExpr(SynExpr.Typed(expr = inner; targetType = annotation) as typed) :: rest when isSame inner node ->
            this.AddAnnotation source annotation
            this.FlowOut source typed rest
        | SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) :: _ when isSame body node ->
            match headPat with
            | FunctionHead name -> this.EnqueueResult source name
            | _ -> this.IntoPattern source headPat path
        | SyntaxNode.SynExpr(SynExpr.Sequential(expr2 = last) as container) :: rest when isSame last node ->
            this.FlowOut source container rest
        | SyntaxNode.SynExpr(SynExpr.LetOrUse letOrUse as container) :: rest when isSame letOrUse.Body node ->
            this.FlowOut source container rest
        | SyntaxNode.SynExpr(SynExpr.IfThenElse(thenExpr = thenExpr; elseExpr = Some elseExpr) as container) :: rest when
            isSame thenExpr node || isSame elseExpr node
            ->
            this.FlowOutOfBranch source container rest
        | SyntaxNode.SynExpr(SynExpr.TryFinally(tryExpr = body) as container) :: rest when isSame body node ->
            this.FlowOut source container rest
        | SyntaxNode.SynExpr(SynExpr.TryWith(tryExpr = body) as container) :: rest when isSame body node ->
            this.FlowOutOfBranch source container rest
        | SyntaxNode.SynMatchClause(SynMatchClause(resultExpr = result)) :: SyntaxNode.SynExpr(SynExpr.Match _ | SynExpr.TryWith _ as container) :: rest when
            isSame result node
            ->
            this.FlowOutOfBranch source container rest
        | SyntaxNode.SynExpr(SynExpr.Lambda(parsedData = Some(_, body)) as lambda) :: rest when isSame body node ->
            tryLambdaOwner lambda rest |> ValueOption.iter (this.EnqueueResult source)
        | SyntaxNode.SynExpr(NameEqualsValue(struct (parameterName, value))) :: rest when isSame value node ->
            this.FlowOutOfNamedArgument source parameterName rest
        | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = funcExpr; argExpr = argument)) :: _ when isSame argument node ->
            tryCallee funcExpr 0
            |> ValueOption.iter (fun struct (name, group) -> this.EnqueueParameter source name group ValueNone)
        | SyntaxNode.SynExpr(SynExpr.Tuple(exprs = exprs)) :: SyntaxNode.SynExpr(SynExpr.Paren _ as paren) :: SyntaxNode.SynExpr(SynExpr.App(
            isInfix = false; funcExpr = funcExpr; argExpr = argument)) :: _ when isSame argument paren ->
            match tryCallee funcExpr 0, List.tryFindIndex (isSame node) exprs with
            | ValueSome(struct (name, group)), Some index -> this.EnqueueParameter source name group (ValueSome index)
            | _ -> ()
        | SyntaxNode.SynExpr(SynExpr.Record(recordFields = fields)) :: _ -> this.FlowOutOfRecordField source node fields
        | SyntaxNode.SynExpr(SynExpr.Match(expr = scrutinee; clauses = clauses) as matchExpr) :: rest when isSame scrutinee node ->
            for SynMatchClause(pat = pat) as clause in clauses do
                this.IntoPattern source pat (SyntaxNode.SynMatchClause clause :: SyntaxNode.SynExpr matchExpr :: rest)
        | _ -> ()

    member this.FlowOutOfBranch (source: Source) (container: SynExpr) (path: SyntaxVisitorPath) =
        this.Retarget source container path
        this.FlowOut source container path

    member this.FlowOutOfNamedArgument (source: Source) (parameterName: Ident) (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynExpr(SynExpr.Tuple _) :: SyntaxNode.SynExpr(SynExpr.Paren _ as paren) :: SyntaxNode.SynExpr(SynExpr.App(
            isInfix = false; funcExpr = funcExpr; argExpr = argument)) :: _
        | SyntaxNode.SynExpr(SynExpr.Paren _ as paren) :: SyntaxNode.SynExpr(SynExpr.App(
            isInfix = false; funcExpr = funcExpr; argExpr = argument)) :: _ when isSame argument paren ->
            tryCallee funcExpr 0
            |> ValueOption.iter (fun struct (methodName, group) -> this.EnqueueNamedParameter source methodName group parameterName)
        | _ -> ()

    member this.FlowOutOfRecordField (source: Source) (node: SynExpr) (fields: SynExprRecordFieldOrSpread list) =
        for field in fields do
            match field with
            | SynExprRecordFieldOrSpread.Field(field = SynExprRecordField(fieldName = (SynLongIdent(id = ids), _); expr = Some value)) when
                isSame value node
                ->
                List.tryLast ids |> Option.iter (this.EnqueueValue source)
            | _ -> ()

    member this.Retarget (source: Source) (expr: SynExpr) (path: SyntaxVisitorPath) =
        let childPath = SyntaxNode.SynExpr expr :: path

        match expr with
        | SynExpr.Paren(expr = inner) -> this.Retarget source inner childPath
        | SynExpr.Typed(expr = inner; targetType = annotation) ->
            this.AddAnnotation source annotation
            this.Retarget source inner childPath
        | _ when kind.IsExpr (callsMethod source.Text source.Check) expr path ->
            this.AddOrFail source (kind.ExprChanges source.Text toStruct expr path)
        | SynExpr.Ident ident -> this.EnqueueValue source ident
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = ids))
        | SynExpr.DotGet(longDotId = SynLongIdent(id = ids)) -> List.tryLast ids |> Option.iter (this.EnqueueValue source)
        | SynExpr.App(isInfix = false) ->
            tryCallee expr 0
            |> ValueOption.iter (fun struct (name, _) -> this.EnqueueResult source name)
        | SynExpr.IfThenElse(thenExpr = thenExpr; elseExpr = Some elseExpr) ->
            this.Retarget source thenExpr childPath
            this.Retarget source elseExpr childPath
        | SynExpr.Match(clauses = clauses) -> this.RetargetResults source clauses childPath
        | SynExpr.TryFinally(tryExpr = body) -> this.Retarget source body childPath
        | SynExpr.TryWith(tryExpr = body; withCases = handlers) ->
            this.Retarget source body childPath
            this.RetargetResults source handlers childPath
        | SynExpr.Sequential(expr2 = last) -> this.Retarget source last childPath
        | SynExpr.LetOrUse letOrUse -> this.Retarget source letOrUse.Body childPath
        | _ -> ()

    member this.RetargetValue (source: Source) (struct (value, path): struct (SynExpr * SyntaxVisitorPath)) =
        this.Retarget source value path

    member this.RetargetResults (source: Source) (clauses: SynMatchClause list) (path: SyntaxVisitorPath) =
        for SynMatchClause(resultExpr = result) as clause in clauses do
            this.Retarget source result (SyntaxNode.SynMatchClause clause :: path)

    member this.Define (source: Source) (declaration: range) =
        let isDeclared = startsAt declaration.Start

        ((), source.Tree)
        ||> ParsedInput.fold (fun () path node -> this.DefineAt source isDeclared path node)

    member this.DefineAt (source: Source) (isDeclared: Ident -> bool) (path: SyntaxVisitorPath) (node: SyntaxNode) =
        match node with
        | SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) when tryPatternIdent headPat |> ValueOption.exists isDeclared ->
            this.Retarget source body (node :: path)
        | SyntaxNode.SynExpr(SynExpr.Match(expr = scrutinee; clauses = clauses)) when
            clauses
            |> List.exists (fun (SynMatchClause(pat = pat)) -> tryPatternIdent pat |> ValueOption.exists isDeclared)
            ->
            this.Retarget source scrutinee (node :: path)
        | SyntaxNode.SynPat(SynPat.Typed(pat = inner; targetType = annotation)) when tryPatternIdent inner |> ValueOption.exists isDeclared ->
            this.AddAnnotation source annotation
        | SyntaxNode.SynTypeDefn(SynTypeDefn(
            typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Record(recordFieldsAndSpreads = fields), _))) ->
            for field in fields do
                match field with
                | SynFieldOrSpread.Field(SynField(idOpt = Some ident; fieldType = annotation)) when isDeclared ident ->
                    this.AddAnnotation source annotation
                | _ -> ()
        | _ -> ()

    member this.DefineResult (source: Source) (declaration: range) =
        ((), source.Tree)
        ||> ParsedInput.fold (fun () path node ->
            match node with
            | SyntaxNode.SynBinding(SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = ids)); expr = body)) when
                List.tryLast ids |> Option.exists (startsAt declaration.Start)
                ->
                this.Retarget source body (node :: path)
            | _ -> ())

        match tryNamedBinding source.Tree declaration with
        | ValueSome(struct (body, bodyPath)) -> this.DefineNamedResult source body bodyPath
        | ValueNone -> ()

    member this.DefineNamedResult (source: Source) (body: SynExpr) (bodyPath: SyntaxVisitorPath) =
        match tryLambda body bodyPath with
        | ValueSome(struct (_, result, resultPath)) -> this.Retarget source result resultPath
        | ValueNone ->
            tryPartiallyAppliedFunction source body
            |> ValueOption.iter (fun struct (callee, _) -> this.EnqueueResult source callee)

    member this.ForEachUse (symbolUse: FSharpSymbolUse) (source: Source) (declaration: range) (visit: Source -> range -> unit) =
        cancellableTask {
            let! uses = SymbolHelpers.getSymbolUses symbolUse source.Document source.Check

            for useDocument, useRange in uses do
                if not (isDeclaration useRange declaration) then
                    let! useSource = this.Load useDocument
                    visit useSource useRange
        }

    member this.ProcessValue (symbolUse: FSharpSymbolUse) (source: Source) =
        cancellableTask {
            match tryDeclarationDocument symbolUse.Symbol with
            | ValueSome(struct (declaration, document)) ->
                let! definition = this.Load document
                this.Define definition declaration
                do! this.ForEachUse symbolUse source declaration this.FlowOutOfValueUse
            | ValueNone -> ()
        }

    member this.FlowOutOfValueUse (useSource: Source) (useRange: range) =
        match tryRecordFieldValue useSource.Tree useRange with
        | Some(value, path) -> this.Retarget useSource value path
        | None ->
            tryUseNode useSource.Tree useRange
            |> Option.iter (fun (node, path) -> this.FlowOut useSource node path)

    member this.ProcessResult (functionUse: FSharpSymbolUse) (source: Source) =
        cancellableTask {
            match tryDeclarationDocument functionUse.Symbol, functionUse.Symbol with
            | ValueSome(struct (declaration, document)), (:? FSharpMemberOrFunctionOrValue as mfv) ->
                let! definition = this.Load document
                this.DefineResult definition declaration
                let groups = groupCount mfv
                do! this.ForEachUse functionUse source declaration (this.FlowOutOfResultUse groups)
            | _ -> ()
        }

    member this.FlowOutOfResultUse (groups: int) (useSource: Source) (useRange: range) =
        match tryUseNode useSource.Tree useRange with
        | Some(node, path) ->
            match tryFullApplication node path groups with
            | ValueSome(struct (application, rest)) -> this.FlowOut useSource application rest
            | ValueNone ->
                match tryForwardingBinding node path 0 with
                | ValueSome(struct (name, applied)) when applied < groups -> this.EnqueueResult useSource name
                | _ -> ()
        | None -> ()

    member this.ProcessParameter (functionUse: FSharpSymbolUse) (source: Source) (group: int) (index: int voption) =
        cancellableTask {
            match tryDeclarationDocument functionUse.Symbol with
            | ValueSome(struct (declaration, document)) ->
                let! definition = this.Load document

                if this.DefineParameter definition declaration group index then
                    let parameterName = tryParameterName functionUse.Symbol group index
                    do! this.ForEachUse functionUse source declaration (this.FlowOutOfParameterUse parameterName group index)
            | ValueNone -> ()
        }

    member this.DefineParameter (definition: Source) (declaration: range) (group: int) (index: int voption) =
        match tryParameterPattern definition.Tree declaration group index with
        | ValueSome(struct (SynPat.Tuple _ as parameter, parameterPath)) when not (kind.IsPat parameter parameterPath) -> false
        | ValueSome(struct (parameter, parameterPath)) ->
            match parameter with
            | SynPat.Typed _ -> this.IntoPattern definition parameter parameterPath
            | _ when kind.IsPat parameter parameterPath ->
                this.AddOrFail definition (kind.PatChanges definition.Text toStruct parameter parameterPath)
            | _ -> ()

            tryPatternIdent parameter |> ValueOption.iter (this.EnqueueValue definition)
            true
        | ValueNone -> this.ForwardParameter definition declaration group index

    member this.ForwardParameter (definition: Source) (declaration: range) (group: int) (index: int voption) =
        let forwarded =
            tryNamedBinding definition.Tree declaration
            |> ValueOption.bind (fun struct (body, _) -> tryPartiallyAppliedFunction definition body)

        match forwarded with
        | ValueSome(struct (callee, applied)) ->
            this.EnqueueParameter definition callee (group + applied) index
            true
        | ValueNone -> false

    member this.FlowOutOfParameterUse
        (parameterName: string option)
        (group: int)
        (index: int voption)
        (useSource: Source)
        (useRange: range)
        =
        match tryUseNode useSource.Tree useRange with
        | Some(node, path) ->
            match tryArgumentInGroup node path group with
            | ValueSome(struct (argument, argumentPath)) ->
                tryArgumentValue parameterName index argument argumentPath
                |> ValueOption.iter (this.RetargetValue useSource)
            | ValueNone ->
                match tryForwardingBinding node path 0 with
                | ValueSome(struct (name, applied)) when applied <= group -> this.EnqueueParameter useSource name (group - applied) index
                | _ -> ()
        | None -> ()

    member this.Seed (source: Source) (caretNode: CaretNode) =
        match caretNode with
        | CaretNode.Expr(node, path) ->
            this.AddOrFail source (kind.ExprChanges source.Text toStruct node path)
            this.FlowOut source node path
        | CaretNode.Pat(node, path) ->
            this.AddOrFail source (kind.PatChanges source.Text toStruct node path)

            for annotation in annotationsOn node path do
                this.AddAnnotation source annotation

            match tryMatchedExpression node path, tryWholeArgument node path with
            | ValueSome(struct (matched, matchedPath)), _ -> this.Retarget source matched matchedPath
            | ValueNone, ValueSome(struct (name, group)) -> this.EnqueueParameter source name group ValueNone
            | ValueNone, ValueNone -> ()
        | CaretNode.Type(node, annotated, isWholeAnnotation) ->
            this.Add source (kind.TypeChanges source.Text toStruct isWholeAnnotation node)

            if isWholeAnnotation then
                this.SeedAnnotated source annotated

    member this.SeedAnnotated (source: Source) (annotated: Annotated) =
        match annotated with
        | Annotated.Pattern(SynPat.Typed(pat = inner) as pat, path) -> this.SeedTypedPattern source pat inner path
        | Annotated.Pattern _ -> ()
        | Annotated.Expression(expr, path) ->
            this.Retarget source expr path
            this.FlowOut source expr path
        | Annotated.Return(SynBinding(headPat = FunctionHead name)) -> this.EnqueueResult source name
        | Annotated.Return(SynBinding(headPat = headPat)) -> tryPatternIdent headPat |> ValueOption.iter (this.EnqueueValue source)
        | Annotated.Field(SynField(idOpt = Some ident)) -> this.EnqueueValue source ident
        | Annotated.Field _ -> ()

    member this.SeedTypedPattern (source: Source) (pat: SynPat) (inner: SynPat) (path: SyntaxVisitorPath) =
        match tryPatternIdent inner with
        | ValueSome ident -> this.EnqueueValue source ident
        | ValueNone ->
            this.IntoPattern source inner (SyntaxNode.SynPat pat :: path)
            tryMatchedExpression pat path |> ValueOption.iter (this.RetargetValue source)

        tryParameterPosition pat path
        |> ValueOption.iter (fun struct (name, group, index) -> this.EnqueueParameter source name group index)

    member this.Run(document: Document, caretNode: CaretNode) =
        cancellableTask {
            let! source = this.Load document
            this.Seed source caretNode

            while pending.Count > 0 && not failed do
                match pending.Dequeue() with
                | Slot.Value(symbolUse, slotSource) -> do! this.ProcessValue symbolUse slotSource
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

let private conversionAction
    (document: Document)
    (kind: StructKind)
    (caretNode: CaretNode)
    (toStructTitle: unit -> string)
    (toReferenceTitle: unit -> string)
    (userOpName: string)
    =
    let isStruct = kind.IsStruct caretNode
    let title = if isStruct then toReferenceTitle () else toStructTitle ()

    let changedSolution =
        cancellableTask {
            let! converted = Engine(document.Project.Solution, kind, not isStruct, userOpName).Run(document, caretNode)
            return converted |> ValueOption.defaultValue document.Project.Solution
        }

    CodeAction.Create(
        title,
        Func<CancellationToken, Task<Solution>>(fun cancellationToken -> CancellableTask.start cancellationToken changedSolution),
        title
    )

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

            let hasCandidate =
                (tryCaretNode kind (fun _ -> false) caret parseResults.ParseTree).IsSome

            let! found =
                if hasCandidate then
                    cancellableTask {
                        let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync userOpName
                        return tryCaretNode kind (callsMethod sourceText checkResults) caret parseResults.ParseTree
                    }
                else
                    CancellableTask.singleton ValueNone

            match found with
            | ValueSome caretNode when not (isInQuotation caretNode) && canChange kind sourceText caretNode ->
                context.RegisterRefactoring(conversionAction document kind caretNode toStructTitle toReferenceTitle userOpName)
            | _ -> ()
    }
