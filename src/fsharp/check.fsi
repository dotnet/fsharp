// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.PostTypecheckSemanticChecks

open Internal.Utilities
open Microsoft.FSharp.Compiler

val testFlagMemberBody : bool ref
val CheckTopImpl : Env.TcGlobals * Import.ImportMap * bool * Infos.InfoReader * Tast.CompilationPath list * Tast.CcuThunk * Tastops.DisplayEnv * Tast.ModuleOrNamespaceExprWithSig * Tast.Attribs * bool -> bool
