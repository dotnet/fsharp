// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerAsyncSeq

open FSharp.Compiler.Import
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

type Lowering =
    | Evaluate of Expr
    | Sequence of ((ValRef * ValRef * ValRef * ValRef list * Expr * Expr * Expr * TType * range) * ValRef option)

val TryConvert: g: TcGlobals -> amap: ImportMap -> expr: Expr -> Lowering option
