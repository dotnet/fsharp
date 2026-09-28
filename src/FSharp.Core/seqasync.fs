// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core.CompilerServices

#if NET
open System
open System.Collections.Generic
open System.Runtime.ExceptionServices
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Control
open Microsoft.FSharp.Control.TaskBuilderExtensions.LowPriority
open Microsoft.FSharp.Control.TaskBuilderExtensions.HighPriority

module RuntimeAsyncSequenceHelpers =
    let EnumerateTryWith
        (source: IAsyncEnumerable<'T>)
        (exceptionFilter: exn -> IAsyncEnumerable<int>)
        (exceptionHandler: exn -> IAsyncEnumerable<'T>)
        : IAsyncEnumerable<'T> =
        { new IAsyncEnumerable<'T> with
            member _.GetAsyncEnumerator(token) =
                let mutable original: IAsyncEnumerator<'T> = null
                let mutable exceptional: IAsyncEnumerator<'T> = null
                let mutable originalClosed = false
                let mutable finished = false
                let mutable current = Unchecked.defaultof<'T>

                let closeOriginal () =
                    TaskBuilder.task {
                        if not originalClosed then
                            originalClosed <- true

                            if not (isNull original) then
                                let enumerator = original
                                original <- null
                                do! enumerator.DisposeAsync()
                    }

                let matches error =
                    TaskBuilder.task {
                        let iterator = (exceptionFilter error).GetAsyncEnumerator(token)
                        let mutable failure: exn = null
                        let mutable result = false

                        try
                            let! moved = iterator.MoveNextAsync()

                            if not moved then
                                invalidOp "An async sequence exception filter must produce a result."

                            result <- iterator.Current = 1
                        with error ->
                            failure <- error

                        try
                            do! iterator.DisposeAsync()
                        with error ->
                            failure <- error

                        if not (isNull failure) then
                            ExceptionDispatchInfo.Capture(failure).Throw()

                        return result
                    }

                let move () =
                    TaskBuilder.task {
                        try
                            if finished then
                                return false
                            elif not (isNull exceptional) then
                                let! moved = exceptional.MoveNextAsync()

                                if moved then
                                    current <- exceptional.Current
                                else
                                    finished <- true
                                    let completed = exceptional
                                    exceptional <- null
                                    do! completed.DisposeAsync()

                                return moved
                            else
                                let mutable error: exn = null
                                let mutable moved = false

                                try
                                    if isNull original then
                                        original <- source.GetAsyncEnumerator(token)

                                    let! hasNext = original.MoveNextAsync()

                                    if hasNext then
                                        current <- original.Current
                                        moved <- true
                                with caught ->
                                    error <- caught

                                if moved then
                                    return true
                                else
                                    let mutable matched = false

                                    if not (isNull error) then
                                        let! originalMatches = matches error
                                        matched <- originalMatches

                                    let mutable cleanupError: exn = null

                                    try
                                        do! closeOriginal ()
                                    with caught ->
                                        cleanupError <- caught

                                    if not (isNull cleanupError) then
                                        if not (isNull error) && matched then
                                            let! cleanupMatches = matches cleanupError

                                            if cleanupMatches then
                                                error <- cleanupError
                                        else
                                            error <- cleanupError
                                            let! cleanupMatches = matches error
                                            matched <- cleanupMatches

                                    if not (isNull error) then
                                        if not matched then
                                            ExceptionDispatchInfo.Capture(error).Throw()

                                        exceptional <- (exceptionHandler error).GetAsyncEnumerator(token)
                                        let! hasNext = exceptional.MoveNextAsync()

                                        if hasNext then
                                            current <- exceptional.Current
                                        else
                                            finished <- true
                                            let completed = exceptional
                                            exceptional <- null
                                            do! completed.DisposeAsync()

                                        return hasNext
                                    else
                                        finished <- true
                                        return false
                        with caught ->
                            finished <- true
                            let mutable failure = caught

                            if not (isNull exceptional) then
                                let enumerator = exceptional
                                exceptional <- null

                                try
                                    do! enumerator.DisposeAsync()
                                with error ->
                                    failure <- error

                            try
                                do! closeOriginal ()
                            with error ->
                                failure <- error

                            ExceptionDispatchInfo.Capture(failure).Throw()
                            return false
                    }

                { new IAsyncEnumerator<'T> with
                    member _.Current = current

                    // The generated consumer already rejects concurrent moves.
                    member _.MoveNextAsync() =
                        ValueTask<bool>(move ())

                    member _.DisposeAsync() =
                        ValueTask(
                            TaskBuilder.task {
                                finished <- true

                                if not (isNull exceptional) then
                                    let enumerator = exceptional
                                    exceptional <- null
                                    do! enumerator.DisposeAsync()
                                else
                                    do! closeOriginal ()
                            }
                        )
                }
        }
#endif
