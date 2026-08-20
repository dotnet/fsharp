module RuntimeAsyncBasic

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let private delayed value =
    Task.Delay(1).ContinueWith(fun (_: Task) -> value)

let add (x: int) (y: int) : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        let first = AsyncHelpers.Await(delayed x)
        first + y)

let lambdaAdd : int -> Task<int> =
    fun value ->
        StateMachineHelpers.__runtimeAsyncReturn (
            let result = AsyncHelpers.Await(delayed value)
            result + 1)

let makeAdder (offset: int) : int -> Task<int> =
    fun value ->
        StateMachineHelpers.__runtimeAsyncReturn (
            let result = AsyncHelpers.Await(delayed value)
            result + offset)

let inline apply ([<InlineIfLambda>] operation: int -> int) (value: int) =
    operation value

let inline awaitAndAdd (value: int) =
    let result =
        AsyncHelpers.Await(Task.Delay(1).ContinueWith(fun (_: Task) -> value))

    apply (fun current -> current + 1) result

let addWithInline (value: int) : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (awaitAndAdd value)

type Calculator() =
    member _.Add(x: int, y: int) : Task<int> =
        StateMachineHelpers.__runtimeAsyncReturn (
            let first = AsyncHelpers.Await(delayed x)
            first + y)

    static member Double(value: int) : Task<int> =
        StateMachineHelpers.__runtimeAsyncReturn (value * 2)

let private resultOf (task: Task<int>) =
    task.GetAwaiter().GetResult()

[<EntryPoint>]
let main _ =
    let calculator = Calculator()
    let capturedAdder = makeAdder 10

    let results =
        [
            add 20 22
            lambdaAdd 41
            capturedAdder 32
            addWithInline 41
            calculator.Add(20, 22)
            Calculator.Double 21
        ]
        |> List.map resultOf

    if results = [ 42; 42; 42; 42; 42; 42 ] then 0 else 1
