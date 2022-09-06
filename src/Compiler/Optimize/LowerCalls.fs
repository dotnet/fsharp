// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerCalls

open Internal.Utilities.Library
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

let LowerCallsRewriteStackGuardDepth = StackGuard.GetDepthOption "LowerCallsRewrite"

//----------------------------------------------------------------------------
// Expansion of calls to methods with statically known arity

let InterceptExpr g cont expr =

    match expr with
    | Expr.Val (vref, flags, m) ->
        match vref.ValReprInfo with
        | Some arity -> Some (fst (AdjustValForExpectedValReprInfo g m vref flags arity))
        | None -> None

    // App (Val v, tys, args)
    | Expr.App (Expr.Val (vref, flags, _) as f0, f0ty, tyargsl, argsl, m) ->
        // Only transform if necessary, i.e. there are not enough arguments
        match vref.ValReprInfo with
        | Some(valReprInfo) ->
            let argsl = List.map cont argsl
            let f0 =
                if valReprInfo.AritiesOfArgs.Length > argsl.Length
                then fst(AdjustValForExpectedValReprInfo g m vref flags valReprInfo)
                else f0

            Some (MakeApplicationAndBetaReduce g (f0, f0ty, [tyargsl], argsl, m))
        | None -> None

    | Expr.App (f0, f0ty, tyargsl, argsl, m) ->
        Some (MakeApplicationAndBetaReduce g (f0, f0ty, [tyargsl], argsl, m) )

    | _ -> None

/// An "expr -> expr" pass that eta-expands under-applied values of
/// known arity to lambda expressions and beta-var-reduces to bind
/// any known arguments.  The results are later optimized by the peephole
/// optimizer in opt.fs
let LowerImplFile g assembly =
    let rwenv =
        { PreIntercept = Some(InterceptExpr g)
          PreInterceptBinding=None
          PostTransform= (fun _ -> None)
          RewriteQuotations=false
          StackGuard = StackGuard(LowerCallsRewriteStackGuardDepth) }
    assembly |> RewriteImplFile rwenv
