// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Optional static linking of all DLLs that depend on the F# Library, plus other specified DLLs
module internal FSharp.Compiler.StaticLinking

open Internal.Utilities.Library
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports

// Compute a static linker. This only captures tcImports (a large data structure) if
// static linking is enabled. Normally this is not the case, which lets us collect tcImports
// prior to this point.
val StaticLink:
    ctok: CompilationThreadToken * tcConfig: TcConfig * tcImports: TcImports * ilGlobals: ILGlobals ->
        (ILModuleDef -> ILModuleDef)
