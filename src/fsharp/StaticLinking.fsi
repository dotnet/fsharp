// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Optional static linking of all DLLs that depend on the F# Library, plus other specified DLLs
module internal FSharp.Compiler.StaticLinking

open System

open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Lib
open FSharp.Compiler.OptimizeInputs
open FSharp.Compiler.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open Internal.Utilities
open Internal.Utilities.Collections

// Compute a static linker. This only captures tcImports (a large data structure) if
// static linking is enabled. Normally this is not the case, which lets us collect tcImports
// prior to this point.
val StaticLink: ctok: int * tcConfig: TcConfig * tcImports: TcImports * ilGlobals: ILGlobals -> (ILModuleDef -> ILModuleDef)
