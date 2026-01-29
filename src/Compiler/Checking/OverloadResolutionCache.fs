// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Caching infrastructure for overload resolution results
module internal FSharp.Compiler.OverloadResolutionCache

open Internal.Utilities.Library
open Internal.Utilities.TypeHashing
open Internal.Utilities.TypeHashing.StructuralUtilities

open FSharp.Compiler
open FSharp.Compiler.Caches
open FSharp.Compiler.Infos
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
#endif

/// Cache key for overload resolution: combines method group identity with caller argument types and return type
type OverloadResolutionCacheKey =
    {
        /// Hash combining all method identities in the method group
        MethodGroupHash: int
        /// Type structures for caller object arguments (the 'this' argument for instance/extension methods)
        /// This is critical for extension methods where the 'this' type determines the overload
        ObjArgTypeStructures: TypeStructure list
        /// Type structures for each caller argument (only used when all types are stable)
        ArgTypeStructures: TypeStructure list
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
    /// Resolution failed (no matching overload)
    | CachedFailed

/// Gets a per-TcGlobals overload resolution cache.
/// Uses WeakMap to tie cache lifetime to TcGlobals (per-compilation isolation).
let getOverloadResolutionCache =
    let factory (g: TcGlobals) =
        let options =
            match g.compilationMode with
            | CompilationMode.OneOff ->
                Caches.CacheOptions.getDefault HashIdentity.Structural |> Caches.CacheOptions.withNoEviction
            | _ ->
                { Caches.CacheOptions.getDefault HashIdentity.Structural with
                    TotalCapacity = 4096
                    HeadroomPercentage = 50 }

        new Caches.Cache<OverloadResolutionCacheKey, OverloadResolutionCacheResult>(options, "overloadResolutionCache")

    Internal.Utilities.Library.Extras.WeakMap.getOrCreate factory

/// Check if a token array contains any Unsolved tokens (flexible unsolved typars)
let private hasUnsolvedTokens (tokens: TypeToken[]) =
    tokens |> Array.exists (function TypeToken.Unsolved _ -> true | _ -> false)

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
let tryGetTypeStructureForOverloadCache (g: TcGlobals) (ty: TType) : TypeStructure voption =
    let ty = stripTyEqns g ty

    match tryGetTypeStructureOfStrippedType ty with
    | ValueSome(Stable tokens) -> ValueSome(Stable tokens)
    | ValueSome(Unstable tokens) ->
        // Only accept Unstable if it doesn't contain flexible unsolved typars
        // Unstable due to solved typars is safe; Unstable due to unsolved is not
        if hasUnsolvedTokens tokens then
            ValueNone // Reject - contains unsolved flexible typars
        else
            ValueSome(Stable tokens) // Accept - unstable only due to solved typars
    | ValueSome PossiblyInfinite -> ValueNone
    | ValueNone -> ValueNone

/// Compute a hash for a method info for caching purposes
let rec computeMethInfoHash (minfo: MethInfo) : int =
    match minfo with
    | FSMeth(_, _, vref, _) -> hash (vref.Stamp, vref.LogicalName)
    | ILMeth(_, ilMethInfo, _) -> hash (ilMethInfo.ILName, ilMethInfo.DeclaringTyconRef.Stamp)
    | DefaultStructCtor(_, _) -> hash "DefaultStructCtor"
    | MethInfoWithModifiedReturnType(original, _) -> computeMethInfoHash original
#if !NO_TYPEPROVIDERS
    | ProvidedMeth(_, mb, _, _) ->
        hash (mb.PUntaint((fun m -> m.Name, (nonNull<ProvidedType> m.DeclaringType).FullName |> string), Range.range0))
#endif

/// Try to compute a cache key for overload resolution.
/// Returns None if the resolution cannot be cached (e.g., unresolved type variables, named arguments).
let tryComputeOverloadCacheKey
    (g: TcGlobals)
    (calledMethGroup: CalledMeth<'T> list)
    (callerArgs: CallerArgs<'T>)
    (reqdRetTyOpt: TType option)
    (anyHasOutArgs: bool)
    : OverloadResolutionCacheKey voption
    =

    // Don't cache if there are named arguments (simplifies key computation)
    let hasNamedArgs =
        callerArgs.Named |> List.exists (fun namedList -> not (List.isEmpty namedList))

    if hasNamedArgs then
        ValueNone
    else

        // Compute method group hash - must be order-dependent since we cache by index
        // Using combineHash pattern from HashingPrimitives for consistency
        let mutable methodGroupHash = 0

        for cmeth in calledMethGroup do
            let methHash = computeMethInfoHash cmeth.Method
            methodGroupHash <- HashingPrimitives.combineHash methodGroupHash methHash

        // Collect type structures for caller object arguments (the 'this' argument)
        // This is critical for extension methods where the 'this' type determines the overload
        // e.g., GItem1 on Tuple<T1,T2> vs Tuple<T1,T2,T3> vs Tuple<T1,T2,T3,T4>
        let objArgStructures = ResizeArray()
        let mutable allStable = true

        match calledMethGroup with
        | cmeth :: _ ->
            for objArgTy in cmeth.CallerObjArgTys do
                match tryGetTypeStructureForOverloadCache g objArgTy with
                | ValueSome ts -> objArgStructures.Add(ts)
                | ValueNone -> allStable <- false
        | [] -> ()

        if not allStable then
            ValueNone
        else

            // Collect type structures for all caller arguments
            let argStructures = ResizeArray()

            for argList in callerArgs.Unnamed do
                for callerArg in argList do
                    let argTy = callerArg.CallerArgumentType

                    match tryGetTypeStructureForOverloadCache g argTy with
                    | ValueSome ts -> argStructures.Add(ts)
                    | ValueNone -> allStable <- false

            if not allStable then
                ValueNone
            else
                // Compute return type structure if present
                // This is critical for cases like:
                // - c.CheckCooperativeLevel() returning bool (calls no-arg overload)
                // - let a, b = c.CheckCooperativeLevel() (calls byref overload with tuple destructuring)
                let retTyStructure =
                    match reqdRetTyOpt with
                    | Some retTy ->
                        match tryGetTypeStructureForOverloadCache g retTy with
                        | ValueSome ts -> ValueSome ts
                        | ValueNone ->
                            // Return type has unresolved type variable
                            // This is only a problem if any candidate has out args, because out args
                            // affect the effective return type (method returning bool with out int becomes bool*int)
                            // For normal overloads (no out args), the return type doesn't affect resolution
                            if anyHasOutArgs then
                                // Don't cache - the expected return type determines which overload to pick
                                // e.g., c.CheckCooperativeLevel() -> bool vs let a,b = c.CheckCooperativeLevel() -> bool*int
                                ValueNone
                            else
                                // Safe to cache with wildcard - return type doesn't affect resolution
                                // Use empty Stable array as marker for "any return type"
                                ValueSome(Stable [||])
                    | None ->
                        // No return type constraint - use empty marker
                        ValueSome(Stable [||])

                match retTyStructure with
                | ValueNone -> ValueNone
                | retStruct ->
                    // Get caller type arg count from first method (all methods in group have same caller type args)
                    let callerTyArgCount =
                        match calledMethGroup with
                        | cmeth :: _ -> cmeth.NumCallerTyArgs
                        | [] -> 0

                    ValueSome
                        {
                            MethodGroupHash = methodGroupHash
                            ObjArgTypeStructures = Seq.toList objArgStructures
                            ArgTypeStructures = Seq.toList argStructures
                            ReturnTypeStructure = retStruct
                            CallerTyArgCount = callerTyArgCount
                        }

/// Compute cache result from resolution outcome
let computeCacheResult
    (calledMethGroup: CalledMeth<'T> list)
    (calledMethOpt: CalledMeth<'T> voption)
    : OverloadResolutionCacheResult option
    =
    match calledMethOpt with
    | ValueSome calledMeth ->
        calledMethGroup
        |> List.tryFindIndex (fun cm -> obj.ReferenceEquals(cm, calledMeth))
        |> Option.map CachedResolved
    | ValueNone -> Some CachedFailed

/// Stores an overload resolution result in the cache.
/// For successful resolutions, finds the method's index in calledMethGroup and stores CachedResolved.
/// For failures, stores CachedFailed.
/// 
/// Also computes and stores under an "after" key if types were solved during resolution.
/// This allows future calls with already-solved types to hit the cache directly.
let storeCacheResult
    (g: TcGlobals)
    (cache: Caches.Cache<OverloadResolutionCacheKey, OverloadResolutionCacheResult>)
    (cacheKeyOpt: OverloadResolutionCacheKey voption)
    (calledMethGroup: CalledMeth<'T> list)
    (callerArgs: CallerArgs<'T>)
    (reqdRetTyOpt: TType option)
    (anyHasOutArgs: bool)
    (calledMethOpt: CalledMeth<'T> voption)
    =
    match cacheKeyOpt with
    | ValueSome cacheKey ->
        match computeCacheResult calledMethGroup calledMethOpt with
        | Some res ->
            // Store under the "before" key
            cache.TryAdd(cacheKey, res) |> ignore

            // Compute "after" key - types may have been solved during resolution
            // If different from "before" key, store under that too for future hits
            match tryComputeOverloadCacheKey g calledMethGroup callerArgs reqdRetTyOpt anyHasOutArgs with
            | ValueSome afterKey when afterKey <> cacheKey -> cache.TryAdd(afterKey, res) |> ignore
            | _ -> ()
        | None -> ()
    | ValueNone ->
        // Even if we couldn't compute a "before" key (unstable types),
        // try to compute an "after" key now that types may be solved
        match tryComputeOverloadCacheKey g calledMethGroup callerArgs reqdRetTyOpt anyHasOutArgs with
        | ValueSome afterKey ->
            match computeCacheResult calledMethGroup calledMethOpt with
            | Some res -> cache.TryAdd(afterKey, res) |> ignore
            | None -> ()
        | ValueNone -> ()
