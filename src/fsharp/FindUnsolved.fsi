// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.


module internal FSharp.Compiler.FindUnsolved

open FSharp.Compiler.Tast
open FSharp.Compiler.Tastops
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Import

/// Find all unsolved inference variables after type inference for an entire file
val UnsolvedTyparsOfModuleDef: g: TcGlobals -> amap: ImportMap -> denv: DisplayEnv -> mdef : ModuleOrNamespaceExpr * extraAttribs: Attrib list -> Typar list
