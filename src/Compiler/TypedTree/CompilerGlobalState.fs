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

    /// Create a naming scope tied to a single ImplFile, identified by 'fileRange' (whose FileIndex
    /// is the consumer file currently being optimized). Names allocated through the returned scope
    /// are bucketed by that file, so parallel optimization of different files cannot race on a
    /// shared name-counter bucket.
    member this.NewFileScope (fileRange: range) = PerFileNamingScope(this, fileRange.FileIndex)

/// A compiler-generated-name allocation scope bound to a single ImplFile being optimized.
///
/// The constructor is intentionally not part of the public signature: a scope can only be obtained
/// from NiceNameGenerator.NewFileScope at the per-file boundary of the parallel optimizer. This makes
/// it impossible for a call site to accidentally bucket names by the wrong (e.g. inlined-source) file
/// and thereby reintroduce the non-determinism fixed by https://github.com/dotnet/fsharp/issues/19732.
and [<Sealed>] PerFileNamingScope internal (nng: NiceNameGenerator, fileIndex: int) =

    /// Allocate a fresh compiler-generated name within this file's scope. 'm' contributes only the
    /// source-location marker in the generated name; the determinism-critical uniqueness bucket is
    /// fixed by this scope's file and never by 'm'.
    member _.Fresh (name: string, m: range) =
        nng.FreshCompilerGeneratedNameInScope(fileIndex, name, m)

/// Tracks, per thread, the index of the file currently being code-generated. IlxGen sets this around
/// each file's code generation (both sequential and parallel) so that compiler-generated names produced
/// during code generation (via StableNiceNameGenerator) are bucketed by the file consuming/emitting them
/// rather than by the (possibly inlined) source location they originate from. Without this, parallel code
/// generation of different files races on a shared name-counter bucket and produces non-deterministic
/// disambiguation suffixes. See https://github.com/dotnet/fsharp/issues/19732.
[<Sealed; AbstractClass>]
type internal CodegenNamingScope private () =

    [<DefaultValue; ThreadStatic>]
    static val mutable private scopePlusOne: int

    /// The index of the file currently being code-generated on this thread, if any.
    static member Current =
        match CodegenNamingScope.scopePlusOne with
        | 0 -> ValueNone
        | n -> ValueSome(n - 1)

    /// Run 'action' with the current code-generation file scope set to 'fileScopeIndex'.
    static member With(fileScopeIndex: int, action: unit -> unit) =
        let prev = CodegenNamingScope.scopePlusOne
        CodegenNamingScope.scopePlusOne <- fileScopeIndex + 1

        try
            action ()
        finally
            CodegenNamingScope.scopePlusOne <- prev

/// Generates compiler-generated names marked up with a source code location, but if given the same unique value then
/// return precisely the same name. Each name generated also includes the StartLine number of the range passed in
/// at the point of first generation.
///
/// This type may be accessed concurrently, though in practice it is only used from the compilation thread.
/// It is made concurrency-safe since a global instance of the type is allocated in tast.fs.
type StableNiceNameGenerator() = 

    let niceNames = ConcurrentDictionary<string * int64, string>(max Environment.ProcessorCount 1, 127)
    let innerGenerator = NiceNameGenerator()

    member x.GetUniqueCompilerGeneratedName (name, m: range, uniq) =
        let basicName = GetBasicNameOfPossibleCompilerGeneratedName name
        let key = basicName, uniq
        niceNames.GetOrAdd(key, fun (basicName, _) ->
            // When code generation is in progress, bucket the uniqueness counter by the file being
            // emitted (deterministic per build) rather than by m.FileIndex (which, for inlined code,
            // points at the shared source of origin and therefore races under parallel code generation).
            match CodegenNamingScope.Current with
            | ValueSome scopeFileIndex -> innerGenerator.FreshCompilerGeneratedNameInScope(scopeFileIndex, basicName, m)
            | ValueNone -> innerGenerator.FreshCompilerGeneratedNameOfBasicName(basicName, m))

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