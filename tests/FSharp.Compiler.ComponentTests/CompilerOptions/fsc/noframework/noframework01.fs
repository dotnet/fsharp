// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:5976

// System.Func<...> is in System.Core.dll (NetFx3.5)
module noframework01

let f ( d : System.Func<int> ) = d.Invoke() + 1

let result = f ( new System.Func<_>(fun _ -> 10) )
if result <> 11 then
    failwith "Expected 11"

printfn "Test passed"

