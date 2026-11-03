module Test
open Microsoft.FSharp.Core.CompilerServices
type Cell = { mutable Value: int }
type C = C0 of Cell | C1
let trace = ResizeArray<int>()
let mutable accepted = -2
let mutable alias = ListCollector<C>()
let guard n cell =
    trace.Add n
    if n <= 0 then
        cell.Value <- 2
        alias.Add C1
        alias.Close() |> ignore
    n = accepted
let run token stack cell =
    let mutable result = -1
    match token, stack with
    | 0, C0 cell :: _ when guard -1 cell -> result
    | _, C0 { Value = 1 } :: _ when guard 0 cell -> result <- 100 + cell.Value; result
    | _, C0 { Value = 1 } :: _ when guard 1 cell -> result <- 101 + cell.Value; result
    | (1 | 2), _ :: rest when token >= 0 -> result <- 202 + rest.Length; result
    | _ -> result
let mutable calls = 0
let (|Seen|) (_: int) = calls <- calls + 1; calls
let observe token input =
    match token, input with
    | 0, Seen v when (trace.Add 10; false) -> v
    | _, Seen v when (trace.Add 20; false) -> v
    | (1 | 2), _ -> -1
    | _, Seen v -> v
type U = First of string | Second
let (|Probe|) = function First s -> s.Length | _ -> 0
let demand take token stack =
    match token, stack with
    | 0, First _ :: _ when take -> 10
    | _, (Probe 0 & Second) :: _ when take -> 11
    | (1 | 2), _ :: _ when take -> 12
    | _ -> 24
let projection gate (input: (int * int) * bool) =
    match input with
    | _, true when gate -> 1
    | ((1, _) | (2, _)), true when gate -> 2
    | _ -> 10
let nullDefault take token stack =
    match token, stack with
    | First _, 1 :: _ when take -> 10
    | _, _ :: _ when not take -> 11
    | Second, _ :: _ when not take -> 12
    | _ -> 24
[<EntryPoint>]
let main _ =
    for token, wanted, expected, guards in [2, 1, 103, [0; 1]; 2, -2, 203, [0; 1]; 0, -2, -1, [-1]] do
        let cell = { Value = 1 }
        let mutable collector = ListCollector<C>()
        collector.Add(C0 cell)
        alias <- collector
        let stack = collector.Close()
        accepted <- wanted
        trace.Clear()
        if run token stack cell <> expected || cell.Value <> 2 || stack.Length <> 2 || List.ofSeq trace <> guards then
            failwith "guard order or aliased mutation"
    trace.Clear()
    if observe 0 42 <> 1 || calls <> 1 || List.ofSeq trace <> [10; 20] then failwith "total active observation"
    if demand false 0 [First Unchecked.defaultof<string>] <> 24 then failwith "refuted active demand"
    if demand true 0 [Unchecked.defaultof<U>] <> 10 then failwith "null head demand"
    if nullDefault true Unchecked.defaultof<U> [1] <> 10 then failwith "null token demand"
    if projection false (Unchecked.defaultof<int * int>, false) <> 10 then failwith "projection demand"
    0
