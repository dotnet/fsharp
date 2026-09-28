// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerRuntimeAsync

open System.Collections.Concurrent

open Internal.Utilities.Library
open FSharp.Compiler
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.RuntimeAsync
open FSharp.Compiler.RuntimeAsyncAnalysis
open FSharp.Compiler.RuntimeAsyncExceptionRewrite
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

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

/// Outlines suspending InlineIfLambda callbacks. `inContext` is true inside a runtime-async body or
/// sequence recipe, including nested lambdas and delegates, but not object-expression methods, which are
/// compiled as ordinary methods.
let private outlineCallbacks g implFile =
    let analyzer = RuntimeAsyncAnalyzer g
    let stackGuard = StackGuard("OutlineRuntimeAsyncCallbacks")

    let rec leadsToRuntimeAsyncReturn expr =
        match expr with
        | Expr.Let(_, rest, _, _)
        | Expr.DebugPoint(_, rest)
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
        // so the suspending callbacks it captures can be inlined or outlined into it.
        | Expr.Let(TBind(callback, construction, point), continuation, m, _) when
            not inContext
            && callback.InlineIfLambda
            && leadsToRuntimeAsyncReturn continuation
            ->
            Some(mkLetBind m (TBind(callback, rewrite true construction, point)) (rewrite false continuation))
        | NewDelegateExpr g _ -> None
        | Expr.Obj _ when inContext -> Some(rewrite false expr)
        | _ -> None

    // Bottom-up: callbacks nested in the construction or continuation are already outlined.
    and postTransform inContext expr =
        match expr with
        | Expr.Let(TBind(callback, construction, _), _, _, _) when callback.InlineIfLambda && analyzer.ContainsSuspension construction ->
            Some(OutlineRuntimeAsyncCallback g analyzer inContext expr)
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
                outlineCallbacks g implFile
            else
                implFile

        prepareBodies g reportedRanges implFile
    else
        implFile
