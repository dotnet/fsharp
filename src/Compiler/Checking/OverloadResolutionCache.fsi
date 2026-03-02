// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Caching infrastructure for overload resolution results
module internal FSharp.Compiler.OverloadResolutionCache

open Internal.Utilities.TypeHashing.StructuralUtilities

open FSharp.Compiler.Caches
open FSharp.Compiler.Infos
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

/// Cache key for overload resolution: combines method group identity with caller argument types and return type
type OverloadResolutionCacheKey =
    {
        /// Hash combining all method identities in the method group
        MethodGroupHash: int
        /// Type structures for caller object arguments (the 'this' argument for instance/extension methods)
        /// This is critical for extension methods where the 'this' type determines the overload
        ObjArgTypeStructures: TypeStructure[]
        /// Type structures for each caller argument (only used when all types are stable)
        ArgTypeStructures: TypeStructure[]
        /// Type structure for expected return type (if any), to differentiate calls with different expected types
        ReturnTypeStructure: TypeStructure voption
        /// Number of caller-provided type arguments (to distinguish calls with different type instantiations)
        CallerTyArgCount: int
    }

/// Result of cached overload resolution
[<Struct>]
type OverloadResolutionCacheResult =
    /// Resolution succeeded - index of the resolved method in the original calledMethGroup list
    | CachedResolved of methodIndex: int

/// Gets a per-TcGlobals overload resolution cache.
/// Uses WeakMap to tie cache lifetime to TcGlobals (per-compilation isolation).
val getOverloadResolutionCache: (TcGlobals -> Cache<OverloadResolutionCacheKey, OverloadResolutionCacheResult>)

/// Compute a hash for a method info for caching purposes
val computeMethInfoHash: MethInfo -> int

/// Try to get a type structure for caching in the overload resolution context.
///
/// In this context, we accept Unstable structures that are unstable ONLY because
/// of solved typars (not unsolved flexible typars). This is safe because:
/// 1. The cache key is computed BEFORE FilterEachThenUndo runs
/// 2. Caller argument types were resolved before overload resolution
/// 3. Solved typars in those types won't be reverted by Trace.Undo
///
/// We reject structures containing Unsolved tokens because unsolved flexible typars
/// could resolve to different types in different contexts, leading to wrong cache hits.
val tryGetTypeStructureForOverloadCache: g: TcGlobals -> ty: TType -> TypeStructure voption

/// Try to compute a cache key for overload resolution.
/// Returns None if the resolution cannot be cached (e.g., unresolved type variables, named arguments).
val tryComputeOverloadCacheKey:
    g: TcGlobals ->
    calledMethGroup: CalledMeth<'T> list ->
    callerArgs: CallerArgs<'T> ->
    reqdRetTyOpt: TType option ->
    anyHasOutArgs: bool ->
        OverloadResolutionCacheKey voption

/// Compute cache result from resolution outcome
val computeCacheResult:
    calledMethGroup: CalledMeth<'T> list ->
    calledMethOpt: CalledMeth<'T> voption ->
        OverloadResolutionCacheResult option

/// Stores an overload resolution result in the cache.
/// For successful resolutions, finds the method's index in calledMethGroup and stores CachedResolved.
/// Failed resolutions are not cached.
///
/// Also computes and stores under an "after" key if types were solved during resolution.
/// This allows future calls with already-solved types to hit the cache directly.
val storeCacheResult:
    g: TcGlobals ->
    cache: Cache<OverloadResolutionCacheKey, OverloadResolutionCacheResult> ->
    cacheKeyOpt: OverloadResolutionCacheKey voption ->
    calledMethGroup: CalledMeth<'T> list ->
    callerArgs: CallerArgs<'T> ->
    reqdRetTyOpt: TType option ->
    anyHasOutArgs: bool ->
    calledMethOpt: CalledMeth<'T> voption ->
        unit
