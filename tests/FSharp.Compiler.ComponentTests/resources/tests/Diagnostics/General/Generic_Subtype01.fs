// #Regression #Diagnostics 
// Regression test for FSHARP1.0:4579
// The following code should not give any error/warning
//<Expects status="success"></Expects>
module M
let fromSeq (s: #seq<'schema>) =     // <--- used to give a false warning that can't be eliminated
        s |> Array.ofSeq
