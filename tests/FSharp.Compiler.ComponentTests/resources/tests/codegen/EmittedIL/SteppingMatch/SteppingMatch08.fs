// #NoMono #NoMT #CodeGen #EmittedIL 
module SteppingMatch08 // Regression test for FSHARP1.0:5099
let test(x) =
    let b = 
        match x with
        | 0 -> 2
        | _   -> 0

    System.Diagnostics.Debug.Write(b)
    System.Diagnostics.Debug.Write(b)
