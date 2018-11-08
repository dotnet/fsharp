// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Implements a set of checks on the TAST for a file that can only be performed after type inference
/// is complete.
module internal Microsoft.FSharp.Compiler.PostTypeCheckSemanticChecks

open Microsoft.FSharp.Compiler.Import
open Microsoft.FSharp.Compiler.InfoReader
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.TcGlobals

val testFlagMemberBody : bool ref

/// Perform the checks on the TAST for a file after type inference is complete.
val CheckTopImpl : TcGlobals * ImportMap * bool * InfoReader * CompilationPath list * CcuThunk * DisplayEnv * ModuleOrNamespaceExprWithSig * Attribs * (bool * bool) * isInternalTestSpanStackReferring: bool -> bool * StampMap<AnonRecdTypeInfo>
