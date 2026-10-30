// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Defines the global environment for all type checking.

module FSharp.Compiler.CompilerGlobalState

open System
open System.Collections.Concurrent
open System.Threading
open Internal.Utilities.Library
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.Text
open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.CompilerGeneratedNameMapState

/// Generates compiler-generated names. Each name generated also includes the StartLine number of the range passed in
/// at the point of first generation.
///
/// This type may be accessed concurrently, though in practice it is only used from the compilation thread.
/// It is made concurrency-safe since a global instance of the type is allocated in tast.fs, and it is good
/// policy to make all globally-allocated objects concurrency safe in case future versions of the compiler
/// are used to host multiple concurrent instances of compilation.
type NiceNameGenerator(getCompilerGeneratedNameMap: unit -> ICompilerGeneratedNameMap option) =
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
        match getCompilerGeneratedNameMap() with
        | Some map ->
            map.GetOrAddName basicName
        | None ->
            let count = increment basicName m
            mkName basicName m count

    member this.FreshCompilerGeneratedName (name, m: range) =
        this.FreshCompilerGeneratedNameOfBasicName (GetBasicNameOfPossibleCompilerGeneratedName name, m)

    member _.FreshCompilerGeneratedNameInScope (scopeFileIndex: int, name: string, m: range) =
        let basicName = GetBasicNameOfPossibleCompilerGeneratedName name

        // The hot reload replay map must win over per-file occurrence buckets, exactly as it
        // does in FreshCompilerGeneratedNameOfBasicName: when a session installs the map, every
        // allocation path replays the baseline's stable names, otherwise line-based per-file
        // names would drift under edits. The map is only ever installed by the hot reload emit
        // hook or the in-process compile, so the deterministic per-file bucketing from
        // https://github.com/dotnet/fsharp/issues/19732 is untouched in normal compilation.
        match getCompilerGeneratedNameMap() with
        | Some map -> map.GetOrAddName basicName
        | None ->
            let count = incrementBucket basicName scopeFileIndex
            mkName basicName m count

    new () = NiceNameGenerator(fun () -> None)

    /// Reset the per-(basicName, file) occurrence counters so a subsequent codegen run assigns the
    /// same compiler-generated occurrence names a fresh process would. Callers must ensure no
    /// concurrent codegen is using this generator when resetting.
    member _.ResetCompilerGeneratedNameState() = basicNameCounts.Clear()

/// Generates compiler-generated names marked up with a source code location, but if given the same unique value then
/// return precisely the same name. Each name generated also includes the StartLine number of the range passed in
/// at the point of first generation.
///
/// This type may be accessed concurrently, though in practice it is only used from the compilation thread.
/// It is made concurrency-safe since a global instance of the type is allocated in tast.fs.
type StableNiceNameGenerator(getCompilerGeneratedNameMap: unit -> ICompilerGeneratedNameMap option) =

    let niceNames = ConcurrentDictionary<string * int64, Lazy<string>>(max Environment.ProcessorCount 1, 127)
    let innerGenerator = NiceNameGenerator(getCompilerGeneratedNameMap)

    member x.GetUniqueCompilerGeneratedName (name, m: range, uniq) =
        let basicName = GetBasicNameOfPossibleCompilerGeneratedName name
        let key = basicName, uniq
        niceNames.GetOrAddLazy(key, fun (basicName, _) -> innerGenerator.FreshCompilerGeneratedNameOfBasicName(basicName, m))

    new () = StableNiceNameGenerator(fun () -> None)

    /// Reset the stable-name cache and inner occurrence counters, so both the cached stable names and
    /// the underlying occurrence counters are cleared. See NiceNameGenerator.ResetCompilerGeneratedNameState.
    member _.ResetCompilerGeneratedNameState() =
        niceNames.Clear()
        innerGenerator.ResetCompilerGeneratedNameState()

[<Sealed>]
type PerFileNamingScope internal (nng: NiceNameGenerator, fileIndex: int) =

    member _.Fresh (name: string, m: range) =
        nng.FreshCompilerGeneratedNameInScope(fileIndex, name, m)

type internal CompilerGlobalState () as this =
    /// Reader for the optional hot reload synthesized-name map attached to this
    /// instance. The accessor resolves the side-channel slot once, so each generated
    /// name costs a single None check (not a weak-table probe and lock) when no map
    /// is installed, i.e. on every compile without the hot reload emit hook.
    let getCompilerGeneratedNameMap = getCompilerGeneratedNameMapAccessor (this :> obj)

    /// A global generator of compiler generated names
    let globalNng = NiceNameGenerator(getCompilerGeneratedNameMap)

    /// A global generator of stable compiler generated names
    let globalStableNameGenerator = StableNiceNameGenerator(getCompilerGeneratedNameMap)

    /// A name generator used by IlxGen for static fields, some generated arguments and other things.
    let ilxgenGlobalNng = NiceNameGenerator(getCompilerGeneratedNameMap)

    member _.NiceNameGenerator = globalNng

    member _.StableNameGenerator = globalStableNameGenerator

    member _.IlxGenNiceNameGenerator = ilxgenGlobalNng

    member _.NewFileScope (fileRange: range) =
        PerFileNamingScope(globalNng, fileRange.FileIndex)

    /// Reset every compiler-generated-name allocator owned by this compiler state so repeated
    /// codegen runs over the same source reproduce a fresh-process layout. The caller must ensure
    /// no compilation is concurrently generating names.
    member _.ResetCompilerGeneratedNameState() =
        globalNng.ResetCompilerGeneratedNameState()
        globalStableNameGenerator.ResetCompilerGeneratedNameState()
        ilxgenGlobalNng.ResetCompilerGeneratedNameState()

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
