// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsync

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

type RuntimeAsyncReturnInfo =
    {
        Value: ValRef
        Flags: ValUseFlag
        Body: Expr
        TypeArgs: TType list
    }

type RuntimeAsyncBoundary =
    | ReturnMarker of RuntimeAsyncReturnInfo
    | Suspension of ILMethodRef

let (|RuntimeAsyncReturn|_|) (g: TcGlobals) (vref: ValRef) =
    valRefEq g vref g.cgh__runtimeAsyncReturn_vref
    || valRefEq g vref g.cgh__runtimeAsyncReturnValueTask_vref
    || valRefEq g vref g.cgh__runtimeAsyncReturnUnit_vref
    || valRefEq g vref g.cgh__runtimeAsyncReturnValueTaskUnit_vref

let rec TryGetRuntimeAsyncReturn (g: TcGlobals) expr =
    match expr with
    | Expr.DebugPoint(_, innerExpr) -> TryGetRuntimeAsyncReturn g innerExpr
    | Expr.App(Expr.Val(RuntimeAsyncReturn g as value, flags, _), _, typeArgs, [ body ], _) ->
        Some
            {
                Value = value
                Flags = flags
                Body = body
                TypeArgs = typeArgs
            }
    | _ -> None

let (|RuntimeAsyncReturnFunction|_|) (g: TcGlobals) expr =
    match stripExpr expr with
    | Expr.Val(RuntimeAsyncReturn g as value, flags, m)
    | Expr.App(Expr.Val(RuntimeAsyncReturn g as value, flags, m), _, [ _ ], [], _) -> ValueSome(value, flags, m)
    | _ -> ValueNone

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

let TryGetRuntimeAsyncBoundary (g: TcGlobals) expr =
    match TryGetRuntimeAsyncReturn g expr with
    | Some info -> Some(RuntimeAsyncBoundary.ReturnMarker info)
    | None ->
        match stripExpr expr with
        | Expr.Op(TOp.ILCall(_, _, _, _, _, _, _, ilMethodRef, _, _, _), _, _, _) when IsRuntimeAsyncSuspensionMethod g ilMethodRef ->
            Some(RuntimeAsyncBoundary.Suspension ilMethodRef)
        | _ -> None
