// #Regression #Conformance #ControlFlow #Sequences 
// Regression for FSHARP1.0:5733
// For expressions should warn when elements will be skipped just like computation expressions do






for Some x in [Some 3; None] do
    ()
let s = [Some 3; None] :> seq<_>
for Some x in s do
    ()
for Some 1 in s do
    ()
for 1 in [1;2;3] do
    ()

// These warned prior to the fix and throw runtime exceptions
async {
    for Some(nm) in [ Some("James"); None; Some("John") ] do 
        printfn "%d" nm.Length 
} |> Async.RunSynchronously         
 
 
let s2 = seq { for Some(nm) in [ Some("James"); None; Some("John") ] do
                  yield nm.Length }
s2 |> Seq.iter (printfn "%A")
