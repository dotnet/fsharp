// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsyncExceptionRewrite

open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

val RewriteRuntimeAsyncExceptionHandlers: g: TcGlobals -> expr: Expr -> Expr
