// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Caching of overload resolution results.
///
/// Two calls share an entry only when overload resolution is given the same question: the same candidates
/// called the same way, and the same types. Whatever else resolution reads must either follow from those or
/// keep the call out of the cache.
module internal FSharp.Compiler.OverloadResolutionCache

open Internal.Utilities.TypeHashing.StructuralUtilities

open FSharp.Compiler.Caches
open FSharp.Compiler.InfoReader
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

/// The expected type of a call, as far as it takes part in choosing the overload
[<Struct; RequireQualifiedAccess>]
type ExpectedReturnType =
    /// No candidate returns out arguments as a tuple, so the return type is not looked at
    | NotConsidered
    | Unknown
    | MustEqual of equalTy: TType
    | MustConvertTo of isMethodArg: bool * convertibleTy: TType

/// The candidates of a call and its types, written so that different calls never get equal keys
[<Struct>]
type OverloadResolutionCacheKey = OverloadResolutionCacheKey of callStructure: TypeStructure

type OverloadResolutionCacheResult =
    { WinnerIndex: int
      IsExactMatch: bool
      LessConcreteLoserIndexes: int list }

/// Gets a per-TcGlobals overload resolution cache.
/// Uses WeakMap to tie cache lifetime to TcGlobals (per-compilation isolation).
val getOverloadResolutionCache: (TcGlobals -> Cache<OverloadResolutionCacheKey, OverloadResolutionCacheResult>)

/// Returns ValueNone for a call that cannot be cached: one with named arguments, with a type that is not
/// fully inferred yet, or with a candidate whose signature or applicability can change.
val tryComputeOverloadCacheKey:
    g: TcGlobals ->
    infoReader: InfoReader ->
    candidates: CalledMeth<'T> list ->
    explicitTyArgs: TType list ->
    callerArgs: CallerArgs<'T> ->
    expectedReturnType: ExpectedReturnType ->
        OverloadResolutionCacheKey voption
