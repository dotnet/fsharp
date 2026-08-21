// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Defines the global environment for all type checking.

module internal FSharp.Compiler.CompilerGlobalState

open FSharp.Compiler.Text

/// Generates compiler-generated names. Each name generated also includes the StartLine number of the range passed in
/// at the point of first generation.
///
/// This type may be accessed concurrently, though in practice it is only used from the compilation thread.
/// It is made concurrency-safe since a global instance of the type is allocated in tast.fs, and it is good
/// policy to make all globally-allocated objects concurrency safe in case future versions of the compiler
/// are used to host multiple concurrent instances of compilation.
type NiceNameGenerator =

    new: unit -> NiceNameGenerator
    member FreshCompilerGeneratedName: name: string * m: range -> string

/// Generates compiler-generated names marked up with a source code location, but if given the same unique value then
/// return precisely the same name. Each name generated also includes the StartLine number of the range passed in
/// at the point of first generation.
///
/// This type may be accessed concurrently, though in practice it is only used from the compilation thread.
/// It is made concurrency-safe since a global instance of the type is allocated in tast.fs.
type StableNiceNameGenerator =

    new: unit -> StableNiceNameGenerator
    member GetUniqueCompilerGeneratedName: name: string * m: range * uniq: int64 -> string

/// A compiler-generated-name allocation scope bound to a single ImplFile being optimized.
/// Instances can only be obtained from CompilerGlobalState.NewFileScope so a call site can't
/// accidentally bucket names by the wrong (e.g. inlined-source) file and reintroduce the
/// non-determinism fixed by https://github.com/dotnet/fsharp/issues/19732.
[<Sealed>]
type PerFileNamingScope =

    /// Allocate a fresh compiler-generated name within this file's scope. 'm' contributes only the
    /// source-location marker baked into the generated name; the uniqueness bucket is this scope's file.
    member Fresh: name: string * m: range -> string

type internal CompilerGlobalState =

    new: unit -> CompilerGlobalState

    /// A name generator used by IlxGen for static fields, some generated arguments and other things.
    member IlxGenNiceNameGenerator: NiceNameGenerator

    /// A global generator of compiler generated names
    member NiceNameGenerator: NiceNameGenerator

    /// A global generator of stable compiler generated names
    member StableNameGenerator: StableNiceNameGenerator

    /// Create a per-file naming scope for the ImplFile identified by 'fileRange'. All names allocated
    /// through the returned scope are bucketed by that file's FileIndex, guaranteeing determinism
    /// under parallel optimization. See https://github.com/dotnet/fsharp/issues/19732.
    member NewFileScope: fileRange: range -> PerFileNamingScope

type Unique = int64

/// Concurrency-safe
val newUnique: (unit -> int64)

/// Unique name generator for stamps attached to to val_specs, tycon_specs etc.
/// Concurrency-safe
val newStamp: (unit -> int64)
