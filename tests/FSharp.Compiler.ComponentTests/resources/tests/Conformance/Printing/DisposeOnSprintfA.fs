// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:4725
// F# is not disposing an IEnumerator in implementation of sprintf "%A"
//<Expects status="success">Done</Expects>
let x = seq { try yield 1; yield 2;  
              finally printfn "Done" }

sprintf "x = %A" x  |> ignore
