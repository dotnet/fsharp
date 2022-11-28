// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Defines the global environment for all type checking.

module FSharp.Compiler.CompilerGlobalState

open System
open System.Collections.Generic
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
    let basicNameCounts = ConcurrentDictionary<string,Ref<int>>(max Environment.ProcessorCount 1, 127)
   
    member _.FreshCompilerGeneratedName (name, m: range) =
        let basicName = GetBasicNameOfPossibleCompilerGeneratedName name
        let countCell = basicNameCounts.GetOrAdd(basicName,fun k -> ref 0)
        let count = Interlocked.Increment(countCell)
        
        CompilerGeneratedNameSuffix basicName (string m.StartLine + (match (count-1) with 0 -> "" | n -> "-" + string n))

/// Generates compiler-generated names marked up with a source code location, but if given the same unique value then
/// return precisely the same name. Each name generated also includes the StartLine number of the range passed in
/// at the point of first generation.
///
/// This type may be accessed concurrently, though in practice it is only used from the compilation thread.
/// It is made concurrency-safe since a global instance of the type is allocated in tast.fs.
type StableNiceNameGenerator() = 

    let niceNames = ConcurrentDictionary<string * int64, string>(max Environment.ProcessorCount 1, 127)
    let innerGenerator = new NiceNameGenerator()

    member x.GetUniqueCompilerGeneratedName (name, m: range, uniq) =
        let basicName = GetBasicNameOfPossibleCompilerGeneratedName name
        let key = basicName, uniq
        niceNames.GetOrAdd(key, fun _ -> innerGenerator.FreshCompilerGeneratedName(name, m))

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
let newUnique() = System.Threading.Interlocked.Increment &uniqueCount

/// Unique name generator for stamps attached to to val_specs, tycon_specs etc.
//++GLOBAL MUTABLE STATE (concurrency-safe)
let mutable private stampCount = 0L
let newStamp() = System.Threading.Interlocked.Increment &stampCount
