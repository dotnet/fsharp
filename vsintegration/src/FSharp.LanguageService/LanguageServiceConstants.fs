// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System.Threading.Tasks

[<RequireQualifiedAccess>]
module internal LanguageServiceConstants =
    
    /// "F#"
    [<Literal>]
    let FSharpLanguageName = "F#"
        
    [<Literal>]
    /// "F# Language Service"
    let FSharpLanguageServiceCallbackName = "F# Language Service"


[<AutoOpen>]
module AsyncExtensions =
    // TODO when FSharp.Core package dep moves to a 11.x that includes RunSynchronouslyImmediate, remove shimming
    type Async with
        static member RunSynchronouslyImmediate (computation: Async<'T>, ?cancellationToken) =
            let tcs = TaskCompletionSource<'T>()
            Async.StartWithContinuations(computation, tcs.SetResult, tcs.SetException, tcs.SetException, ?cancellationToken = cancellationToken)
            // Synchronously block waiting for the result (i.e. even if continuations run on another thread, caller thread will be blocked)
            tcs.Task.GetAwaiter().GetResult() // GetResult() unpacks the AggregateException that .Result would present

