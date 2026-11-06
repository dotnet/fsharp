module Test

open System

type Context = C0 of int[] | C1
let trace = ResizeArray<string>()
let expected = InvalidOperationException("guard")
let mutable throwAt = -2
let guard n (values: int[]) =
    trace.Add($"g{n}:{values[0]}")
    values[0] <- values[0] + 1
    if n = throwAt then raise expected
    n = 1 && throwAt = -2
let (|Observe|) (values: int[]) =
    trace.Add($"a:{values[0]}")
    values
let (|Read|_|) n (values: int[]) =
    trace.Add($"p{n}:{values[0]}")
    if values[0] >= n then Some values else None
let run token stack =
    try
        try
            match token, stack with
            | 0, C0 values :: tail when guard -1 values -> -100 + tail.Length
            | _, C0 ({{PATTERN}}) :: (_ :: _) when guard 0 values -> 100 + values[0]
            | _, C0 ({{PATTERN}}) :: (_ :: _) when guard 1 values -> 101 + values[0]
            | (1 | 2), _ :: tail when token >= 0 -> 201 + tail.Length
            | _ -> -1
        with error when (trace.Add("filter"); obj.ReferenceEquals(error, expected)) -> -2
    finally trace.Add("finally")
let check stop length expectedResult expectedValue expectedTrace =
    let values = [|1|]
    throwAt <- stop
    trace.Clear()
    let stack = if length = 1 then [C0 values] else [C0 values; C1]
    if run 2 stack <> expectedResult || values[0] <> expectedValue || List.ofSeq trace <> expectedTrace then
        failwith (String.concat "," trace)
[<EntryPoint>]
let main _ =
    let partialPattern = {{PARTIAL}}
    let observations value = if partialPattern then [$"p0:{value}"; $"a:{value}"] else []
    let prefix = if partialPattern then [] else ["a:1"]
    let traceThrough last =
        [ yield! prefix
          for n in 0..last do
              yield! observations (n + 1)
              yield $"g{n}:{n + 1}" ]
    let shortTrace = if partialPattern then ["p0:1"; "a:1"; "p0:1"; "a:1"] else ["a:1"]
    check -2 1 201 1 (shortTrace @ ["finally"])
    check -2 2 104 3 (traceThrough 1 @ ["finally"])
    check -1 2 202 3 (traceThrough 1 @ ["finally"])
    for stop in 0..1 do
        check stop 2 -2 (stop + 2) (traceThrough stop @ ["filter"; "finally"])
    0
