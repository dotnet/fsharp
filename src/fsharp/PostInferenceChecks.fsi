// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Implements a set of checks on the TAST for a file that can only be performed after type inference
/// is complete.
module internal FSharp.Compiler.PostTypeCheckSemanticChecks

open FSharp.Compiler.Import
open FSharp.Compiler.InfoReader
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals

/// Perform the checks on the TAST for a file after type inference is complete.
val CheckTopImpl:
    g: TcGlobals *
    amap: ImportMap *
    reportErrors: bool *
    infoReader: InfoReader *
    internalsVisibleToPaths: CompilationPath list *
    viewCcu: CcuThunk *
    tcValF: ConstraintSolver.TcValF *
    denv: DisplayEnv *
    mexpr: ModuleOrNamespaceExprWithSig *
    extraAttribs: Attribs *
    (bool * bool) *
    isInternalTestSpanStackReferring: bool ->
        bool * StampMap<AnonRecdTypeInfo>
