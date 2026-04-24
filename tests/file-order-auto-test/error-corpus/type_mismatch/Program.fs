module ErrorCorpus.TypeMismatch

[<EntryPoint>]
let main _argv =
    // ERROR: passing string where int expected
    let x : int = "not an int"
    printfn "%d" x
    0
