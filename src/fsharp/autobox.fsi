// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.AutoBox 

open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Import

/// Rewrite mutable locals to reference cells across an entire implementation file
val TransformImplFile: g: TcGlobals -> amap: ImportMap -> implFile: TypedImplFile -> TypedImplFile


