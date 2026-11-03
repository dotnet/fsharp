module Test

let trace = ResizeArray<int>()
let guard n = trace.Add n; false
type Cell = { mutable Value: int }
let mutate cell = cell.Value <- 2; guard 2
let record token cell =
    match token, cell with
    | 0, { Value = 1 } when guard 1 -> 0
    | _, { Value = 1 } when mutate cell -> 1
    | ({{ALTERNATIVES}}), _ -> 3
    | _, { Value = 1 } -> 4
    | _ -> 5
let mutable calls = 0
let (|Seen|) (input: int) =
    calls <- calls + 1
    trace.Add calls
    calls
let active token input =
    match token, input with
    | 0, Seen value when guard 10 -> value
    | _, Seen value when guard 20 -> value
    | ({{ALTERNATIVES}}), _ -> -1
    | _, Seen value -> value
let demand (input: (int * int) * bool) =
    match input with
{{DEMAND_GUARDS}}
    | (0, _), true -> -1
    | (3, _), true -> -1
    | (4, _), true -> -1
    | ((1, _) | (2, _)), true when guard 9 -> 9
    | _ -> 10
type U = A | B
type Payload = P of int | Q
type Pair = { mutable Left: Payload; mutable Right: Payload }
let reread cell =
    match cell with
    | { Value = value } when (cell.Value <- 2; true) -> value
    | _ -> -1
let saved pair =
    match pair with
    | { Left = P x; Right = P y } when (pair.Left <- Q; pair.Right <- Q; true) -> x + y
    | _ -> -1
[<CLIMutable>]
type OpenCell = { Item: Payload }
let cliMutate (cell: OpenCell) =
    typeof<OpenCell>.GetProperty("Item").SetValue(box cell, box Q)
    true
let cli token cell =
    let mutable result = 43
    match token, cell with
    | 0, _ -> result
    | _, { Item = P value } when cliMutate cell ->
        result <- value
        result
    | (1 | 2 | 3 | 4), _ when token >= 0 ->
        result <- 42
        result
    | _ -> result
type SmallToken = First of int | Second of int
let nullDefault takeFirst token (stack: int list) =
    match token, stack with
    | First _, 1 :: _ when takeFirst -> 10
{{NULL_DEFAULT_GUARDS}}
    | Second _, _ :: _ when not takeFirst -> 20
    | Second _, _ :: _ when not takeFirst -> 21
    | Second _, _ :: _ when not takeFirst -> 22
    | (First _ | Second _), _ :: _ when not takeFirst -> 23
    | _ -> 24
type Token = T0 | T1 | T2 | T3
let nullHead takeFirst token stack =
    match token, stack with
    | T0, First _ :: _ when takeFirst -> 10
{{NULL_HEAD_GUARDS}}
    | T1, _ :: _ when not takeFirst -> 20
    | T2, _ :: _ when not takeFirst -> 21
    | T3, _ :: _ when not takeFirst -> 22
    | (T1 | T2 | T3), _ :: _ when not takeFirst -> 23
    | _ -> 24
type Head = H0 of string | H1 of int | H2 | H3
let (|Probe|) head = match head with H0 text -> text.Length | _ -> 0
let refutedActive takeFirst token stack =
    match token, stack with
    | T0, H0 _ :: _ when takeFirst -> 10
    | _, (Probe 0 & H1 _) :: _ when takeFirst -> 11
{{REFUTED_GUARDS}}
    | T1, _ :: _ when takeFirst -> 20
    | T2, _ :: _ when takeFirst -> 21
    | T3, _ :: _ when takeFirst -> 22
    | (T1 | T2 | T3), _ :: _ when takeFirst -> 23
    | _ -> 24
let ignored flag (u: U) n =
    match u, n with
    | _, 0 -> 0
    | _ when (trace.Add 30; flag) -> 1
    | A, 1 -> 2
    | _ -> 3
[<EntryPoint>]
let main _ =
    let cell = { Value = 1 }
    if record 0 cell <> 4 || cell.Value <> 2 || List.ofSeq trace <> [1; 2] then
        failwith "record observation"
    trace.Clear()
    if active 0 42 <> 1 || calls <> 1 || List.ofSeq trace <> [1; 10; 20] then
        failwith "active observation"
    trace.Clear()
    if demand (Unchecked.defaultof<int * int>, false) <> 10 || trace.Count <> 0 then
        failwith "projection demand"
    if ignored false Unchecked.defaultof<U> 2 <> 3 || List.ofSeq trace <> [30] then
        failwith "refuted column demand"
    if reread { Value = 1 } <> 2 || saved { Left = P 1; Right = P 2 } <> 3 then
        failwith "success argument observation"
    if cli 2 { Item = P 7 } <> 7 then failwith "CLR-settable parent"
    if nullDefault true Unchecked.defaultof<SmallToken> [1] <> 10 then failwith "exhaustive null default"
    if nullHead true T0 [Unchecked.defaultof<SmallToken>] <> 10 then failwith "exhaustive head null default"
    if refutedActive false T0 [H0 Unchecked.defaultof<string>] <> 24 then failwith "refuted active demand"
    0
