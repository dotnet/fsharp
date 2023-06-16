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
val CheckImplFile:
    g: TcGlobals *
    amap: ImportMap *
    reportErrors: bool *
    infoReader: InfoReader *
    internalsVisibleToPaths: CompilationPath list *
    viewCcu: CcuThunk *
    tcValF: ConstraintSolver.TcValF *
    denv: DisplayEnv *
    implFileTy: ModuleOrNamespaceType *
    implFileContents: ModuleOrNamespaceContents *
    extraAttribs: Attribs *
    (bool * bool) *
    isInternalTestSpanStackReferring: bool ->
        bool * StampMap<AnonRecdTypeInfo>

module Limit =
    [<System.Flags>]
    type LimitFlags =
        | None = 0b00000
        | ByRef = 0b00001
        | ByRefOfSpanLike = 0b00011
        | ByRefOfStackReferringSpanLike = 0b00101
        | SpanLike = 0b01000
        | StackReferringSpanLike = 0b10000

    [<Struct>]
    type Limit = { scope: int; flags: LimitFlags }

    val NoLimit: Limit
    val CombineTwoLimits: Limit -> Limit -> Limit
