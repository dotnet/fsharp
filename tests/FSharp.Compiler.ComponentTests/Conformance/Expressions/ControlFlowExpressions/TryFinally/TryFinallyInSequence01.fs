// #Regression #Sequences #Conformance #ControlFlow #Exceptions 
// Test issue 4234. The behavior here differs from the equivalent C# but that's ok. Just make sure if it changes we notice.

let mutable result = ""
let fail() = result <- result + "fail "; failwith "err"
let print() = result <- result + "finally "
let check e a =
    if e <> a then
        printfn "Expected:\n%A\nbut got:\n%A" e a
        exit 1
let s = seq { try yield 1; fail() finally print() }

let Example1 (s:seq<int>) =
    let e1 = s.GetEnumerator()
    result <- result + "BeforeMoveNext1 "
    e1.MoveNext() |> ignore
    result <- result + "BeforeMoveNext2 "
    try e1.MoveNext() |> ignore
    with ex -> result <- result + "caught "
    result <- result + "BeforeDispose "
    e1.Dispose()
    result <- result + "done"

let Example2 (s:seq<int>) =
    let e1 = s.GetEnumerator()
    result <- result + "BeforeMoveNext1 "
    e1.MoveNext() |> ignore
    result <- result + "BeforeDispose "
    e1.Dispose()
    result <- result + "done"

Example1 s
check "BeforeMoveNext1 BeforeMoveNext2 fail caught BeforeDispose finally done" result

result <- ""

Example2 s
check "BeforeMoveNext1 BeforeDispose finally done" result

exit 0