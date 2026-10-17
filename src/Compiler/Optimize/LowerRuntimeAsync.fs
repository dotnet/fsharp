// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerRuntimeAsync

open System.Collections.Concurrent
open System.Collections.Generic

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

open FSharp.Compiler
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.RuntimeAsync
open FSharp.Compiler.RuntimeAsyncAnalysis
open FSharp.Compiler.RuntimeAsyncExceptionRewrite
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeRelations

let private isQuotationUsing (v: Val) expr =
    match expr with
    | Expr.Quote(body, _, _, _, _) ->
        ExistsExpr
            (function
            | Expr.Val(vref, _, _) -> valEq v vref.Deref
            | _ -> false)
            body
    | _ -> false

let private tryCallbackInvocation (g: TcGlobals) (callback: Val) isDelegate expression =
    match expression with
    | Expr.App(Expr.Val(vref, _, _), _, [], [ arg ], m) when not isDelegate && valEq callback vref.Deref -> ValueSome(struct (arg, m))
    | DelegateInvokeExpr g (_, _, _, Expr.Val(vref, _, _), arg, m) when isDelegate && valEq callback vref.Deref ->
        ValueSome(struct (arg, m))
    | _ -> ValueNone

let private tryAnalyzeCallbackUses g callback isDelegate continuation =
    let folder =
        { ExprFolder0 with
            exprIntercept =
                fun recurse noIntercept uses expression ->
                    match uses with
                    | ValueNone -> ValueNone
                    | ValueSome count ->
                        match tryCallbackInvocation g callback isDelegate expression with
                        | ValueSome(struct (arg, _)) -> recurse (ValueSome(count + 1)) arg
                        | ValueNone ->
                            match expression with
                            | Expr.Val(vref, _, _) when valEq callback vref.Deref -> ValueNone
                            | Expr.Quote(_, dataCell, _, _, _) ->
                                if isQuotationUsing callback expression then
                                    ValueNone
                                else
                                    match dataCell.Value with
                                    | Some((_, _, args, _), (_, _, legacyArgs, _)) ->
                                        List.fold recurse (List.fold recurse uses args) legacyArgs
                                    | None -> uses
                            | _ -> noIntercept uses expression
        }

    FoldExpr folder (ValueSome 0) continuation

type private CallbackBranch =
    {
        Parameter: Val
        Body: Expr
        Captures: Val list
        NodeCount: int
    }

/// A callback construction whose branches select one of several lambdas. The construction runs once
/// and records the selected branch and the values it captures; each invocation dispatches on the branch.
type private CallbackBranches =
    {
        Construction: Expr
        Tag: Val
        Branches: CallbackBranch list
        Captures: (Val * Val) list
    }

/// A captured mutable keeps a single shared home: its binding becomes an assignment to the hoisted local and
/// every other use is redirected to it, so writes made by the inlined callback stay visible to other closures.
/// Returns None if a captured mutable is not bound by a let in the construction.
let private tryShareMutableCallbackCaptures (g: TcGlobals) (mutableCaptures: (Val * Val) list) construction =
    if mutableCaptures.IsEmpty then
        Some construction
    else
        let hoistedOf =
            ValMap.OfList(mutableCaptures |> List.map (fun (v, hoisted) -> v, mkLocalValRef hoisted))

        let rewritten = HashSet<Stamp>()

        let rwenv =
            {
                PreIntercept =
                    Some(fun rewrite expression ->
                        match expression with
                        | Expr.Let(TBind(v, rhs, point), body, m, _) when hoistedOf.ContainsVal v ->
                            rewritten.Add v.Stamp |> ignore
                            let assignment = mkValSet m hoistedOf[v] (rewrite rhs)

                            let assignment =
                                match point with
                                | DebugPointAtBinding.Yes point -> mkDebugPoint point assignment
                                | _ -> assignment

                            let body = rewrite body

                            let body =
                                if v.IsCompilerGenerated then
                                    body
                                else
                                    Expr.Op(TOp.DebugLocalScope(hoistedOf[v], v.LogicalName), [], [ body ], m)

                            Some(mkCompGenSequential m assignment body)
                        | _ -> None)
                PreInterceptBinding = None
                PostTransform = (fun _ -> None)
                RewriteQuotations = false
                StackGuard = StackGuard("RewriteRuntimeAsyncMutableCaptures")
            }

        let construction = RewriteExpr rwenv construction

        if rewritten.Count = mutableCaptures.Length then
            Some(remapExpr g CloneAll { emptyRemap with valRemap = hoistedOf } construction)
        else
            None

let private exprNodeCount expr =
    let folder =
        { ExprFolder0 with
            exprIntercept = fun _ noInterceptF count expr -> noInterceptF (count + 1) expr
        }

    FoldExpr folder 0 expr

/// Replaces each lambda in tail position of a callback construction with an assignment of its branch tag
/// and captured locals, or returns None if a tail is not a supported single-argument lambda.
let private tryDefunctionalizeCallback (g: TcGlobals) (stackGuard: StackGuard) m construction =
    let freeVarOptions = CollectLocalsWithStackGuard()
    let constructionFree = (freeInExpr freeVarOptions construction).FreeLocals
    let tag, _ = mkMutableCompGenLocal m "runtimeAsyncCallbackTag" g.int32_ty
    let branches = ResizeArray<CallbackBranch>()
    let captures = Dictionary<Stamp, Val * Val>()

    let rec defunctionalize expr =
        stackGuard.Guard(fun () -> defunctionalizeCore expr)

    and defunctionalizeCore expr =
        match expr with
        | Expr.Lambda(_, None, None, [ parameter ], body, m, _)
        | NewDelegateExpr g (_, [ parameter ], body, m, _) ->
            let captured =
                (freeInExpr freeVarOptions body).FreeLocals
                |> Zset.elements
                |> List.filter (fun v -> not (valEq v parameter) && not (Zset.contains v constructionFree))

            if
                captured
                |> List.exists (fun v -> v.IsPinning || isByrefTy g v.Type || isByrefLikeTy g m v.Type)
            then
                None
            else
                let assignments =
                    captured
                    |> List.map (fun v ->
                        let _, hoisted =
                            match captures.TryGetValue v.Stamp with
                            | true, capture -> capture
                            | _ ->
                                let hoisted =
                                    if v.IsMutable then
                                        let hoisted = Construct.NewModifiedVal id v
                                        hoisted.SetIsCompilerGenerated true
                                        hoisted
                                    else
                                        fst (mkMutableCompGenLocal m v.LogicalName v.Type)

                                let capture = v, hoisted
                                captures[v.Stamp] <- capture
                                capture

                        if v.IsMutable then
                            None
                        else
                            Some(mkValSet m (mkLocalValRef hoisted) (exprForVal m v)))
                    |> List.choose id

                let select = mkValSet m (mkLocalValRef tag) (mkInt32 g m branches.Count)

                branches.Add
                    {
                        Parameter = parameter
                        Body = body
                        Captures = captured
                        NodeCount = exprNodeCount body
                    }

                Some(List.foldBack (mkCompGenSequential m) assignments select)
        | RuntimeAsyncConstructionPrefix(body, rebuild) -> defunctionalize body |> Option.map rebuild
        | Expr.Match(point, matchRange, tree, targets, m, _) ->
            TryMapRuntimeAsyncMatchTargets g (point, matchRange, tree, targets, m) (fun _ -> defunctionalize)
        | _ -> None

    defunctionalize construction
    |> Option.bind (fun construction ->
        let captures = List.ofSeq captures.Values

        tryShareMutableCallbackCaptures g (captures |> List.filter (fun (v, _) -> v.IsMutable)) construction
        |> Option.map (fun construction ->
            {
                Construction = construction
                Tag = tag
                Branches = List.ofSeq branches
                Captures = captures
            }))

/// Every invocation copies every branch body, so the rewrite is skipped when the copies would exceed this many
/// expression nodes; the callback then stays a closure and a suspension left in it is reported as FS3918.
let private maxInlinedCallbackCopySize = 2000

/// Inlines an invocation of a defunctionalized callback: the argument is bound once, then each branch body
/// is copied with its parameter and captured locals remapped.
let private mkCallbackDispatch (g: TcGlobals) (stackGuard: StackGuard) (callback: CallbackBranches) arg resultTy m =
    let argVal, _ = mkCompGenLocal m "runtimeAsyncCallbackArg" (tyOfExpr g arg)

    let captureRemap =
        callback.Captures
        |> List.map (fun (v, hoisted) -> v, mkLocalValRef hoisted)
        |> ValMap.OfList

    let branch branch =
        let remap =
            { emptyRemap with
                valRemap = captureRemap.Add branch.Parameter (mkLocalValRef argVal)
            }

        let body = remapExpr g CloneAll remap branch.Body

        (branch.Captures, body)
        ||> List.foldBack (fun v body ->
            if v.IsMutable && not v.IsCompilerGenerated then
                Expr.Op(TOp.DebugLocalScope(captureRemap[v], v.LogicalName), [], [ body ], m)
            else
                body)

    let rec dispatch i branches =
        stackGuard.Guard(fun () -> dispatchCore i branches)

    and dispatchCore i branches =
        match branches with
        | [ last ] -> branch last
        | first :: rest ->
            let isSelected = mkILAsmCeq g m (exprForVal m callback.Tag) (mkInt32 g m i)

            mkCond DebugPointAtBinding.NoneAtInvisible m resultTy isSelected (branch first) (dispatch (i + 1) rest)
        | [] -> failwith "unreachable: callback without branches"

    mkCompGenLet m argVal arg (dispatch 0 callback.Branches)

type private CallbackRewrite =
    | KeepCallback
    | InlineDelegate of (Expr * TType * TType list * Expr * range -> Expr)
    | DispatchBranches of isDelegate: bool * branches: CallbackBranches

let private inlineCallback (g: TcGlobals) runtimeAsyncContext (expr: Expr) =
    let stackGuard = StackGuard("InlineRuntimeAsyncCallback")
    let freeVarOptions = CollectLocalsWithStackGuard()

    let rec tryDelegateSignature expr =
        match expr with
        | NewDelegateExpr g (_, [ parameter ], body, _, _) -> Some(parameter.Type, tyOfExpr g body)
        | RuntimeAsyncDebugWrapper rest
        | Expr.Sequential(_, rest, NormalSeq, _)
        | Expr.Let(_, rest, _, _) -> tryDelegateSignature rest
        | Expr.Match(_, _, _, targets, _, _) when targets.Length > 0 ->
            let (TTarget(_, body, _)) = targets[0]
            tryDelegateSignature body
        | _ -> None

    let isTrivialValue =
        function
        | Expr.Const _ -> true
        | Expr.Val(vref, _, _) -> vref.IsLocalRef && not vref.IsMutable && not vref.IsTypeFunction
        | _ -> false

    // Construction moves to the invocation, so its evaluation must not have observable effects.
    let rec tryPrepareDelegateInvocation construction =
        stackGuard.Guard(fun () ->
            let prepared =
                match construction with
                | NewDelegateExpr g (_, [ _ ], _, _, _) ->
                    ValueSome(fun (invokeRef, invokeTy, tyargs, arg, callRange) ->
                        MakeFSharpDelegateInvokeAndTryBetaReduce g (invokeRef, construction, invokeTy, tyargs, arg, callRange))
                | RuntimeAsyncConstructionPrefix(body, rebuild) when
                    match construction with
                    | Expr.Sequential(first, _, _, _) -> isTrivialValue first
                    | Expr.Let(TBind(_, rhs, _), _, _, _) -> isTrivialValue rhs
                    | _ -> true
                    ->
                    tryPrepareDelegateInvocation body
                    |> ValueOption.map (fun invoke -> invoke >> rebuild)
                | Expr.Match(point,
                             matchRange,
                             (TDSwitch((Expr.Val _ as input), [ TCase(_, TDSuccess([], _)) ], Some(TDSuccess([], _)), _) as tree),
                             targets,
                             m,
                             _) when isTrivialValue input ->
                    let invocations =
                        targets
                        |> Array.map (fun (TTarget(_, body, _)) -> tryPrepareDelegateInvocation body)

                    if targets.Length > 0 && Array.forall ValueOption.isSome invocations then
                        let invocations = Array.map ValueOption.get invocations

                        ValueSome(fun args ->
                            let targets =
                                (targets, invocations)
                                ||> Array.map2 (fun (TTarget(vals, _, flags)) invoke -> TTarget(vals, invoke args, flags))

                            Expr.Match(point, matchRange, tree, targets, m, tyOfExpr g targets[0].TargetExpression))
                    else
                        ValueNone
                | _ -> ValueNone

            prepared
            |> ValueOption.map (fun invoke -> fun args -> stackGuard.Guard(fun () -> invoke args)))

    // A residual Invoke can surface after ordinary optimization; try its single use before dispatching on branches.
    let inlineDelegate callback invoke continuation =
        let rwenv =
            {
                PreIntercept =
                    Some(fun rewrite expression ->
                        match expression with
                        | DelegateInvokeExpr g (invokeRef, invokeTy, tyargs, Expr.Val(vref, _, _), arg, callRange) when
                            valEq callback vref.Deref
                            ->
                            Some(invoke (invokeRef, invokeTy, tyargs, rewrite arg, callRange))
                        | _ -> None)
                PreInterceptBinding = None
                PostTransform = (fun _ -> None)
                RewriteQuotations = false
                StackGuard = stackGuard
            }

        RewriteExpr rwenv continuation

    let chooseRewrite (callback: Val) construction continuation m =
        let canInline =
            runtimeAsyncContext || (TryGetRuntimeAsyncReturn g continuation).IsSome

        let shape =
            match tryDestFunTy g callback.Type with
            | ValueSome(argTy, resultTy) -> Some(argTy, resultTy, false)
            | ValueNone when isFSharpDelegateTy g callback.Type ->
                tryDelegateSignature construction
                |> Option.map (fun (argTy, resultTy) -> argTy, resultTy, true)
            | _ -> None

        match shape with
        | Some(argTy, resultTy, isDelegate) when canInline && not (isByrefLikeTy g m argTy || isByrefLikeTy g m resultTy) ->
            let freeVals = (freeInExpr freeVarOptions construction).FreeLocals

            let canCapture =
                freeVals
                |> Zset.forall (fun v -> not v.IsPinning && not (isByrefTy g v.Type || isByrefLikeTy g m v.Type))

            let callbackUses =
                if canCapture then
                    tryAnalyzeCallbackUses g callback isDelegate continuation
                else
                    ValueNone

            let delegateInvocation =
                match callbackUses with
                | ValueSome 1 when isDelegate -> tryPrepareDelegateInvocation construction
                | _ -> ValueNone

            match delegateInvocation, callbackUses with
            | ValueSome invoke, _ -> InlineDelegate invoke
            | ValueNone, ValueSome invocations ->
                tryDefunctionalizeCallback g stackGuard m construction
                |> Option.filter (fun branches ->
                    invocations = 0
                    || List.sumBy _.NodeCount branches.Branches
                       <= maxInlinedCallbackCopySize / invocations)
                |> Option.map (fun branches -> DispatchBranches(isDelegate, branches))
                |> Option.defaultValue KeepCallback
            | ValueNone, ValueNone -> KeepCallback
        | _ -> KeepCallback

    match expr with
    | Expr.Let(TBind(callback, construction, point), continuation, m, _) ->
        match chooseRewrite callback construction continuation m with
        | KeepCallback -> None
        | InlineDelegate invoke -> Some(inlineDelegate callback invoke continuation)
        | DispatchBranches(isDelegate, callbackBranches) ->
            let rwenv =
                {
                    PreIntercept =
                        Some(fun rewrite expression ->
                            match tryCallbackInvocation g callback isDelegate expression with
                            | ValueSome(struct (arg, callRange)) ->
                                Some(mkCallbackDispatch g stackGuard callbackBranches (rewrite arg) (tyOfExpr g expression) callRange)
                            | ValueNone -> None)
                    PreInterceptBinding = None
                    PostTransform = (fun _ -> None)
                    RewriteQuotations = false
                    StackGuard = stackGuard
                }

            let continuation = RewriteExpr rwenv continuation

            let construction =
                match point with
                | DebugPointAtBinding.Yes point -> mkDebugPoint point callbackBranches.Construction
                | _ -> callbackBranches.Construction

            let body = mkCompGenSequential m construction continuation

            Some(
                (callbackBranches.Tag :: List.map snd callbackBranches.Captures, body)
                ||> List.foldBack (fun v body -> mkCompGenLet m v (mkDefault (m, v.Type)) body)
            )
    | _ -> None

let private isRuntimeAsyncEntry g expr =
    (TryGetRuntimeAsyncReturn g expr).IsSome
    || (TryGetRuntimeAsyncSequence g expr).IsSome

let private containsRuntimeAsyncEntry g implFile =
    let folder =
        { ExprFolder0 with
            exprIntercept =
                fun _ noInterceptF acc expr ->
                    if acc || isRuntimeAsyncEntry g expr then
                        true
                    else
                        noInterceptF acc expr
        }

    FoldImplFile folder false implFile

/// Inlines suspending InlineIfLambda callbacks into the enclosing method. `inContext` is true inside a runtime-async body or
/// sequence recipe, including nested lambdas and delegates, but not object-expression methods, which are
/// compiled as ordinary methods.
let private inlineCallbacks g implFile =
    let analyzer = RuntimeAsyncAnalyzer g
    let stackGuard = StackGuard("InlineRuntimeAsyncCallbacks")

    let rec leadsToRuntimeAsyncReturn expr =
        match expr with
        | Expr.Let(_, rest, _, _)
        | RuntimeAsyncDebugWrapper rest
        | Expr.Sequential(_, rest, NormalSeq, _) -> leadsToRuntimeAsyncReturn rest
        | _ -> (TryGetRuntimeAsyncReturn g expr).IsSome

    let rec rewriter inContext =
        {
            PreIntercept = Some(intercept inContext)
            PreInterceptBinding = None
            PostTransform = postTransform inContext
            RewriteQuotations = false
            StackGuard = stackGuard
        }

    and rewrite inContext expr = RewriteExpr (rewriter inContext) expr

    and intercept inContext _ expr =
        match expr with
        | Expr.App(f, fty, tyargs, [ body ], m) when (TryGetRuntimeAsyncReturn g expr).IsSome ->
            Some(Expr.App(f, fty, tyargs, [ rewrite true body ], m))
        | Expr.App(f, fty, tyargs, [ recipe ], m) when (TryGetRuntimeAsyncSequence g expr).IsSome ->
            Some(Expr.App(f, fty, tyargs, [ rewrite true recipe ], m))
        // A callback constructed for a runtime-async body that follows is lowered as part of that body,
        // so the suspending callbacks it captures can be inlined into it.
        | Expr.Let(TBind(callback, construction, point), continuation, m, _) when
            not inContext
            && callback.InlineIfLambda
            && leadsToRuntimeAsyncReturn continuation
            ->
            Some(mkLetBind m (TBind(callback, rewrite true construction, point)) (rewrite false continuation))
        | DelegateInvokeExpr g (invokeRef, invokeTy, tyargs, receiver, arg, m) when inContext && analyzer.ContainsSuspension receiver ->
            let callback, callbackExpr =
                mkCompGenLocal m "runtimeAsyncDelegate" (tyOfExpr g receiver)

            callback.SetInlineIfLambda()
            let invoke = Expr.App(invokeRef, invokeTy, tyargs, [ callbackExpr; arg ], m)
            Some(rewrite inContext (mkCompGenLet m callback receiver invoke))
        | NewDelegateExpr g _ -> None
        | Expr.Obj _ when inContext -> Some(rewrite false expr)
        | _ -> None

    // Substitution can make an enclosing callback eligible, so revisit successful replacements.
    and postTransform inContext expr =
        match expr with
        | Expr.Let(TBind(callback, construction, _), _, _, _) when callback.InlineIfLambda && analyzer.ContainsSuspension construction ->
            inlineCallback g inContext expr |> Option.map (rewrite inContext)
        | _ -> None

    RewriteImplFile (rewriter false) implFile

/// Prepares each runtime-async body once its final shape is known, innermost first.
let private prepareBodies g (reportedRanges: ConcurrentDictionary<range, unit>) implFile =
    let prepare body =
        for v in GetRuntimeAsyncNonPreservableUses g body do
            if reportedRanges.TryAdd(v.Range, ()) then
                errorR (Error(FSComp.SR.ilRuntimeAsyncLocalUsedAfterSuspension (RichText.mkText v.DisplayName), v.Range))

        RewriteRuntimeAsyncExceptionHandlers g body

    RewriteImplFile
        {
            PreIntercept = None
            PreInterceptBinding = None
            PostTransform =
                fun expr ->
                    match expr with
                    | Expr.App(f, fty, tyargs, [ body ], m) when (TryGetRuntimeAsyncReturn g expr).IsSome ->
                        Some(Expr.App(f, fty, tyargs, [ prepare body ], m))
                    | _ -> None
            RewriteQuotations = false
            StackGuard = StackGuard("PrepareRuntimeAsyncBodies")
        }
        implFile

let TransformImplFile (g: TcGlobals) reportedRanges (implFile: CheckedImplFile) =
    if containsRuntimeAsyncEntry g implFile then
        // Bodies inlined from an assembly compiled with the feature are still prepared.
        let implFile =
            if g.langVersion.SupportsFeature LanguageFeature.RuntimeAsync then
                inlineCallbacks g implFile
            else
                implFile

        prepareBodies g reportedRanges implFile
    else
        implFile
