module Test

open Microsoft.FSharp.Core.CompilerServices

type Cell = { mutable Value: int }
type Context = C0 of Cell | C1 | C2 | C3
let trace = ResizeArray<int>()
let mutable accepted = -2
let mutable alias = ListCollector<Context>()
let guard n cell =
    trace.Add n
    if n <= 0 then
        cell.Value <- 2
        alias.Add C1
        alias.Close() |> ignore
    n >= 0 && n = accepted
let run token stack cell =
    let mutable result = -1
    match token, stack with
    | 0, C0 cell :: _ when guard -1 cell -> result
    | _, C0 { Value = 1 } :: _ when guard 0 cell -> result <- 100 + cell.Value; result
    | _, C0 { Value = 1 } :: _ when guard 1 cell -> result <- 101 + cell.Value; result
    | _, C0 { Value = 1 } :: _ when guard 2 cell -> result <- 102 + cell.Value; result
    | _, C0 { Value = 1 } :: _ when guard 3 cell -> result <- 103 + cell.Value; result
    | _, C0 { Value = 1 } :: _ when guard 4 cell -> result <- 104 + cell.Value; result
    | _, C0 { Value = 1 } :: _ when guard 5 cell -> result <- 105 + cell.Value; result
    | _, C0 { Value = 1 } :: _ when guard 6 cell -> result <- 106 + cell.Value; result
    | _, C0 { Value = 1 } :: _ when guard 7 cell -> result <- 107 + cell.Value; result
    | 1, _ :: rest -> result <- 201 + rest.Length; result
    | 2, _ :: rest -> result <- 202 + rest.Length; result
    | 3, _ :: rest -> result <- 203 + rest.Length; result
    | (4 | 5), _ :: rest when token >= 0 -> result <- rest.Length; result
    | _ -> result
let makeStack cell =
    let mutable collector = ListCollector<Context>()
    collector.Add(C0 cell)
    alias <- collector
    collector.Close()
[<EntryPoint>]
let main _ =
    for token, wanted, expected in [(2, 1, 103); (2, -2, 203); (0, -2, -1)] do
        let cell = { Value = 1 }
        let stack = makeStack cell
        accepted <- wanted
        trace.Clear()
        if run token stack cell <> expected || cell.Value <> 2 || stack.Length <> 2 then
            failwith "mutation or tail recipe"
        let expectedTrace = if token = 0 then [-1] elif wanted = 1 then [0; 1] else [0..7]
        if List.ofSeq trace <> expectedTrace then failwith "guard order"
    0
