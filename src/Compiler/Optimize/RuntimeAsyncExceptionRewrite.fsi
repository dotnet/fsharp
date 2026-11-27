// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsyncExceptionRewrite

open FSharp.Compiler.Import
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

/// ExceptionDispatchInfo.Capture(exn).Throw(), so a pending exception keeps the stack trace of its original throw.
val RethrowRuntimeAsyncException: g: TcGlobals -> amap: ImportMap -> m: range -> exnExpr: Expr -> Expr

/// Moves suspending `try/with` handlers and filters, and suspending `try/finally` compensations, out of their
/// exception-handling region. The body's outcome is recorded in locals without allocating, and a pending
/// exception is rethrown with its original dispatch information.
val RewriteRuntimeAsyncExceptionHandlers: g: TcGlobals -> amap: ImportMap -> expr: Expr -> Expr
