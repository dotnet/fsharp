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

/// It's unlikely you want to use this module except within
/// PostInferenceChecks. It's exposed to allow testing.
module Limit =
    [<System.Flags>]
    type LimitFlags =
        | None = 0b00000
        | ByRef = 0b00001
        | ByRefOfSpanLike = 0b00011
        | ByRefOfStackReferringSpanLike = 0b00101
        | SpanLike = 0b01000
        | StackReferringSpanLike = 0b10000

    /// A "limit" here is some combination of restrictions on a Val.
    [<Struct>]
    type Limit =
        {
            /// The scope of this Limit, i.e. "to which scope can a Val safely escape?".
            /// Some values are not allowed to escape their scope.
            /// For example, a top-level function is allowed to return a byref type, but inner functions are not.
            /// This `scope` field is the information that lets us track that.
            /// (Recall that in general scopes are counted starting from 0 indicating the top-level scope, and
            /// increasing by 1 essentially for every nested `let`-binding, method, or module.)
            ///
            /// Some specific values which are often used:
            /// * the value 0 is used in NoLimit and other situations which don't limit where the Val can escape;
            /// * the value 1 is a "top-level local scope", allowing us to express the restriction "this cannot appear
            ///   at the top level" (for example, `let x = &y` cannot appear at the top level).
            scope: int
            /// The combinations of limits which apply.
            flags: LimitFlags
        }

    /// Indicates that no limit applies to some Val. It can appear at the top level or within a `let`-binding,
    /// and the Val does not have any byref- or span-related restrictions.
    val NoLimit: Limit

    /// Construct a Limit which expresses "this Val must obey the first Limit and the second Limit simultaneously".
    //// If none of the limits are limited by a by-ref or a stack referring span-like, the scope will be 0.
    val CombineTwoLimits: Limit -> Limit -> Limit
