// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing
// Regression test for FSHARP1.0:5835
//<Expects status="success"></Expects>
module M

let t1 = (1, "a")
let t2 = (1, "a", (fun x -> x + 1))

try
    let v = sprintf "%A" (System.Object.Equals(t1,t2))
    match (v = bool.FalseString) with
    | false -> 0
    | x -> (printfn "Unexpected value '%A'" x); 1
with
| _ -> (printfn "Exception caught - did we regress?"); 1

|> exit
