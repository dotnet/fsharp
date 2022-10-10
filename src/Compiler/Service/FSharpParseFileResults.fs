// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO
open System.Collections.Generic
open System.Diagnostics
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range

module SourceFileImpl =
    let IsSignatureFile file =
        let ext = Path.GetExtension file
        0 = String.Compare(".fsi", ext, StringComparison.OrdinalIgnoreCase)

    /// Additional #defines that should be in place when editing a file in a file editor such as VS.
    let GetImplicitConditionalDefinesForEditing (isInteractive: bool) =
        if isInteractive then
            [ "INTERACTIVE"; "EDITING" ] // This is still used by the foreground parse
        else
            [ "COMPILED"; "EDITING" ]

type CompletionPath = string list * string option // plid * residue

[<RequireQualifiedAccess>]
type FSharpInheritanceOrigin =
    | Class
    | Interface
    | Unknown

[<RequireQualifiedAccess>]
type InheritanceContext =
    | Class
    | Interface
    | Unknown

[<RequireQualifiedAccess>]
type RecordContext =
    | CopyOnUpdate of range: range * path: CompletionPath
    | Constructor of typeName: string
    | New of path: CompletionPath

[<RequireQualifiedAccess>]
type CompletionContext =
    /// Completion context cannot be determined due to errors
    | Invalid

    /// Completing something after the inherit keyword
    | Inherit of context: InheritanceContext * path: CompletionPath

    /// Completing records field
    | RecordField of context: RecordContext

    | RangeOperator

    /// Completing named parameters\setters in parameter list of constructor\method calls
    /// end of name ast node * list of properties\parameters that were already set
    | ParameterList of pos * HashSet<string>

    | AttributeApplication

    | OpenDeclaration of isOpenType: bool

    /// Completing pattern type (e.g. foo (x: |))
    | PatternType

//----------------------------------------------------------------------------
// FSharpParseFileResults
//----------------------------------------------------------------------------

[<Sealed>]
type FSharpParseFileResults(diagnostics: FSharpDiagnostic[], input: ParsedInput, parseHadErrors: bool, dependencyFiles: string[]) =

    member _.Diagnostics = diagnostics

    member _.ParseHadErrors = parseHadErrors

    member _.ParseTree = input

    member _.TryRangeOfNameOfNearestOuterBindingContainingPos pos =
        let tryGetIdentRangeFromBinding binding =
            match binding with
            | SynBinding (headPat = headPat) ->
                match headPat with
                | SynPat.LongIdent (longDotId = longIdentWithDots) -> Some longIdentWithDots.Range
                | SynPat.As(rhsPat = SynPat.Named (ident = SynIdent (ident, _); isThisVal = false))
                | SynPat.Named (SynIdent (ident, _), false, _, _) -> Some ident.idRange
                | _ -> None

        let rec walkBinding expr workingRange =
            match expr with

            // This lets us dive into subexpressions that may contain the binding we're after
            | SynExpr.Sequential (_, _, expr1, expr2, _) ->
                if rangeContainsPos expr1.Range pos then
                    walkBinding expr1 workingRange
                else
                    walkBinding expr2 workingRange

            | SynExpr.LetOrUse (bindings = bindings; body = bodyExpr) ->
                let potentialNestedRange =
                    bindings
                    |> List.tryFind (fun binding -> rangeContainsPos binding.RangeOfBindingWithRhs pos)
                    |> Option.bind tryGetIdentRangeFromBinding

                match potentialNestedRange with
                | Some range -> walkBinding bodyExpr range
                | None -> walkBinding bodyExpr workingRange

            | _ -> Some workingRange

        let visitor =
            { new SyntaxVisitorBase<_>() with
                override _.VisitExpr(_, _, defaultTraverse, expr) = defaultTraverse expr

                override _.VisitBinding(_path, defaultTraverse, binding) =
                    match binding with
                    | SynBinding (valData = SynValData(memberFlags = None); expr = expr) as b when
                        rangeContainsPos b.RangeOfBindingWithRhs pos
                        ->
                        match tryGetIdentRangeFromBinding b with
                        | Some range -> walkBinding expr range
                        | None -> None
                    | _ -> defaultTraverse binding
            }

        SyntaxTraversal.Traverse(pos, input, visitor)

    member _.TryIdentOfPipelineContainingPosAndNumArgsApplied pos =
        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.App (_,
                                   _,
                                   SynExpr.App (_, true, SynExpr.LongIdent(longDotId = SynLongIdent(id = [ ident ])), _, _),
                                   argExpr,
                                   _) when rangeContainsPos argExpr.Range pos ->
                        match argExpr with
                        | SynExpr.App (_, _, _, SynExpr.Paren (expr, _, _, _), _) when rangeContainsPos expr.Range pos -> None
                        | _ ->
                            if ident.idText = "op_PipeRight" then Some(ident, 1)
                            elif ident.idText = "op_PipeRight2" then Some(ident, 2)
                            elif ident.idText = "op_PipeRight3" then Some(ident, 3)
                            else None
                    | _ -> defaultTraverse expr
            }

        SyntaxTraversal.Traverse(pos, input, visitor)

    member _.IsPosContainedInApplication pos =
        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, traverseSynExpr, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.TypeApp (_, _, _, _, _, _, range) when rangeContainsPos range pos -> Some range
                    | SynExpr.App (_, _, _, SynExpr.ComputationExpr (_, expr, _), range) when rangeContainsPos range pos ->
                        traverseSynExpr expr
                    | SynExpr.App (_, _, _, _, range) when rangeContainsPos range pos -> Some range
                    | _ -> defaultTraverse expr
            }

        let result = SyntaxTraversal.Traverse(pos, input, visitor)
        result.IsSome

    member _.TryRangeOfFunctionOrMethodBeingApplied pos =
        let rec getIdentRangeForFuncExprInApp traverseSynExpr expr pos =
            match expr with
            | SynExpr.Ident ident -> Some ident.idRange

            | SynExpr.LongIdent (_, _, _, range) -> Some range

            | SynExpr.Paren (expr, _, _, range) when rangeContainsPos range pos -> getIdentRangeForFuncExprInApp traverseSynExpr expr pos

            | SynExpr.TypeApp (expr, _, _, _, _, _, _) -> getIdentRangeForFuncExprInApp traverseSynExpr expr pos

            | SynExpr.App (_, _, funcExpr, argExpr, _) ->
                match argExpr with
                | SynExpr.App (_, _, _, _, range) when rangeContainsPos range pos ->
                    getIdentRangeForFuncExprInApp traverseSynExpr argExpr pos

                // Special case: `async { ... }` is actually a ComputationExpr inside of the argExpr of a SynExpr.App
                | SynExpr.ComputationExpr (_, expr, range)
                | SynExpr.Paren (expr, _, _, range) when rangeContainsPos range pos ->
                    getIdentRangeForFuncExprInApp traverseSynExpr expr pos

                // Yielding values in an array or list that is used as an argument: List.sum [ getVal a b; getVal b c ]
                | SynExpr.ArrayOrListComputed (_, expr, range) when rangeContainsPos range pos ->
                    if rangeContainsPos expr.Range pos then
                        getIdentRangeForFuncExprInApp traverseSynExpr expr pos
                    else
                        (*
                            In cases like

                            let test () = div [] [
                                str ""
                                ; |
                            ]

                            `ProvideParametersAsyncAux` currently works with the wrong symbol or
                            doesn't detect the previously applied arguments.
                            Until that is fixed, don't show any tooltips rather than the wrong signature.
                        *)
                        None

                | _ ->
                    match funcExpr with
                    | SynExpr.App (_, true, _, _, _) when rangeContainsPos argExpr.Range pos ->
                        // x |> List.map
                        // Don't dive into the funcExpr (the operator expr)
                        // because we dont want to offer sig help for that!
                        getIdentRangeForFuncExprInApp traverseSynExpr argExpr pos
                    | _ ->
                        // Generally, we want to dive into the func expr to get the range
                        // of the identifier of the function we're after
                        getIdentRangeForFuncExprInApp traverseSynExpr funcExpr pos

            | SynExpr.Sequential (_, _, expr1, expr2, range) when rangeContainsPos range pos ->
                if rangeContainsPos expr1.Range pos then
                    getIdentRangeForFuncExprInApp traverseSynExpr expr1 pos
                else
                    getIdentRangeForFuncExprInApp traverseSynExpr expr2 pos

            | SynExpr.LetOrUse (bindings = bindings; body = body; range = range) when rangeContainsPos range pos ->
                let binding =
                    bindings |> List.tryFind (fun x -> rangeContainsPos x.RangeOfBindingWithRhs pos)

                match binding with
                | Some (SynBinding.SynBinding (expr = expr)) -> getIdentRangeForFuncExprInApp traverseSynExpr expr pos
                | None -> getIdentRangeForFuncExprInApp traverseSynExpr body pos

            | SynExpr.IfThenElse (ifExpr = ifExpr; thenExpr = thenExpr; elseExpr = elseExpr; range = range) when rangeContainsPos range pos ->
                if rangeContainsPos ifExpr.Range pos then
                    getIdentRangeForFuncExprInApp traverseSynExpr ifExpr pos
                elif rangeContainsPos thenExpr.Range pos then
                    getIdentRangeForFuncExprInApp traverseSynExpr thenExpr pos
                else
                    match elseExpr with
                    | None -> None
                    | Some expr -> getIdentRangeForFuncExprInApp traverseSynExpr expr pos

            | SynExpr.Match (expr = expr; clauses = clauses; range = range) when rangeContainsPos range pos ->
                if rangeContainsPos expr.Range pos then
                    getIdentRangeForFuncExprInApp traverseSynExpr expr pos
                else
                    let clause =
                        clauses |> List.tryFind (fun clause -> rangeContainsPos clause.Range pos)

                    match clause with
                    | None -> None
                    | Some clause ->
                        match clause with
                        | SynMatchClause.SynMatchClause (whenExpr = whenExprOpt; resultExpr = resultExpr) ->
                            match whenExprOpt with
                            | None -> getIdentRangeForFuncExprInApp traverseSynExpr resultExpr pos
                            | Some whenExpr ->
                                if rangeContainsPos whenExpr.Range pos then
                                    getIdentRangeForFuncExprInApp traverseSynExpr whenExpr pos
                                else
                                    getIdentRangeForFuncExprInApp traverseSynExpr resultExpr pos

            // Ex: C.M(x, y, ...) <--- We want to find where in the tupled application the call is being made
            | SynExpr.Tuple (_, exprs, _, tupRange) when rangeContainsPos tupRange pos ->
                let expr = exprs |> List.tryFind (fun expr -> rangeContainsPos expr.Range pos)

                match expr with
                | None -> None
                | Some expr -> getIdentRangeForFuncExprInApp traverseSynExpr expr pos

            // Capture the body of a lambda, often nested in a call to a collection function
            | SynExpr.Lambda (body = body) when rangeContainsPos body.Range pos -> getIdentRangeForFuncExprInApp traverseSynExpr body pos

            | SynExpr.Do (expr, range) when rangeContainsPos range pos -> getIdentRangeForFuncExprInApp traverseSynExpr expr pos

            | SynExpr.Assert (expr, range) when rangeContainsPos range pos -> getIdentRangeForFuncExprInApp traverseSynExpr expr pos

            | SynExpr.ArbitraryAfterError (_debugStr, range) when rangeContainsPos range pos -> Some range

            | expr -> traverseSynExpr expr

        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, traverseSynExpr, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.TypeApp (expr, _, _, _, _, _, range) when rangeContainsPos range pos ->
                        getIdentRangeForFuncExprInApp traverseSynExpr expr pos
                    | SynExpr.App (_, _, _funcExpr, _, range) as app when rangeContainsPos range pos ->
                        getIdentRangeForFuncExprInApp traverseSynExpr app pos
                    | _ -> defaultTraverse expr
            }

        SyntaxTraversal.Traverse(pos, input, visitor)

    member _.GetAllArgumentsForFunctionApplicationAtPostion pos =
        SynExprAppLocationsImpl.getAllCurriedArgsAtPosition pos input

    member _.TryRangeOfParenEnclosingOpEqualsGreaterUsage opGreaterEqualPos =
        let (|Ident|_|) ofName =
            function
            | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ ident ])) when ident.idText = ofName -> Some()
            | _ -> None

        let (|InfixAppOfOpEqualsGreater|_|) =
            function
            | SynExpr.App (ExprAtomicFlag.NonAtomic,
                           false,
                           SynExpr.App (ExprAtomicFlag.NonAtomic, true, Ident "op_EqualsGreater", actualParamListExpr, _),
                           actualLambdaBodyExpr,
                           _) -> Some(actualParamListExpr, actualLambdaBodyExpr)
            | _ -> None

        SyntaxTraversal.Traverse(
            opGreaterEqualPos,
            input,
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.Paren (InfixAppOfOpEqualsGreater (lambdaArgs, lambdaBody) as app, _, _, _) ->
                        Some(app.Range, lambdaArgs.Range, lambdaBody.Range)
                    | _ -> defaultTraverse expr

                member _.VisitBinding(_path, defaultTraverse, binding) =
                    match binding with
                    | SynBinding (kind = SynBindingKind.Normal; expr = InfixAppOfOpEqualsGreater (lambdaArgs, lambdaBody) as app) ->
                        Some(app.Range, lambdaArgs.Range, lambdaBody.Range)
                    | _ -> defaultTraverse binding
            }
        )

    member _.TryRangeOfStringInterpolationContainingPos pos =
        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.InterpolatedString (range = range) when rangeContainsPos range pos -> Some range
                    | _ -> defaultTraverse expr
            }

        SyntaxTraversal.Traverse(pos, input, visitor)

    member _.TryRangeOfExprInYieldOrReturn pos =
        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_path, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.YieldOrReturn (_, expr, range)
                    | SynExpr.YieldOrReturnFrom (_, expr, range) when rangeContainsPos range pos -> Some expr.Range
                    | _ -> defaultTraverse expr
            }

        SyntaxTraversal.Traverse(pos, input, visitor)

    member _.TryRangeOfRecordExpressionContainingPos pos =
        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.Record (_, _, _, range) when rangeContainsPos range pos -> Some range
                    | _ -> defaultTraverse expr
            }

        SyntaxTraversal.Traverse(pos, input, visitor)

    member _.TryRangeOfRefCellDereferenceContainingPos expressionPos =
        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.App (_, false, SynExpr.LongIdent(longDotId = SynLongIdent(id = [ funcIdent ])), expr, _) ->
                        if funcIdent.idText = "op_Dereference" && rangeContainsPos expr.Range expressionPos then
                            Some funcIdent.idRange
                        else
                            None
                    | _ -> defaultTraverse expr
            }

        SyntaxTraversal.Traverse(expressionPos, input, visitor)

    member _.TryRangeOfExpressionBeingDereferencedContainingPos expressionPos =
        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.App (_, false, SynExpr.LongIdent(longDotId = SynLongIdent(id = [ funcIdent ])), expr, _) ->
                        if funcIdent.idText = "op_Dereference" && rangeContainsPos expr.Range expressionPos then
                            Some expr.Range
                        else
                            None
                    | _ -> defaultTraverse expr
            }

        SyntaxTraversal.Traverse(expressionPos, input, visitor)

    member _.FindParameterLocations pos = ParameterLocations.Find(pos, input)

    member _.IsPositionContainedInACurriedParameter pos =
        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_path, traverseSynExpr, defaultTraverse, expr) = defaultTraverse (expr)

                override _.VisitBinding(_path, _, binding) =
                    match binding with
                    | SynBinding (valData = valData; range = range) when rangeContainsPos range pos ->
                        let info = valData.SynValInfo.CurriedArgInfos
                        let mutable found = false

                        for group in info do
                            for arg in group do
                                match arg.Ident with
                                | Some ident when rangeContainsPos ident.idRange pos -> found <- true
                                | _ -> ()

                        if found then Some range else None
                    | _ -> None
            }

        let result = SyntaxTraversal.Traverse(pos, input, visitor)
        result.IsSome

    member _.IsTypeAnnotationGivenAtPosition pos =
        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_path, _traverseSynExpr, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.Typed (_expr, _typeExpr, range) when Position.posEq range.Start pos -> Some range
                    | _ -> defaultTraverse expr

                override _.VisitSimplePats(_path, pats) =
                    match pats with
                    | [] -> None
                    | _ ->
                        let exprFunc pat =
                            match pat with
                            // (s: string)
                            | SynSimplePat.Typed (_pat, _targetExpr, range) when Position.posEq range.Start pos -> Some range
                            | _ -> None

                        pats |> List.tryPick exprFunc

                override _.VisitPat(_path, defaultTraverse, pat) =
                    // (s: string)
                    match pat with
                    | SynPat.Typed (_pat, _targetType, range) when Position.posEq range.Start pos -> Some range
                    | _ -> defaultTraverse pat

                override _.VisitBinding(_path, defaultTraverse, binding) =
                    // let x : int = 12
                    match binding with
                    | SynBinding (headPat = SynPat.Named (range = patRange)
                                  returnInfo = Some (SynBindingReturnInfo(typeName = SynType.LongIdent _))) -> Some patRange
                    | _ -> defaultTraverse binding
            }

        let result = SyntaxTraversal.Traverse(pos, input, visitor)
        result.IsSome

    member _.IsBindingALambdaAtPosition pos =
        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_path, _traverseSynExpr, defaultTraverse, expr) = defaultTraverse expr

                override _.VisitBinding(_path, defaultTraverse, binding) =
                    match binding with
                    | SynBinding.SynBinding (expr = expr; range = range) when Position.posEq range.Start pos ->
                        match expr with
                        | SynExpr.Lambda _ -> Some range
                        | _ -> None
                    | _ -> defaultTraverse binding
            }

        let result = SyntaxTraversal.Traverse(pos, input, visitor)
        result.IsSome

    /// Get declared items and the selected item at the specified location
    member _.GetNavigationItemsImpl() =
        DiagnosticsScope.Protect
            range0
            (fun () ->
                match input with
                | ParsedInput.ImplFile _ as p -> Navigation.getNavigation p
                | ParsedInput.SigFile _ -> Navigation.empty)
            (fun err ->
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetNavigationItemsImpl: '%s'" err)
                Navigation.empty)

    member _.ValidateBreakpointLocationImpl pos =
        let isMatchRange m =
            rangeContainsPos m pos || m.StartLine = pos.Line

        // Process let-binding
        let findBreakPoints () =
            let checkRange m =
                [
                    if isMatchRange m && not m.IsSynthetic then
                        yield m
                ]

            let walkBindSeqPt sp =
                [
                    match sp with
                    | DebugPointAtBinding.Yes m -> yield! checkRange m
                    | _ -> ()
                ]

            let walkForSeqPt sp =
                [
                    match sp with
                    | DebugPointAtFor.Yes m -> yield! checkRange m
                    | _ -> ()
                ]

            let walkInOrToSeqPt sp =
                [
                    match sp with
                    | DebugPointAtInOrTo.Yes m -> yield! checkRange m
                    | _ -> ()
                ]

            let walkWhileSeqPt sp =
                [
                    match sp with
                    | DebugPointAtWhile.Yes m -> yield! checkRange m
                    | _ -> ()
                ]

            let walkTrySeqPt sp =
                [
                    match sp with
                    | DebugPointAtTry.Yes m -> yield! checkRange m
                    | _ -> ()
                ]

            let walkWithSeqPt sp =
                [
                    match sp with
                    | DebugPointAtWith.Yes m -> yield! checkRange m
                    | _ -> ()
                ]

            let walkFinallySeqPt sp =
                [
                    match sp with
                    | DebugPointAtFinally.Yes m -> yield! checkRange m
                    | _ -> ()
                ]

            let rec walkBind (SynBinding (kind = kind; expr = synExpr; debugPoint = spInfo; range = m)) =
                [
                    yield! walkBindSeqPt spInfo
                    let extendDebugPointForDo =
                        match kind with
                        | SynBindingKind.Do -> not (IsControlFlowExpression synExpr)
                        | _ -> false

                    // This extends the range of the implicit debug point for 'do expr' range to include the 'do'
                    if extendDebugPointForDo then
                        yield! checkRange m

                    let useImplicitDebugPoint =
                        match spInfo with
                        | DebugPointAtBinding.Yes _ -> false
                        | _ -> not extendDebugPointForDo

                    yield! walkExpr useImplicitDebugPoint synExpr
                ]

            and walkExprs exprs = exprs |> List.collect (walkExpr false)

            and walkBinds exprs = exprs |> List.collect walkBind

            and walkMatchClauses clauses =
                [
                    for SynMatchClause (whenExpr = whenExprOpt; resultExpr = tgtExpr) in clauses do
                        match whenExprOpt with
                        | Some whenExpr -> yield! walkExpr false whenExpr
                        | _ -> ()

                        yield! walkExpr true tgtExpr
                ]

            and walkExprOpt (spImplicit: bool) eOpt =
                [
                    match eOpt with
                    | Some e -> yield! walkExpr spImplicit e
                    | _ -> ()
                ]

            // Determine the breakpoint locations for an expression. spImplicit indicates we always
            // emit a breakpoint location for the expression unless it is a syntactic control flow construct
            and walkExpr spImplicit expr =
                let m = expr.Range

                [
                    if isMatchRange m then
                        if spImplicit && not (IsControlFlowExpression expr) then
                            yield! checkRange m

                        match expr with
                        | SynExpr.ArbitraryAfterError _
                        | SynExpr.LongIdent _
                        | SynExpr.LibraryOnlyILAssembly _
                        | SynExpr.LibraryOnlyStaticOptimization _
                        | SynExpr.Null _
                        | SynExpr.Typar _
                        | SynExpr.Ident _
                        | SynExpr.ImplicitZero _
                        | SynExpr.Const _
                        | SynExpr.Dynamic _ -> ()

                        | SynExpr.Quote (_, _, e, _, _)
                        | SynExpr.TypeTest (e, _, _)
                        | SynExpr.Upcast (e, _, _)
                        | SynExpr.AddressOf (_, e, _, _)
                        | SynExpr.ComputationExpr (_, e, _)
                        | SynExpr.ArrayOrListComputed (_, e, _)
                        | SynExpr.Typed (e, _, _)
                        | SynExpr.FromParseError (e, _)
                        | SynExpr.DiscardAfterMissingQualificationAfterDot (e, _)
                        | SynExpr.Do (e, _)
                        | SynExpr.Assert (e, _)
                        | SynExpr.Fixed (e, _)
                        | SynExpr.DotGet (e, _, _, _)
                        | SynExpr.LongIdentSet (_, e, _)
                        | SynExpr.New (_, _, e, _)
                        | SynExpr.TypeApp (e, _, _, _, _, _, _)
                        | SynExpr.LibraryOnlyUnionCaseFieldGet (e, _, _, _)
                        | SynExpr.Downcast (e, _, _)
                        | SynExpr.InferredUpcast (e, _)
                        | SynExpr.InferredDowncast (e, _)
                        | SynExpr.Lazy (e, _)
                        | SynExpr.TraitCall (_, _, e, _)
                        | SynExpr.Paren (e, _, _, _) -> yield! walkExpr false e

                        | SynExpr.InterpolatedString (parts, _, _) ->
                            yield!
                                walkExprs
                                    [
                                        for part in parts do
                                            match part with
                                            | SynInterpolatedStringPart.String _ -> ()
                                            | SynInterpolatedStringPart.FillExpr (fillExpr, _) -> yield fillExpr
                                    ]

                        | SynExpr.DebugPoint (DebugPointAtLeafExpr.Yes m, isControlFlow, innerExpr) ->
                            yield! checkRange m
                            yield! walkExpr isControlFlow innerExpr

                        | SynExpr.YieldOrReturn (_, e, m) ->
                            yield! checkRange m
                            yield! walkExpr false e

                        | SynExpr.YieldOrReturnFrom (_, e, _)
                        | SynExpr.DoBang (e, _) ->
                            yield! checkRange e.Range
                            yield! walkExpr false e

                        | SynOrElse (e1, e2)
                        | SynAndAlso (e1, e2) ->
                            yield! walkExpr true e1
                            yield! walkExpr true e2

                        // Always allow breakpoints on input and stages of x |> f1 |> f2 pipelines
                        | SynPipeRight _ ->
                            let rec loop e =
                                seq {
                                    match e with
                                    | SynPipeRight (xExpr, fExpr) ->
                                        yield! checkRange fExpr.Range
                                        yield! walkExpr false fExpr
                                        yield! loop xExpr
                                    | SynPipeRight2 (xExpr1, xExpr2, fExpr) ->
                                        yield! checkRange fExpr.Range
                                        yield! checkRange xExpr1.Range
                                        yield! checkRange xExpr2.Range
                                        yield! walkExpr false xExpr1
                                        yield! walkExpr false xExpr2
                                        yield! walkExpr false fExpr
                                    | SynPipeRight3 (xExpr1, xExpr2, xExpr3, fExpr) ->
                                        yield! checkRange fExpr.Range
                                        yield! checkRange xExpr1.Range
                                        yield! checkRange xExpr2.Range
                                        yield! checkRange xExpr3.Range
                                        yield! walkExpr false xExpr1
                                        yield! walkExpr false xExpr2
                                        yield! walkExpr false xExpr3
                                        yield! walkExpr false fExpr
                                    | _ ->
                                        yield! checkRange e.Range
                                        yield! walkExpr false e
                                }

                            yield! loop expr
                        | SynExpr.NamedIndexedPropertySet (_, e1, e2, _)
                        | SynExpr.DotSet (e1, _, e2, _)
                        | SynExpr.Set (e1, e2, _)
                        | SynExpr.LibraryOnlyUnionCaseFieldSet (e1, _, _, e2, _)
                        | SynExpr.App (_, _, e1, e2, _) ->
                            yield! walkExpr false e1
                            yield! walkExpr false e2

                        | SynExpr.ArrayOrList (_, exprs, _)
                        | SynExpr.Tuple (_, exprs, _, _) -> yield! walkExprs exprs

                        | SynExpr.Record (_, copyExprOpt, fs, _) ->
                            match copyExprOpt with
                            | Some (e, _) -> yield! walkExpr true e
                            | None -> ()

                            yield! walkExprs (fs |> List.choose (fun (SynExprRecordField (expr = e)) -> e))

                        | SynExpr.AnonRecd (_isStruct, copyExprOpt, fs, _) ->
                            match copyExprOpt with
                            | Some (e, _) -> yield! walkExpr true e
                            | None -> ()

                            yield! walkExprs (fs |> List.map (fun (_, _, e) -> e))

                        | SynExpr.ObjExpr (argOptions = args; bindings = bs; members = ms; extraImpls = is) ->
                            let bs = unionBindingAndMembers bs ms

                            match args with
                            | None -> ()
                            | Some (arg, _) -> yield! walkExpr false arg

                            yield! walkBinds bs

                            for SynInterfaceImpl (bindings = bs) in is do
                                yield! walkBinds bs

                        | SynExpr.While (spWhile, e1, e2, _) ->
                            yield! walkWhileSeqPt spWhile
                            yield! walkExpr false e1
                            yield! walkExpr true e2

                        | SynExpr.JoinIn (e1, _range, e2, _range2) ->
                            yield! walkExpr false e1
                            yield! walkExpr false e2

                        | SynExpr.For (forDebugPoint = spFor; toDebugPoint = spTo; identBody = e1; toBody = e2; doBody = e3) ->
                            yield! walkForSeqPt spFor
                            yield! walkInOrToSeqPt spTo
                            yield! walkExpr false e1
                            yield! walkExpr true e2
                            yield! walkExpr true e3

                        | SynExpr.ForEach (spFor, spIn, _, _, _, e1, e2, _) ->
                            yield! walkForSeqPt spFor
                            yield! walkInOrToSeqPt spIn
                            yield! walkBindSeqPt (DebugPointAtBinding.Yes e1.Range)
                            yield! walkExpr false e1
                            yield! walkExpr true e2

                        | SynExpr.MatchLambda (_isExnMatch, _argm, cl, spBind, _wholem) ->
                            yield! walkBindSeqPt spBind

                            for SynMatchClause (whenExpr = whenExpr; resultExpr = resultExpr) in cl do
                                yield! walkExprOpt true whenExpr
                                yield! walkExpr true resultExpr

                        | SynExpr.Lambda (body = bodyExpr) -> yield! walkExpr true bodyExpr

                        | SynExpr.Match (matchDebugPoint = spBind; expr = inpExpr; clauses = cl) ->
                            yield! walkBindSeqPt spBind
                            yield! walkExpr false inpExpr

                            for SynMatchClause (whenExpr = whenExpr; resultExpr = tgtExpr) in cl do
                                yield! walkExprOpt true whenExpr
                                yield! walkExpr true tgtExpr

                        | SynExpr.LetOrUse (bindings = binds; body = bodyExpr) ->
                            yield! walkBinds binds
                            yield! walkExpr true bodyExpr

                        | SynExpr.TryWith (tryExpr = tryExpr; withCases = cl; tryDebugPoint = spTry; withDebugPoint = spWith) ->
                            yield! walkTrySeqPt spTry
                            yield! walkWithSeqPt spWith
                            yield! walkExpr true tryExpr
                            yield! walkMatchClauses cl

                        | SynExpr.TryFinally (tryExpr = e1; finallyExpr = e2; tryDebugPoint = spTry; finallyDebugPoint = spFinally) ->
                            yield! walkExpr true e1
                            yield! walkExpr true e2
                            yield! walkTrySeqPt spTry
                            yield! walkFinallySeqPt spFinally

                        | SynExpr.SequentialOrImplicitYield (spSeq, e1, e2, _, _)
                        | SynExpr.Sequential (spSeq, _, e1, e2, _) ->
                            let implicit1 =
                                match spSeq with
                                | DebugPointAtSequential.SuppressExpr
                                | DebugPointAtSequential.SuppressBoth -> false
                                | _ -> true

                            yield! walkExpr implicit1 e1

                            let implicit2 =
                                match spSeq with
                                | DebugPointAtSequential.SuppressStmt
                                | DebugPointAtSequential.SuppressBoth -> false
                                | _ -> true

                            yield! walkExpr implicit2 e2

                        | SynExpr.IfThenElse (ifExpr = e1; thenExpr = e2; elseExpr = e3opt; spIfToThen = spBind) ->
                            yield! walkBindSeqPt spBind
                            yield! walkExpr false e1
                            yield! walkExpr true e2
                            yield! walkExprOpt true e3opt

                        | SynExpr.DotIndexedGet (e1, es, _, _) ->
                            yield! walkExpr false e1
                            yield! walkExpr false es

                        | SynExpr.IndexRange (expr1, _, expr2, _, _, _) ->
                            match expr1 with
                            | Some e -> yield! walkExpr false e
                            | None -> ()

                            match expr2 with
                            | Some e -> yield! walkExpr false e
                            | None -> ()

                        | SynExpr.IndexFromEnd (e, _) -> yield! walkExpr false e

                        | SynExpr.DotIndexedSet (e1, es, e2, _, _, _) ->
                            yield! walkExpr false e1
                            yield! walkExpr false es
                            yield! walkExpr false e2

                        | SynExpr.DotNamedIndexedPropertySet (e1, _, e2, e3, _) ->
                            yield! walkExpr false e1
                            yield! walkExpr false e2
                            yield! walkExpr false e3

                        | SynExpr.LetOrUseBang (spBind, _, _, _, rhsExpr, andBangs, bodyExpr, _, _) ->
                            yield! walkBindSeqPt spBind
                            yield! walkExpr true rhsExpr

                            for SynExprAndBang (debugPoint = andBangSpBind; body = eAndBang) in andBangs do
                                yield! walkBindSeqPt andBangSpBind
                                yield! walkExpr true eAndBang

                            yield! walkExpr true bodyExpr

                        | SynExpr.MatchBang (matchDebugPoint = spBind; expr = inpExpr; clauses = clauses) ->
                            yield! walkBindSeqPt spBind
                            yield! walkExpr false inpExpr

                            for SynMatchClause (whenExpr = whenExpr; resultExpr = resExpr) in clauses do
                                yield! walkExprOpt true whenExpr
                                yield! walkExpr true resExpr
                ]

            // Process a class declaration or F# type declaration
            let rec walkTycon (SynTypeDefn (typeRepr = repr; members = membDefns; implicitConstructor = implicitCtor; range = m)) =
                if not (isMatchRange m) then
                    []
                else
                    [
                        for memb in membDefns do
                            yield! walkMember memb
                        match repr with
                        | SynTypeDefnRepr.ObjectModel (_, membDefns, _) ->
                            for memb in membDefns do
                                yield! walkMember memb
                        | _ -> ()
                        for memb in membDefns do
                            yield! walkMember memb
                        for memb in Option.toList implicitCtor do
                            yield! walkMember memb
                    ]

            // Returns class-members for the right dropdown
            and walkMember memb =
                if not (isMatchRange memb.Range) then
                    []
                else
                    [
                        match memb with
                        | SynMemberDefn.LetBindings (binds, _, _, _) -> yield! walkBinds binds
                        | SynMemberDefn.AutoProperty (synExpr = synExpr) -> yield! walkExpr true synExpr
                        | SynMemberDefn.ImplicitCtor (_, _, _, _, _, m) -> yield! checkRange m
                        | SynMemberDefn.Member (bind, _) -> yield! walkBind bind
                        | SynMemberDefn.GetSetMember (getBinding, setBinding, _, _) ->
                            match getBinding, setBinding with
                            | None, None -> ()
                            | None, Some binding
                            | Some binding, None -> yield! walkBind binding
                            | Some getBinding, Some setBinding ->
                                yield! walkBind getBinding
                                yield! walkBind setBinding
                        | SynMemberDefn.Interface(members = Some membs) ->
                            for m in membs do
                                yield! walkMember m
                        | SynMemberDefn.Inherit (_, _, m) ->
                            // can break on the "inherit" clause
                            yield! checkRange m
                        | SynMemberDefn.ImplicitInherit (_, arg, _, m) ->
                            // can break on the "inherit" clause
                            yield! checkRange m
                            yield! walkExpr true arg
                        | _ -> ()
                    ]

            // Process declarations nested in a module that should be displayed in the left dropdown
            // (such as type declarations, nested modules etc.)
            let rec walkDecl decl =
                [
                    match decl with
                    | SynModuleDecl.Let (_, binds, m) when isMatchRange m -> yield! walkBinds binds
                    | SynModuleDecl.Expr (expr, m) when isMatchRange m -> yield! walkExpr true expr
                    | SynModuleDecl.ModuleAbbrev _ -> ()
                    | SynModuleDecl.NestedModule (decls = decls; range = m) when isMatchRange m ->
                        for d in decls do
                            yield! walkDecl d
                    | SynModuleDecl.Types (tydefs, m) when isMatchRange m ->
                        for d in tydefs do
                            yield! walkTycon d
                    | SynModuleDecl.Exception (SynExceptionDefn (SynExceptionDefnRepr _, _, membDefns, _), m) when isMatchRange m ->
                        for m in membDefns do
                            yield! walkMember m
                    | _ -> ()
                ]

            // Collect all the items in a module
            let walkModule (SynModuleOrNamespace (decls = decls; range = m)) =
                if isMatchRange m then List.collect walkDecl decls else []

            /// Get information for implementation file
            let walkImplFile (modules: SynModuleOrNamespace list) = List.collect walkModule modules

            match input with
            | ParsedInput.ImplFile file -> walkImplFile file.Contents
            | _ -> []

        DiagnosticsScope.Protect
            range0
            (fun () ->
                let locations = findBreakPoints ()

                if pos.Column = 0 then
                    // we have a breakpoint that was set with mouse at line start
                    match
                        locations
                        |> List.filter (fun m -> m.StartLine = m.EndLine && pos.Line = m.StartLine)
                    with
                    | [] ->
                        match locations |> List.filter (fun m -> rangeContainsPos m pos) with
                        | [] ->
                            match locations |> List.filter (fun m -> rangeBeforePos m pos |> not) with
                            | [] -> Seq.tryHead locations
                            | locationsAfterPos -> Seq.tryHead locationsAfterPos
                        | coveringLocations -> Seq.tryLast coveringLocations
                    | locationsOnSameLine -> Seq.tryHead locationsOnSameLine
                else
                    match locations |> List.filter (fun m -> rangeContainsPos m pos) with
                    | [] ->
                        match locations |> List.filter (fun m -> rangeBeforePos m pos |> not) with
                        | [] -> Seq.tryHead locations
                        | locationsAfterPos -> Seq.tryHead locationsAfterPos
                    | coveringLocations -> Seq.tryLast coveringLocations)
            (fun msg ->
                Trace.TraceInformation(sprintf "FCS: recovering from error in ValidateBreakpointLocationImpl: '%s'" msg)
                None)

    /// When these files appear or disappear the configuration for the current project is invalidated.
    member _.DependencyFiles = dependencyFiles

    member _.FileName = input.FileName

    // Get items for the navigation drop down bar
    member scope.GetNavigationItems() =
        // This does not need to be run on the background thread
        scope.GetNavigationItemsImpl()

    member scope.ValidateBreakpointLocation pos =
        // This does not need to be run on the background thread
        scope.ValidateBreakpointLocationImpl pos
