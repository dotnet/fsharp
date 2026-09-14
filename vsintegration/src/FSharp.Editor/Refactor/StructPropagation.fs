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

/// The name of the function applied by the expression, and how many arguments are applied before it.
let rec private tryCallee (expr: SynExpr) (applied: int) =
    match expr with
    | SynExpr.App(isInfix = false; funcExpr = funcExpr) -> tryCallee funcExpr (applied + 1)
    | SynExpr.TypeApp(expr = inner) -> tryCallee inner applied
    | SynExpr.Ident ident -> ValueSome(struct (ident, applied))
    | SynExpr.LongIdent(longDotId = SynLongIdent(id = ids))
    | SynExpr.DotGet(longDotId = SynLongIdent(id = ids)) ->
        match List.tryLast ids with
        | Some ident -> ValueSome(struct (ident, applied))
        | None -> ValueNone
    | _ -> ValueNone

/// The application of a function reference to all of its curried argument groups.
let rec private tryApplication (node: SynExpr) (path: SyntaxVisitorPath) (remaining: int) =
    if remaining = 0 then
        ValueSome(struct (node, path))
    else
        match path with
        | SyntaxNode.SynExpr(SynExpr.TypeApp(expr = inner) as typeApp) :: rest when isSame inner node ->
            tryApplication typeApp rest remaining
        | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = funcExpr) as app) :: rest when isSame funcExpr node ->
            tryApplication app rest (remaining - 1)
        | _ -> ValueNone

/// The argument a function reference is applied to in the given curried group.
let rec private tryArgument (node: SynExpr) (path: SyntaxVisitorPath) (group: int) =
    match path with
    | SyntaxNode.SynExpr(SynExpr.TypeApp(expr = inner) as typeApp) :: rest when isSame inner node -> tryArgument typeApp rest group
    | SyntaxNode.SynExpr(SynExpr.App(isInfix = false; funcExpr = funcExpr; argExpr = argument) as app) :: rest when isSame funcExpr node ->
        if group = 0 then
            ValueSome(struct (argument, SyntaxNode.SynExpr app :: rest))
        else
            tryArgument app rest (group - 1)
    | _ -> ValueNone

/// The function and the position of the parameter a pattern in a binding's head declares.
let rec private tryParameterPosition (pat: SynPat) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynPat(SynPat.LongIdent(longDotId = SynLongIdent(id = ids); argPats = SynArgPats.Pats args)) :: SyntaxNode.SynBinding _ :: _ ->
        let found =
            args
            |> List.indexed
            |> List.tryFind (fun (_, arg) -> containsRange arg.Range pat.Range)

        match found, List.tryLast ids with
        | Some(group, arg), Some name ->
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

            ValueSome(struct (name, group, index))
        | _ -> ValueNone
    | _ :: rest -> tryParameterPosition pat rest
    | [] -> ValueNone

/// The expression a pattern takes apart: the right-hand side of its binding or the matched expression.
let rec private tryMatchedExpression (pat: SynPat) (path: SyntaxVisitorPath) =
    match path with
    | SyntaxNode.SynPat(SynPat.Paren _ as paren) :: rest -> tryMatchedExpression paren rest
    | SyntaxNode.SynBinding(SynBinding(headPat = headPat; expr = body)) :: _ when isSame headPat pat -> ValueSome(struct (body, path))
    | SyntaxNode.SynMatchClause(SynMatchClause(pat = clausePat)) :: SyntaxNode.SynExpr(SynExpr.Match(expr = scrutinee) as matchExpr) :: rest when
        isSame clausePat pat
        ->
        ValueSome(struct (scrutinee, SyntaxNode.SynExpr matchExpr :: rest))
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

/// The pattern declaring a parameter of the function declared at the range: its whole curried argument, or one
/// element of an argument that is a tuple of parameters.
let private tryParameterPattern (tree: ParsedInput) (declaration: range) (group: int) (index: int voption) =
    (ValueNone, tree)
    ||> ParsedInput.fold (fun found _ node ->
        match found, node with
        | ValueNone,
          SyntaxNode.SynBinding(SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = ids); argPats = SynArgPats.Pats args))) when
            group < args.Length
            && List.tryLast ids
               |> Option.exists (fun ident -> Position.posEq ident.idRange.Start declaration.Start)
            ->
            match stripParenPats (List.item group args), index with
            | SynPat.Tuple(elementPats = elements), ValueSome index when index < elements.Length ->
                ValueSome(stripParenPats (List.item index elements))
            | SynPat.Tuple _, _
            | _, ValueSome _ -> ValueNone
            | parameter, ValueNone -> ValueSome parameter
        | _ -> found)

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
                | :? FSharpField -> true
                | :? FSharpMemberOrFunctionOrValue as mfv -> not mfv.IsFunction && not mfv.IsMember
                | _ -> false

            if isValue then
                this.Enqueue (keyOf "S" symbolUse.Symbol) (Slot.Symbol(symbolUse, source))
        | _ -> ()

    member this.EnqueueResult (source: Source) (name: Ident) =
        match symbolUseAt source name with
        | Some functionUse when (tryDeclarationDocument functionUse.Symbol).IsSome ->
            match functionUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv when mfv.IsFunction || mfv.IsMember ->
                this.Enqueue (keyOf "R" functionUse.Symbol) (Slot.Result(functionUse, source))
            | _ -> ()
        | _ -> ()

    member this.EnqueueParameter (source: Source) (functionName: Ident) (group: int) (index: int voption) =
        match symbolUseAt source functionName with
        | Some functionUse when (tryDeclarationDocument functionUse.Symbol).IsSome ->
            match functionUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv when mfv.IsFunction || mfv.IsMember ->
                let position =
                    match index with
                    | ValueSome index -> $"{group}|{index}"
                    | ValueNone -> $"{group}"

                this.Enqueue
                    (keyOf "P" functionUse.Symbol |> Option.map (fun key -> $"{key}|{position}"))
                    (Slot.Parameter(functionUse, source, group, index))
            | _ -> ()
        | _ -> ()

    /// A value of the changing form flows into a pattern.
    member this.IntoPattern (source: Source) (pat: SynPat) (path: SyntaxVisitorPath) =
        match pat with
        | SynPat.Paren(pat = inner) -> this.IntoPattern source inner (SyntaxNode.SynPat pat :: path)
        | SynPat.Typed(pat = inner; targetType = annotation) ->
            this.Add source (annotationChanges source.Text annotation)
            this.IntoPattern source inner (SyntaxNode.SynPat pat :: path)
        | SynPat.Named(ident = SynIdent(ident, _))
        | SynPat.LongIdent(longDotId = SynLongIdent(id = [ ident ]); argPats = SynArgPats.Pats []) -> this.EnqueueSymbol source ident
        | _ when kind.IsPat pat -> this.AddOrFail source (kind.PatChanges source.Text toStruct pat path)
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
        | SyntaxNode.SynMatchClause(SynMatchClause(resultExpr = result)) :: SyntaxNode.SynExpr(SynExpr.Match _ as container) :: rest when
            isSame result node
            ->
            this.Retarget source container rest
            this.FlowOut source container rest
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
                match stripParenPats pat with
                | stripped when kind.IsPat stripped ->
                    let strippedPath =
                        match pat with
                        | SynPat.Paren _ -> [ SyntaxNode.SynPat pat ]
                        | _ -> [ SyntaxNode.SynMatchClause clause; SyntaxNode.SynExpr matchExpr ] @ rest

                    this.AddOrFail source (kind.PatChanges source.Text toStruct stripped strippedPath)
                | _ -> ()
        | _ -> ()

    /// The expression must now produce the changing form: change what it is built from.
    member this.Retarget (source: Source) (expr: SynExpr) (path: SyntaxVisitorPath) =
        let childPath = SyntaxNode.SynExpr expr :: path

        match expr with
        | SynExpr.Paren(expr = inner) -> this.Retarget source inner childPath
        | SynExpr.Typed(expr = inner; targetType = annotation) ->
            this.Add source (annotationChanges source.Text annotation)
            this.Retarget source inner childPath
        | _ when kind.IsExpr expr path -> this.AddOrFail source (kind.ExprChanges source.Text toStruct expr path)
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
            | SyntaxNode.SynBinding(SynBinding(headPat = SynPat.Named(ident = SynIdent(ident, _)); expr = body)) when isDeclared ident ->
                this.Retarget source body (node :: path)
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

    /// Changes the declared or inferred result of a function.
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
                let groups = max 1 mfv.CurriedParameterGroups.Count
                let! uses = SymbolHelpers.getSymbolUses functionUse source.Document source.Check

                for useDocument, useRange in uses do
                    if not (isDeclaration useRange declaration) then
                        let! useSource = this.Load useDocument

                        match tryUseNode useSource.Tree useRange with
                        | Some(node, path) ->
                            match tryApplication node path groups with
                            | ValueSome(struct (application, rest)) -> this.FlowOut useSource application rest
                            | ValueNone -> ()
                        | None -> ()
            | _ -> ()
        }

    member this.ProcessParameter (functionUse: FSharpSymbolUse) (source: Source) (group: int) (index: int voption) =
        cancellableTask {
            match tryDeclarationDocument functionUse.Symbol with
            | ValueSome(struct (declaration, document)) ->
                let! definition = this.Load document

                match tryParameterPattern definition.Tree declaration group index with
                | ValueSome parameter ->
                    match parameter with
                    | SynPat.Typed(targetType = annotation) -> this.Add definition (annotationChanges definition.Text annotation)
                    | _ -> ()

                    match tryPatternIdent parameter with
                    | ValueSome ident -> this.EnqueueSymbol definition ident
                    | ValueNone -> ()

                    let! uses = SymbolHelpers.getSymbolUses functionUse source.Document source.Check

                    for useDocument, useRange in uses do
                        if not (isDeclaration useRange declaration) then
                            let! useSource = this.Load useDocument

                            match tryUseNode useSource.Tree useRange with
                            | Some(node, path) ->
                                match tryArgument node path group, index with
                                | ValueSome(struct (SynExpr.Paren(expr = SynExpr.Tuple(exprs = exprs) as tuple) as argument, argumentPath)),
                                  ValueSome index when index < exprs.Length ->
                                    this.Retarget
                                        useSource
                                        (List.item index exprs)
                                        (SyntaxNode.SynExpr tuple :: SyntaxNode.SynExpr argument :: argumentPath)
                                | ValueSome(struct (argument, argumentPath)), ValueNone -> this.Retarget useSource argument argumentPath
                                | _ -> ()
                            | None -> ()
                | ValueNone -> ()
            | ValueNone -> ()
        }

    member this.Seed (source: Source) (caretNode: CaretNode) =
        match caretNode with
        | CaretNode.Expr(node, path) ->
            this.AddOrFail source (kind.ExprChanges source.Text toStruct node path)
            this.FlowOut source node path
        | CaretNode.Pat(node, path) ->
            this.AddOrFail source (kind.PatChanges source.Text toStruct node path)

            match tryMatchedExpression node path with
            | ValueSome(struct (matched, matchedPath)) -> this.Retarget source matched matchedPath
            | ValueNone -> ()
        | CaretNode.Type(node, annotated, isWholeAnnotation) ->
            this.Add source (kind.TypeChanges source.Text toStruct isWholeAnnotation node)

            if isWholeAnnotation then
                match annotated with
                | Annotated.Pattern(SynPat.Typed(pat = inner) as pat, path) ->
                    match tryPatternIdent inner with
                    | ValueSome ident -> this.EnqueueSymbol source ident
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

            match tryCaretNode kind caret parseResults.ParseTree with
            | ValueSome caretNode when not (isInQuotation caretNode) ->
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
