open System.Threading.Tasks
open RuntimeAsync.Library

[<EntryPoint>]
let main _ =
    let fromTaskLike = (Api.addFromTaskAndValueTask (Task.FromResult 10) (ValueTask<int>(5))).Result
    let fromUnitTasks = (Api.bindUnitTaskAndUnitValueTask ()).Result
    let fromTryWith = (Api.safeDivide 10 0).Result
    let fromNested = (Api.nestedRuntimeTask ()).Result

    printfn "Task<T> + ValueTask<T> -> %d" fromTaskLike
    printfn "Task + ValueTask -> %s" fromUnitTasks
    printfn "try/with -> %d" fromTryWith
    printfn "nested runtimeTask -> %d" fromNested

    0
