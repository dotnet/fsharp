// #Regression #Conformance #TypesAndModules #Unions 
// Implicitly implemented Equals should not throw
// Regression test for FSHARP1.0:1633
// Unions
#light

type M = 
  | A of string

let a = A("Hello")

let e = 
    try
       let _ = a.Equals(a)            // compare to itself
       let _ = a.Equals("")           // compare to another string			
       let _ = a.Equals(10)           // compare to a different type [==> should not throw!]
       0
    with
       | err -> printfn "FAIL: Equals threw! (%s)" err.Message
                1
if e <> 0 then failwith $"Failed: {e}"
