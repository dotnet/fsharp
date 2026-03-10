// #Regression #Diagnostics 
// Regression test for FSHARP1.0:5259
// Make sure we do not emit an error on "e"
// Instead we warn on 'TException
//<Expects status="success"></Expects>

#nowarn "0064"       // This construct causes code to be less generic than indicated by the type annotations. The type variable 'TException has been constrained to be type 'exn'

[<EntryPoint>]
let main(_) =
    try
       ()
    with 
    | :? 'TException as e -> 
          let msg = e.ToString()
          ()
    0
