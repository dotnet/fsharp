module RuntimeAsyncAsyncLocal

open System.Threading
open System.Threading.Tasks

open Microsoft.FSharp.Control
open Microsoft.FSharp.Control.RuntimeAsync.TaskExpressions

let private context = AsyncLocal<string>()

let private preservesValueAcrossAwait () =
    task {
        context.Value <- "before"
        do! Task.Delay(1)

        if context.Value <> "before" then
            failwith "AsyncLocal value was not preserved across await"
    }

let private propagatesValueToNestedRuntimeTask () =
    task {
        context.Value <- "parent"

        let! nestedValue =
            task {
                do! Task.Delay(1)
                return context.Value
            }

        if nestedValue <> "parent" then
            failwith "AsyncLocal value was not propagated to nested task"
    }

let private isolatesChildTaskChanges () =
    task {
        context.Value <- "parent"

        let! childValue =
            Task.Run(fun () ->
                context.Value <- "child"
                context.Value)

        if childValue <> "child" then
            failwith "AsyncLocal child value was not set"

        if context.Value <> "parent" then
            failwith "AsyncLocal child change leaked to parent"
    }

let private queuedAsync2ChildSeesParentContext () =
    task {
        let leaf = async2 { return context.Value }

        let middle =
            async2 {
                context.Value <- "middle"
                return! leaf
            }

        let outer = async2 { return! middle }

        context.Value <- "caller"
        let! actual = outer.Start CancellationToken.None

        if actual <> "middle" then
            failwithf "Queued async2 child observed AsyncLocal value '%s' instead of 'middle'" actual
    }

let private queuedAsync2SiblingsUseTheirCapturedContexts () =
    task {
        let overwrite =
            Async2<unit>(fun _ ->
                context.Value <- "preceding child"
                ValueTask<unit>(()))

        let observe = Async2<string>(fun _ -> ValueTask<string>(context.Value))

        let middle =
            async2 {
                context.Value <- "middle"
                let! observed = observe
                and! () = overwrite
                return observed
            }

        let outer = async2 { return! middle }

        context.Value <- "caller"
        let! actual = outer.Start CancellationToken.None

        if actual <> "middle" then
            failwithf "Queued async2 sibling observed AsyncLocal value '%s' instead of 'middle'" actual
    }

[<EntryPoint>]
let main _ =
    context.Value <- "main"

    [|
        preservesValueAcrossAwait ()
        propagatesValueToNestedRuntimeTask ()
        isolatesChildTaskChanges ()
        queuedAsync2ChildSeesParentContext ()
        queuedAsync2SiblingsUseTheirCapturedContexts ()
    |]
    |> Task.WhenAll
    |> _.Result
    |> ignore

    if context.Value <> "main" then
        failwith "AsyncLocal value was not preserved after all tasks completed"

    0
