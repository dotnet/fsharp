// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Caching infrastructure for overload resolution results
module internal FSharp.Compiler.OverloadResolutionCache

open Internal.Utilities.Library
open Internal.Utilities.TypeHashing
open Internal.Utilities.TypeHashing.StructuralUtilities

open FSharp.Compiler
open FSharp.Compiler.Caches
open FSharp.Compiler.Features
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
let getOverloadResolutionCache =
    let factory (g: TcGlobals) =
        let options =
            match g.compilationMode with
            | CompilationMode.OneOff ->
                Caches.CacheOptions.getDefault HashIdentity.Structural
                |> Caches.CacheOptions.withNoEviction
            | _ ->
                { Caches.CacheOptions.getDefault HashIdentity.Structural with
                    TotalCapacity = 4096
                    HeadroomPercentage = 50
                }

        new Caches.Cache<OverloadResolutionCacheKey, OverloadResolutionCacheResult>(options, "overloadResolutionCache")

    Internal.Utilities.Library.Extras.WeakMap.getOrCreate factory

/// Check if a token array contains any Unsolved tokens (flexible unsolved typars)
let private hasUnsolvedTokens (tokens: TypeToken[]) =
    tokens
    |> Array.exists (function
        | TypeToken.Unsolved _ -> true
        | _ -> false)

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
        if hasUnsolvedTokens tokens then
            ValueNone
        else
            ValueSome(Stable tokens)
    | ValueSome PossiblyInfinite -> ValueNone
    | ValueNone -> ValueNone

let rec computeMethInfoHash (minfo: MethInfo) : int =
    match minfo with
    | FSMeth(_, _, vref, _) -> HashingPrimitives.combineHash (hash vref.Stamp) (hash vref.LogicalName)
    | ILMeth(_, ilMethInfo, _) -> HashingPrimitives.combineHash (hash ilMethInfo.ILName) (hash ilMethInfo.DeclaringTyconRef.Stamp)
    | DefaultStructCtor(_, _) -> hash "DefaultStructCtor"
    | MethInfoWithModifiedReturnType(original, _) -> computeMethInfoHash original
#if !NO_TYPEPROVIDERS
    | ProvidedMeth(_, mb, _, _) ->
        let name, declTypeName =
            mb.PUntaint((fun m -> m.Name, (nonNull<ProvidedType> m.DeclaringType).FullName |> string), Range.range0)

        HashingPrimitives.combineHash (hash name) (hash declTypeName)
#endif

/// Try to compute a cache key for overload resolution.
/// Returns None if the resolution cannot be cached (e.g., unresolved type variables, named arguments).
let tryComputeOverloadCacheKey
    (g: TcGlobals)
    (calledMethGroup: CalledMeth<'T> list)
    (callerArgs: CallerArgs<'T>)
    (reqdRetTyOpt: TType option)
    (anyHasOutArgs: bool)
    : OverloadResolutionCacheKey voption =

    let hasNamedArgs =
        callerArgs.Named |> List.exists (fun namedList -> not (List.isEmpty namedList))

    if hasNamedArgs then
        ValueNone
    else

        let mutable methodGroupHash = 0

        for cmeth in calledMethGroup do
            let methHash = computeMethInfoHash cmeth.Method
            methodGroupHash <- HashingPrimitives.combineHash methodGroupHash methHash

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
                let retTyStructure =
                    match reqdRetTyOpt with
                    | Some retTy ->
                        match tryGetTypeStructureForOverloadCache g retTy with
                        | ValueSome ts -> ValueSome ts
                        | ValueNone -> if anyHasOutArgs then ValueNone else ValueSome(Stable [||])
                    | None -> ValueSome(Stable [||])

                match retTyStructure with
                | ValueNone -> ValueNone
                | retStruct ->
                    let callerTyArgCount =
                        match calledMethGroup with
                        | cmeth :: _ -> cmeth.NumCallerTyArgs
                        | [] -> 0

                    ValueSome
                        {
                            MethodGroupHash = methodGroupHash
                            ObjArgTypeStructures = objArgStructures.ToArray()
                            ArgTypeStructures = argStructures.ToArray()
                            ReturnTypeStructure = retStruct
                            CallerTyArgCount = callerTyArgCount
                        }

/// Compute cache result from resolution outcome
let computeCacheResult
    (calledMethGroup: CalledMeth<'T> list)
    (calledMethOpt: CalledMeth<'T> voption)
    : OverloadResolutionCacheResult option =
    match calledMethOpt with
    | ValueSome calledMeth ->
        calledMethGroup
        |> List.tryFindIndex (fun cm -> obj.ReferenceEquals(cm, calledMeth))
        |> Option.map CachedResolved
    | ValueNone -> None

/// Stores an overload resolution result in the cache.
/// For successful resolutions, finds the method's index in calledMethGroup and stores CachedResolved.
/// Failed resolutions are not cached.
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
    if not (g.langVersion.SupportsFeature LanguageFeature.MethodOverloadsCache) then
        ()
    else
        match cacheKeyOpt with
        | ValueSome cacheKey ->
            match computeCacheResult calledMethGroup calledMethOpt with
            | Some res ->
                cache.TryAdd(cacheKey, res) |> ignore

                match tryComputeOverloadCacheKey g calledMethGroup callerArgs reqdRetTyOpt anyHasOutArgs with
                | ValueSome afterKey when afterKey <> cacheKey -> cache.TryAdd(afterKey, res) |> ignore
                | _ -> ()
            | None -> ()
        | ValueNone ->
            match tryComputeOverloadCacheKey g calledMethGroup callerArgs reqdRetTyOpt anyHasOutArgs with
            | ValueSome afterKey ->
                match computeCacheResult calledMethGroup calledMethOpt with
                | Some res -> cache.TryAdd(afterKey, res) |> ignore
                | None -> ()
            | ValueNone -> ()
