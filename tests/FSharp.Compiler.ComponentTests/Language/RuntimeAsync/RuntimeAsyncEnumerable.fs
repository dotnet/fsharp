module RuntimeAsyncEnumerable

open System
open System.Collections.Generic
open System.Diagnostics
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Control
open Microsoft.FSharp.Core.CompilerServices

type CounterEnumerator(count: int) =
    let mutable current = -1

    member private _.MoveNextCore() : ValueTask<bool> =
        StateMachineHelpers.__runtimeAsyncReturnValueTask (
            AsyncHelpers.Await(Task.Delay(1))
            current <- current + 1
            current < count
        )

    interface IAsyncEnumerator<int> with
        member _.Current = current
        member this.MoveNextAsync() = this.MoveNextCore()
        member _.DisposeAsync() = ValueTask()

type CounterEnumerable(count: int) =
    interface IAsyncEnumerable<int> with
        member _.GetAsyncEnumerator(_cancellationToken: CancellationToken) =
            CounterEnumerator(count) :> IAsyncEnumerator<int>

let objectExpressionEnumerable count : IAsyncEnumerable<int> =
    { new IAsyncEnumerable<int> with
        member _.GetAsyncEnumerator(_cancellationToken: CancellationToken) =
            let mutable current = -1

            { new IAsyncEnumerator<int> with
                member _.Current = current

                member _.MoveNextAsync() : ValueTask<bool> =
                    StateMachineHelpers.__runtimeAsyncReturnValueTask (
                        AsyncHelpers.Await(Task.Delay(100))
                        current <- current + 1
                        current < count
                    )

                member _.DisposeAsync() = ValueTask()
            }
    }

let collect (enumerable: IAsyncEnumerable<int>) : Task<int[]> =
    StateMachineHelpers.__runtimeAsyncReturn (
        let enumerator = enumerable.GetAsyncEnumerator(CancellationToken.None)
        let values = ResizeArray<int>()
        let mutable hasNext = AsyncHelpers.Await(enumerator.MoveNextAsync())

        while hasNext do
            values.Add enumerator.Current
            hasNext <- AsyncHelpers.Await(enumerator.MoveNextAsync())

        AsyncHelpers.Await(enumerator.DisposeAsync())
        Seq.toArray values
    )

let collectWithTaskCe (enumerable: IAsyncEnumerable<int>) =
    task {
        use enumerator = enumerable.GetAsyncEnumerator(CancellationToken.None)
        let values = ResizeArray<int>()
        let stopwatch = Stopwatch.StartNew()

        while! enumerator.MoveNextAsync() do
            values.Add enumerator.Current

        return Seq.toArray values, stopwatch.Elapsed
    }

[<EntryPoint>]
let main _ =
    let expected = [| 0; 1; 2 |]
    let classEnumerable = CounterEnumerable(3) :> IAsyncEnumerable<int>

    let classValues =
        collect classEnumerable |> fun task -> task.GetAwaiter().GetResult()

    let taskValues, elapsed =
        collectWithTaskCe (objectExpressionEnumerable 3)
        |> fun task -> task.GetAwaiter().GetResult()

    if
        classValues = expected
        && taskValues = expected
        && elapsed >= TimeSpan.FromMilliseconds(300.)
    then
        0
    else
        1
