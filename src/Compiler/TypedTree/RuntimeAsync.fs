// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsync

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

type RuntimeAsyncReturnInfo = { Body: Expr; TypeArgs: TType list }

let (|RuntimeAsyncDebugWrapper|_|) expr =
    match expr with
    | Expr.DebugPoint(_, body)
    | Expr.Op(TOp.DebugLocalScope _, [], [ body ], _) -> ValueSome body
    | _ -> ValueNone

let RebuildRuntimeAsyncDebugWrapper wrapper body =
    match wrapper with
    | Expr.DebugPoint(point, _) -> Expr.DebugPoint(point, body)
    | Expr.Op(TOp.DebugLocalScope _ as op, [], [ _ ], m) -> Expr.Op(op, [], [ body ], m)
    | _ -> failwith "unreachable: expression is not a runtime-async debug wrapper"

[<return: Struct>]
let (|RuntimeAsyncConstructionPrefix|_|) expr =
    match expr with
    | RuntimeAsyncDebugWrapper body -> ValueSome(body, RebuildRuntimeAsyncDebugWrapper expr)
    | Expr.Let(binding, body, m, _) -> ValueSome(body, mkLetBind m binding)
    | Expr.Sequential(first, body, NormalSeq, m) -> ValueSome(body, fun body -> Expr.Sequential(first, body, NormalSeq, m))
    | _ -> ValueNone

let TryMapRuntimeAsyncMatchTargets (g: TcGlobals) matchInfo mapTarget =
    let point, matchRange, tree, (targets: DecisionTreeTarget array), m = matchInfo

    let targets =
        targets
        |> Array.mapi (fun i (TTarget(vals, body, flags)) -> mapTarget i body |> Option.map (fun body -> TTarget(vals, body, flags)))

    if targets.Length > 0 && Array.forall Option.isSome targets then
        let targets = Array.map Option.get targets
        Some(Expr.Match(point, matchRange, tree, targets, m, tyOfExpr g targets[0].TargetExpression))
    else
        None

let (|RuntimeAsyncReturn|_|) (g: TcGlobals) (vref: ValRef) =
    valRefEq g vref g.cgh__runtimeAsyncReturn_vref
    || valRefEq g vref g.cgh__runtimeAsyncReturnValueTask_vref
    || valRefEq g vref g.cgh__runtimeAsyncReturnUnit_vref
    || valRefEq g vref g.cgh__runtimeAsyncReturnValueTaskUnit_vref

let rec TryGetRuntimeAsyncReturn (g: TcGlobals) expr =
    match expr with
    | Expr.DebugPoint(_, innerExpr) -> TryGetRuntimeAsyncReturn g innerExpr
    | Expr.App(Expr.Val(RuntimeAsyncReturn g, _, _), _, typeArgs, [ body ], _) -> Some { Body = body; TypeArgs = typeArgs }
    | _ -> None

let (|RuntimeAsyncReturnFunction|_|) (g: TcGlobals) expr =
    match stripExpr expr with
    | Expr.Val(RuntimeAsyncReturn g as value, flags, m)
    | Expr.App(Expr.Val(RuntimeAsyncReturn g as value, flags, m), _, [ _ ], [], _) -> ValueSome(value, flags, m)
    | _ -> ValueNone

let TryGetRuntimeAsyncSequence (g: TcGlobals) expr =
    match expr with
    | ValApp g g.cgh__runtimeAsyncSequence_vref ([ elementTy ], [ recipe ], _) -> Some(recipe, elementTy)
    | _ -> None

let (|RuntimeAsyncSequenceFunction|_|) (g: TcGlobals) expr =
    match stripExpr expr with
    | Expr.Val(value, _, _)
    | Expr.App(Expr.Val(value, _, _), _, [ _ ], [], _) when valRefEq g value g.cgh__runtimeAsyncSequence_vref -> ValueSome value
    | _ -> ValueNone

let private runtimeAsyncHelpersTypeName =
    "System.Runtime.CompilerServices.AsyncHelpers"

let private runtimeAsyncSuspensionMethodNames =
    [ "Await"; "AwaitAwaiter"; "UnsafeAwaitAwaiter" ]

let IsRuntimeAsyncSuspensionMethod (g: TcGlobals) (ilMethRef: ILMethodRef) =
    let (TILObjectReprData(coreLibScope, _, _)) = g.system_Object_tcref.ILTyconInfo

    ilMethRef.DeclaringTypeRef.Scope = coreLibScope
    && ilMethRef.DeclaringTypeRef.FullName = runtimeAsyncHelpersTypeName
    && List.contains ilMethRef.Name runtimeAsyncSuspensionMethodNames

let IsRuntimeAsyncSuspensionExpr (g: TcGlobals) expr =
    match stripExpr expr with
    | Expr.Op(TOp.ILCall(_, _, _, _, _, _, _, ilMethodRef, _, _, _), _, _, _) -> IsRuntimeAsyncSuspensionMethod g ilMethodRef
    | _ -> false

let IsRuntimeAsyncBoundary (g: TcGlobals) expr =
    (TryGetRuntimeAsyncReturn g expr).IsSome || IsRuntimeAsyncSuspensionExpr g expr

/// Returns true when any sub-expression matches the predicate, short-circuiting at the first match.
let ExistsExpr (predicate: Expr -> bool) expr =
    let folder =
        { ExprFolder0 with
            exprIntercept =
                fun _ noInterceptF acc expr ->
                    if acc then true
                    elif predicate expr then true
                    else noInterceptF acc expr
        }

    FoldExpr folder false expr
