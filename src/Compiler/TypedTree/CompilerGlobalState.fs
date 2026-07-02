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

    /// Allocate a fresh name for an already-basic name, bucketed by `scopeFileIndex` (not `m.FileIndex`).
    member _.FreshCompilerGeneratedNameOfBasicNameInScope (scopeFileIndex: int, basicName, m: range) =
        let count = incrementBucket basicName scopeFileIndex
        mkName basicName m count

    /// Allocate a fresh name bucketed by `scopeFileIndex` (not by `m.FileIndex`).
    member this.FreshCompilerGeneratedNameInScope (scopeFileIndex: int, name: string, m: range) =
        this.FreshCompilerGeneratedNameOfBasicNameInScope(scopeFileIndex, GetBasicNameOfPossibleCompilerGeneratedName name, m)

/// Generates compiler-generated names marked up with a source code location, but if given the same unique value then
/// return precisely the same name. Each name generated also includes the StartLine number of the range passed in
/// at the point of first generation.
///
/// This type may be accessed concurrently, though in practice it is only used from the compilation thread.
/// It is made concurrency-safe since a global instance of the type is allocated in tast.fs.
type StableNiceNameGenerator() =

    let niceNames = ConcurrentDictionary<string * int64, Lazy<string>>(max Environment.ProcessorCount 1, 127)
    let innerGenerator = NiceNameGenerator()

    // Shared stable-name cache: a given `uniq` always yields the same name, materialized once (lazily)
    // via `fresh`, which owns the bucketing choice (per-range vs per-scope-file).
    member private _.GetOrAddStableName(name, uniq, fresh: string -> string) =
        let basicName = GetBasicNameOfPossibleCompilerGeneratedName name
        niceNames.GetOrAdd((basicName, uniq), (fun (basicName, _) -> lazy fresh basicName)).Value

    member x.GetUniqueCompilerGeneratedName(name, m: range, uniq) =
        x.GetOrAddStableName(name, uniq, (fun basicName -> innerGenerator.FreshCompilerGeneratedNameOfBasicName(basicName, m)))

    /// As GetUniqueCompilerGeneratedName, but the disambiguating "-N" suffix counter is bucketed by
    /// `scopeFileIndex` rather than by `m.FileIndex`. IlxGen passes the current codegen file scope so
    /// that closure type names materialized during the parallel per-file drain cannot race on a bucket
    /// shared across files - e.g. an inlined closure whose range points at its (foreign or synthetic)
    /// definition file. `m` still contributes only the source-line marker. See
    /// https://github.com/dotnet/fsharp/issues/19928.
    member x.GetUniqueCompilerGeneratedNameInScope(scopeFileIndex: int, name, m: range, uniq) =
        x.GetOrAddStableName(name, uniq, (fun basicName -> innerGenerator.FreshCompilerGeneratedNameOfBasicNameInScope(scopeFileIndex, basicName, m)))

[<Sealed>]
type PerFileNamingScope internal (nng: NiceNameGenerator, fileIndex: int) =

    /// Allocate a fresh name within this file's bucket; `m` contributes only the source-location marker.
    member _.Fresh (name: string, m: range) =
        nng.FreshCompilerGeneratedNameInScope(fileIndex, name, m)

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