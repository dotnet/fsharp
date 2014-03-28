// #Regression #TypeProvider #Methods #Inline
// This is regression test for DevDiv:358710 - [Type Providers] Calls of provided methods though member constrains and directly has inconsistent behavior
//<Expects status="success"></Expects>

// Note: the idea is that now for now we just throw.
// Eventually, in a more refined fix, we will emit an error message
// due to a mismatch in the return type and declared return type

open TPTest

try
    let r = Test.StaticMethod3()
    printfn "Test Failed: Method call did not throw"
    1
with
| :? System.InvalidCastException -> 0
| _ -> (printfn "Unexpected exception"; 1)

|> exit
