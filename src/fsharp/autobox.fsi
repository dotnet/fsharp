// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.AutoBox

open FSharp.Compiler.Import
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

/// Rewrite mutable locals to reference cells across an entire implementation file
val TransformImplFile: g: TcGlobals -> amap: ImportMap -> implFile: TypedImplFile -> TypedImplFile
