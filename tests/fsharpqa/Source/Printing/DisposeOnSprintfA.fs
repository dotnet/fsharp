// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:4725
// F# is not disposing an IEnumerator in implementation of sprintf "%A"
//<Expect status="success">Done</Expect>
let x = seq { try yield 1; yield 2;  
              finally printfn "Done" }

sprintf "x = %A" x

exit 0
