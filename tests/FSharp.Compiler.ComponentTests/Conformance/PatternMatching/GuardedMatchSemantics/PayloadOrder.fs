module Test

open System

type Context = C0 of int[] | C1 | C2 | C3
let trace = ResizeArray<string>()
let expected = InvalidOperationException("guard")
let mutable throwAt = -2
let guard n (values: int[]) =
    trace.Add($"g{n}:{values[0]}")
    values[0] <- values[0] + 1
    if n = throwAt then raise expected
    n = 6 && throwAt = -2
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
            | _, C0 __PAYLOAD__ :: (_ :: _) when guard 0 values -> 100 + values[0]
            | _, C0 __PAYLOAD__ :: (_ :: _) when guard 1 values -> 101 + values[0]
            | _, C0 __PAYLOAD__ :: (_ :: _) when guard 2 values -> 102 + values[0]
            | _, C0 __PAYLOAD__ :: (_ :: _) when guard 3 values -> 103 + values[0]
            | _, C0 __PAYLOAD__ :: (_ :: _) when guard 4 values -> 104 + values[0]
            | _, C0 __PAYLOAD__ :: (_ :: _) when guard 5 values -> 105 + values[0]
            | _, C0 __PAYLOAD__ :: (_ :: _) when guard 6 values -> 106 + values[0]
            | _, C0 __PAYLOAD__ :: (_ :: _) when guard 7 values -> 107 + values[0]
            | 1, _ :: tail -> 201 + tail.Length
            | 2, _ :: tail -> 202 + tail.Length
            | 3, _ :: tail -> 203 + tail.Length
            | (4 | 5), _ :: tail when token >= 0 -> 204 + tail.Length
            | _ -> -1
        with error when (trace.Add("filter"); obj.ReferenceEquals(error, expected)) -> -2
    finally trace.Add("finally")
let check stop length expectedResult expectedValue expectedTrace =
    let values = [|1|]
    throwAt <- stop
    trace.Clear()
    let stack = if length = 1 then [C0 values] else [C0 values; C3]
    if run 2 stack <> expectedResult || values[0] <> expectedValue || List.ofSeq trace <> expectedTrace then
        failwith (String.concat "," trace)
[<EntryPoint>]
let main _ =
#if PARTIAL
    let partialPattern = true
#else
    let partialPattern = false
#endif
    let observations value = if partialPattern then [$"p0:{value}"; $"a:{value}"] else []
    let prefix = if partialPattern then [] else ["a:1"]
    let traceThrough last =
        [ yield! prefix
          for n in 0..last do
              yield! observations (n + 1)
              yield $"g{n}:{n + 1}" ]
    let shortTrace = if partialPattern then List.replicate 8 ["p0:1"; "a:1"] |> List.concat else ["a:1"]
    check -2 1 202 1 (shortTrace @ ["finally"])
    check -2 2 114 8 (traceThrough 6 @ ["finally"])
    check -1 2 203 9 (traceThrough 7 @ ["finally"])
    for stop in 0..7 do
        check stop 2 -2 (stop + 2) (traceThrough stop @ ["filter"; "finally"])
    0
