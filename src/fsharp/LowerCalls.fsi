// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerCalls

open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

/// Expands under-applied values of known arity to lambda expressions, and then reduce to bind
/// any known arguments. The results are later optimized by Optimizer.fs
val LowerImplFile: g: TcGlobals -> assembly: TypedImplFile -> TypedImplFile

