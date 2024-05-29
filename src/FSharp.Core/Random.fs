// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

open System
open System.Threading

[<AbstractClass; Sealed>]
type internal ThreadSafeRandom() =
    [<DefaultValue>] static val mutable private globalRandom : Random
    [<DefaultValue>] static val mutable private localRandom : ThreadLocal<Random>
    static do ThreadSafeRandom.globalRandom <- Random()
    static do ThreadSafeRandom.localRandom <- new ThreadLocal<Random>(fun () ->
        lock ThreadSafeRandom.globalRandom (fun () ->
            Random(ThreadSafeRandom.globalRandom.Next())
        )
    )
    // Don't pass the returned Random object between threads
    static member Shared = ThreadSafeRandom.localRandom.Value