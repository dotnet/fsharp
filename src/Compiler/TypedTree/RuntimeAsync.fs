// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsync

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

let (|RuntimeAsyncReturn|_|) (g: TcGlobals) (vref: ValRef) =
    valRefEq g vref g.cgh__runtimeAsyncReturn_vref
    || valRefEq g vref g.cgh__runtimeAsyncReturnValueTask_vref
    || valRefEq g vref g.cgh__runtimeAsyncReturnUnit_vref
    || valRefEq g vref g.cgh__runtimeAsyncReturnValueTaskUnit_vref

let IsRuntimeAsyncReturnUnitExpr (g: TcGlobals) expr =
    match stripExpr expr with
    | Expr.App(Expr.Val(RuntimeAsyncReturn g, _, _), _, [], [ _ ], _) -> true
    | _ -> false

let rec TryUnwrapRuntimeAsyncReturnExpr (g: TcGlobals) expr =
    match expr with
    | Expr.DebugPoint(_, innerExpr) ->
        match TryUnwrapRuntimeAsyncReturnExpr g innerExpr with
        | true, body -> true, body
        | false, _ -> false, expr
    | Expr.App(Expr.Val(RuntimeAsyncReturn g, _, _), _, _, [ body ], _) -> true, body
    | _ -> false, expr

let IsRuntimeAsyncSuspensionMethod (g: TcGlobals) (ilMethRef: ILMethodRef) =
    let (TILObjectReprData(coreLibScope, _, _)) = g.system_Object_tcref.ILTyconInfo

    ilMethRef.DeclaringTypeRef.Scope = coreLibScope
    && ilMethRef.DeclaringTypeRef.FullName = "System.Runtime.CompilerServices.AsyncHelpers"
    && ilMethRef.Name
       |> function
           | "Await"
           | "AwaitAwaiter"
           | "UnsafeAwaitAwaiter" -> true
           | _ -> false

let IsRuntimeAsyncSuspensionExpr (g: TcGlobals) expr =
    match stripExpr expr with
    | Expr.Op(TOp.ILCall(_, _, _, _, _, _, _, ilMethodRef, _, _, _), _, _, _) -> IsRuntimeAsyncSuspensionMethod g ilMethodRef
    | _ -> false

let ExprContainsRuntimeAsyncSuspension (g: TcGlobals) expr =
    let folder =
        { ExprFolder0 with
            exprIntercept =
                fun _ noInterceptF acc expr ->
                    if acc || IsRuntimeAsyncSuspensionExpr g expr then
                        true
                    else
                        noInterceptF acc expr
        }

    FoldExpr folder false expr
