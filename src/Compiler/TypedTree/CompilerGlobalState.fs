// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Defines the global environment for all type checking.

module FSharp.Compiler.CompilerGlobalState

open System
open System.Collections.Concurrent
open System.Threading
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.Text

/// Generates compiler-generated names. Each name generated also includes the StartLine number of the range passed in
/// at the point of first generation.
///
/// This type may be accessed concurrently, though in practice it is only used from the compilation thread.
/// It is made concurrency-safe since a global instance of the type is allocated in tast.fs, and it is good
/// policy to make all globally-allocated objects concurrency safe in case future versions of the compiler
/// are used to host multiple concurrent instances of compilation.
type NiceNameGenerator() =    
    let basicNameCounts = ConcurrentDictionary<struct (string * int), int ref>(max Environment.ProcessorCount 1, 127)
    // Cache this as a delegate.
    let basicNameCountsAddDelegate = Func<struct (string * int), int ref>(fun _ -> ref 0)

    let incrementBucket basicName (fileIndex: int) =
        let key = struct (basicName, fileIndex)
        let countCell = basicNameCounts.GetOrAdd(key, basicNameCountsAddDelegate)
        Interlocked.Increment(countCell)

    let increment basicName (m: range) = incrementBucket basicName m.FileIndex

    let mkName basicName (m: range) count =
        CompilerGeneratedNameSuffix basicName (string m.StartLine + (match (count - 1) with 0 -> "" | n -> "-" + string n))

    member _.FreshCompilerGeneratedNameOfBasicName (basicName, m: range) =
        let count = increment basicName m
        mkName basicName m count

    member this.FreshCompilerGeneratedName (name, m: range) =
        this.FreshCompilerGeneratedNameOfBasicName (GetBasicNameOfPossibleCompilerGeneratedName name, m)

    member _.IncrementOnly(name: string, m: range) = increment name m

    /// Allocate a fresh compiler-generated name whose uniqueness counter is bucketed by an
    /// explicit per-file scope (see PerFileNamingScope) rather than by the file index of 'm'.
    /// 'm' is used only for the human-readable start-line marker baked into the generated name,
    /// so passing a range that points at inlined source code can no longer make compiler-generated
    /// names non-deterministic under parallel optimization. See
    /// https://github.com/dotnet/fsharp/issues/19732.
    member _.FreshCompilerGeneratedNameInScope (scopeFileIndex: int, name: string, m: range) =
        let basicName = GetBasicNameOfPossibleCompilerGeneratedName name
        let count = incrementBucket basicName scopeFileIndex
        mkName basicName m count

/// Generates compiler-generated names marked up with a source code location, but if given the same unique value then
/// return precisely the same name. Each name generated also includes the StartLine number of the range passed in
/// at the point of first generation.
///
/// This type may be accessed concurrently, though in practice it is only used from the compilation thread.
/// It is made concurrency-safe since a global instance of the type is allocated in tast.fs.
type StableNiceNameGenerator() = 

    // The value is wrapped in Lazy<_> so the inner counter-incrementing factory runs exactly once
    // per cache key, even when ConcurrentDictionary.GetOrAdd's value-factory is invoked on multiple
    // threads under contention. Without the Lazy wrapper, spurious factory invocations would
    // increment the counter and produce non-deterministic suffixes. See
    // https://github.com/dotnet/fsharp/issues/19732.
    let niceNames = ConcurrentDictionary<string * int64, Lazy<string>>(max Environment.ProcessorCount 1, 127)
    let innerGenerator = NiceNameGenerator()

    member x.GetUniqueCompilerGeneratedName (name, m: range, uniq) =
        let basicName = GetBasicNameOfPossibleCompilerGeneratedName name
        let key = basicName, uniq
        let lazyName =
            niceNames.GetOrAdd(key, fun (basicName, _) ->
                lazy innerGenerator.FreshCompilerGeneratedNameOfBasicName(basicName, m))
        lazyName.Value

/// A compiler-generated-name allocation scope bound to a single ImplFile being optimized. The
/// constructor is not part of the public signature: a scope can only be obtained from
/// CompilerGlobalState.NewFileScope so a call site can't accidentally bucket names by the wrong
/// (e.g. inlined-source) file and reintroduce the non-determinism fixed by
/// https://github.com/dotnet/fsharp/issues/19732.
[<Sealed>]
type PerFileNamingScope internal (nng: NiceNameGenerator, fileIndex: int) =

    /// Allocate a fresh compiler-generated name within this file's scope. 'm' contributes only the
    /// source-location marker in the generated name; the determinism-critical uniqueness bucket is
    /// fixed by this scope's file and never by 'm'.
    member _.Fresh (name: string, m: range) =
        nng.FreshCompilerGeneratedNameInScope(fileIndex, name, m)

/// Per-consumer-file closure type-name allocation scope used by IlxGen.
/// Disambiguates closure type names when the SAME source range is inlined into
/// multiple consumer files — without the per-consumer-file marker, those would
/// race on the shared StableNiceNameGenerator bucket under parallel codegen.
/// See https://github.com/dotnet/fsharp/issues/19928.
///
/// For closures whose source m.FileIndex matches the consumer file, the name
/// uses the legacy format `basicName@<line>[-N]` so existing baselines remain
/// stable. Only cross-file inlined closures get the `F<consumerFileIndex>` marker.
[<Sealed>]
type PerFileClosureNameScope(consumerFileIndex: int) =

    let byUniq = System.Collections.Generic.Dictionary<int64, string>()
    let buckets =
        System.Collections.Generic.Dictionary<struct (string * int * int * bool), int>()

    member _.EmitClosureName(basicName: string, m: range, uniq: int64) =
        match byUniq.TryGetValue uniq with
        | true, cached ->
            cached
        | false, _ ->
            let isInlinedFromAnotherFile = m.FileIndex <> consumerFileIndex

            // Bucket key omits StartColumn so two Lambdas sharing a line (different columns)
            // share one bucket and produce different `-N` suffixes. Embedding the column in the
            // bucket key without also embedding it in the emitted NAME would cause distinct
            // bucket keys to produce the same name (each "first" in its own bucket → both
            // get "" suffix → duplicate type defs in IL).
            //
            // The `isInlinedFromAnotherFile` flag splits the bucket so that local closures
            // (legacy naming) and inlined closures (with `F<consumerFileIndex>` marker) at
            // the same source line do not contend for the same suffix counter.
            let bucketKey =
                struct (basicName, m.FileIndex, m.StartLine, isInlinedFromAnotherFile)

            let occ =
                match buckets.TryGetValue bucketKey with
                | true, n ->
                    buckets[bucketKey] <- n + 1
                    n
                | false, _ ->
                    buckets[bucketKey] <- 1
                    0

            let lineMarker =
                if isInlinedFromAnotherFile then
                    string m.StartLine + "F" + string consumerFileIndex
                else
                    string m.StartLine

            let suffix =
                if occ = 0 then ""
                else "-" + string occ

            let name =
                CompilerGeneratedNameSuffix basicName (lineMarker + suffix)

            byUniq[uniq] <- name
            name

type internal CompilerGlobalState () =
    /// A global generator of compiler generated names
    let globalNng = NiceNameGenerator()

    /// A global generator of stable compiler generated names
    let globalStableNameGenerator = StableNiceNameGenerator ()

    /// A name generator used by IlxGen for static fields, some generated arguments and other things.
    let ilxgenGlobalNng = NiceNameGenerator ()

    member _.NiceNameGenerator = globalNng

    member _.StableNameGenerator = globalStableNameGenerator

    member _.IlxGenNiceNameGenerator = ilxgenGlobalNng

    /// Create a per-file naming scope tied to a single ImplFile. Names allocated through the returned
    /// scope are bucketed by 'fileRange.FileIndex', so parallel optimization of different files cannot
    /// race on a shared name-counter bucket. See https://github.com/dotnet/fsharp/issues/19732.
    member _.NewFileScope (fileRange: range) =
        PerFileNamingScope(globalNng, fileRange.FileIndex)

/// Unique name generator for stamps attached to lambdas and object expressions
type Unique = int64

//++GLOBAL MUTABLE STATE (concurrency-safe)
let mutable private uniqueCount = 0L
let newUnique() = Interlocked.Increment &uniqueCount

/// Unique name generator for stamps attached to to val_specs, tycon_specs etc.
//++GLOBAL MUTABLE STATE (concurrency-safe)
let mutable private stampCount = 0L
let newStamp() =
    let stamp = Interlocked.Increment &stampCount
    stamp