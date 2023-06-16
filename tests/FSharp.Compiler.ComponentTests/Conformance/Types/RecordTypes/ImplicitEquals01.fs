// #Regression #Conformance #TypesAndModules #Records 
// Implicitly implemented Equals should not throw
// Regression test for FSHARP1.0:1633
// Records
#light

type R = { A : string } 
type S = { B : int } 

let r = { A = "Hello" }
let s = { B = 10 }

let e = 
    try
       let _ = r.Equals(r)                // compare to itself
       let _ = r.Equals({A = "World"})    // compare to another record (same type)
       let _ = r.Equals(s)                // compare to another record (different type)
       let _ = r.Equals(10)               // compare to a different type [==> should not throw!]
       let _ = s.Equals(10)               // compare to a different type [==> should not throw!]
       0
    with
       | err -> printfn "FAIL: Equals threw! (%s)" err.Message
                1
if e <> 0 then failwith "Failed"

