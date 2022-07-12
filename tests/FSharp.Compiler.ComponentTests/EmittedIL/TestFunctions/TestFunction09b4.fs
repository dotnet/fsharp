// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for FSHARP1.0:2484
// Note: we might see changes related to another bug
// related to spans
#light

[<Literal>]
let Null = null

let x = 5

match box x with                    // bp here
  | Null -> printfn "Is null"       // bp here
  | _ -> ()
