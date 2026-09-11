// Direct runtime-async calls must move suspension points out of exception handlers.
module RuntimeAsyncAwaitInExceptionRegion

open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let run () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        try
            1
        finally
            AsyncHelpers.Await(Task.Delay(1))
    )

let runCatch () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        try
            failwith "boom"
        with _ ->
            AsyncHelpers.Await(Task.Delay(1))
            2
    )

let runFilter () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        try
            try
                raise (System.InvalidOperationException())
            with :? System.InvalidOperationException when
                (AsyncHelpers.Await(Task.Delay(1))
                 false) ->
                2
        with :? System.InvalidOperationException ->
            3
    )

[<EntryPoint>]
let main _ =
    let first = run ()
    let second = runCatch ()
    let third = runFilter ()

    let results =
        [|
            first.GetAwaiter().GetResult()
            second.GetAwaiter().GetResult()
            third.GetAwaiter().GetResult()
        |]

    if results = [| 1; 2; 3 |] then 0 else 1
