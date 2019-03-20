// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Implements a set of checks on the TAST for a file that can only be performed after type inference
/// is complete.
module internal FSharp.Compiler.PostTypeCheckSemanticChecks

open FSharp.Compiler.Import
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Tast
open FSharp.Compiler.Tastops
open FSharp.Compiler.TcGlobals

/// Perform the checks on the TAST for a file after type inference is complete.
val CheckTopImpl : TcGlobals * ImportMap * bool * InfoReader * CompilationPath list * CcuThunk * DisplayEnv * ModuleOrNamespaceExprWithSig * Attribs * (bool * bool) * isInternalTestSpanStackReferring: bool -> bool * StampMap<AnonRecdTypeInfo>
