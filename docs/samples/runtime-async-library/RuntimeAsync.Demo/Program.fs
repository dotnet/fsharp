open System.Threading.Tasks
open RuntimeAsync.Library

[<EntryPoint>]
let main _ =
    let fromTaskLike = (Api.addFromTaskAndValueTask (Task.FromResult 10) (ValueTask<int>(5))).Result
    let fromUnitTasks = (Api.bindUnitTaskAndUnitValueTask ()).Result
    let fromTryWith = (Api.safeDivide 10 0).Result
    let fromNested = (Api.nestedRuntimeTask ()).Result
    let fromDeepNested = (Api.deeplyNestedRuntimeTask ()).Result
    let fromOlderCE = (Api.consumeOlderTaskCE ()).Result
    let fromDelayYieldRun = (Api.taskDelayYieldAndRun ()).Result
    let fromAsyncDisposable = (Api.useAsyncDisposable ()).Result
    let fromAsyncEnumerable = (Api.iterateAsyncEnumerable ()).Result
    let fromConfigureAwait = (Api.configureAwaitExample ()).Result
    let fromInlineNested = (Api.inlineNestedRuntimeTask ()).Result

    printfn "Task<T> + ValueTask<T> -> %d" fromTaskLike
    printfn "Task + ValueTask -> %s" fromUnitTasks
    printfn "try/with -> %d" fromTryWith
    printfn "nested runtimeTask -> %d" fromNested
    printfn "deeply nested runtimeTask -> %d" fromDeepNested
    printfn "consume older task CE -> %d" fromOlderCE
    printfn "Task.Delay + Task.Yield + Task.Run -> %d" fromDelayYieldRun
    printfn "IAsyncDisposable -> %s" fromAsyncDisposable
    printfn "IAsyncEnumerable sum -> %d" fromAsyncEnumerable
    printfn "ConfigureAwait(false) -> %d" fromConfigureAwait
    printfn "inline-nested runtimeTask -> %d" fromInlineNested

    0
