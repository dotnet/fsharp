#if REFLECTED_TOP_MODULE
[<ReflectedDefinition>]
#endif
module Test

open Microsoft.FSharp.Quotations

type Context =
    | C0 of int | C1 of int | C2 of int | C3 of int
    | C4 of int | C5 of int | C6 of int | C7 of int
    | C8 of int | C9 of int | C10 of int | C11 of int
    | C12 of int | C13 of int | C14 of int | C15 of int

let trace = ResizeArray<int>()
let guard n value col = trace.Add n; col <= value + n

#if IL_BOUND
let run token stack col =
            let mutable result = -1
#elif BYREF
let run (data: int[]) token stack col : byref<int> =
#elif RETHROW
let expected = System.Exception("identity")
let run token stack col : int =
    try raise expected with _ ->
#elif PROTECTED
type Holder() =
    member this.Run(token, stack, col) =
#elif TAIL
let run token stack col =
    let rec loop remaining result =
        if remaining = 0 then result else
#elif TASK
let run token stack col =
    task {
        do! System.Threading.Tasks.Task.Yield()
        return (
#elif RUNTIME
open System.Threading.Tasks
let run (ready: Task) token stack col : Task<int> =
    Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers.__runtimeAsyncReturn (
#elif EXPLICIT_QUOTE
let q =
    <@
        fun token stack col ->
#elif REFLECTED_ARGUMENT
type Sink = static member Capture([<ReflectedDefinition>] value: Expr<int -> Context list -> int -> int>) = value
let q =
    Sink.Capture(
        fun token stack col ->
#elif REFLECTED_FUNCTION
[<ReflectedDefinition>]
let run token stack col =
#elif REFLECTED_TYPE
[<ReflectedDefinition>]
type Holder =
    static member Run(token, stack, col) =
#elif REFLECTED_MODULE
[<ReflectedDefinition>]
module Holder =
    let run token stack col =
#elif LINQ
type Sink = static member Capture(value: System.Linq.Expressions.Expression<System.Func<int, Context list, int, int>>) = value
let q =
    Sink.Capture(
        fun token stack col ->
#elif LINQ_OVERLOAD
type Sink =
    static member Capture(value: System.Linq.Expressions.Expression<System.Func<int, Context list, int, int>>) = value
    static member Capture(value: string) = value
let q =
    Sink.Capture(
        fun token stack col ->
#elif AUTO_QUOTE
type Builder() =
    member _.Return(value) = value
    member _.Delay(f) = f()
    member _.Quote(value: Expr<'T>) = value
    member _.Run(value: Expr<'T>) = value
let q =
    (Builder() {
        return (fun token stack col ->
#elif REFLECTED_TOP_MODULE
let run token stack col =
#elif REFLECTED_CONSTRUCTOR
type Holder [<ReflectedDefinition>](token: int, stack: Context list, col: int) =
    let result =
#endif
            // MATCH
#if IL_BOUND
[<EntryPoint>]
let main _ =
    for token in 1..16 do
        if run token [C0 0] 1000 <> token then failwith "completed tail"
    if trace.Count <> 16 then failwith ("guard count: " + string trace.Count)
    if run 5 [C15 0] 0 <> 115 then failwith "guard success"
    if run 5 [] 1000 <> -1 then failwith "empty list"
    0
#elif BYREF
[<EntryPoint>]
let main _ =
    let data = Array.zeroCreate<int> 128
    (run data 5 [C15 0] 0) <- 99
    (run data 5 [C0 0] 1000) <- 77
    if data[115] <> 99 || data[5] <> 77 then failwith "byref target"
    0
#elif RETHROW
[<EntryPoint>]
let main _ =
    for stack in [ [C15 0]; [] ] do
        try
            run 5 stack 0 |> ignore
            failwith "missing rethrow"
        with error when obj.ReferenceEquals(error, expected) -> ()
    0
#elif PROTECTED
[<EntryPoint>]
let main _ =
    let holder = Holder()
    if holder.Run(5, [C15 0], 0) <> 115 || holder.Run(5, [C0 0], 1000) <> 5 then failwith "protected target"
    0
#elif TAIL
    loop 100000 0
[<EntryPoint>]
let main _ =
    if run 5 [C15 0] 0 <> 115 || run 5 [C0 0] 1000 <> 5 then failwith "tail target"
    0
#elif TASK
        )
    }
[<EntryPoint>]
let main _ =
    if (run 5 [C15 0] 0).Result <> 115 || (run 5 [] 0).Result <> -1 then
        failwith "task target"
    0
#elif RUNTIME
    )
[<EntryPoint>]
let main _ =
    let ready = TaskCompletionSource<unit>()
    let work = run ready.Task 5 [C15 0] 0
    if work.IsCompleted then failwith "missing suspension"
    ready.SetResult(())
    if work.GetAwaiter().GetResult() <> 115 || (run Task.CompletedTask 5 [] 0).GetAwaiter().GetResult() <> -1 then
        failwith "runtime async target"
    0
#elif EXPLICIT_QUOTE
    @> :> Expr
#elif REFLECTED_ARGUMENT
    ) :> Expr
#elif REFLECTED_FUNCTION || REFLECTED_TOP_MODULE
let q = Expr.TryGetReflectedDefinition(typeof<Context>.DeclaringType.GetMethod("run")).Value
#elif REFLECTED_TYPE
let q = Expr.TryGetReflectedDefinition(typeof<Holder>.GetMethod("Run")).Value
#elif REFLECTED_MODULE
let q = Expr.TryGetReflectedDefinition(typeof<Context>.DeclaringType.GetNestedType("Holder").GetMethod("run")).Value
#elif LINQ || LINQ_OVERLOAD
    )
#elif AUTO_QUOTE
        )
    }) :> Expr
#elif REFLECTED_CONSTRUCTOR
    member _.Result = result
let q = Expr.TryGetReflectedDefinition(typeof<Holder>.GetConstructors()[0]).Value
#endif
