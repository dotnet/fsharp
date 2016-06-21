// #Conformance #DataExpressions #ReqNOMT 
// AddressOf Operator
// Verify we can use the & operator to invoke .Net method (byref)
// In this case we call into a C# method (out)
//<Expects status="success"></Expects>
#light

#r @"addressof002dll.dll"

let mutable x = 0
let c = new C()
let u = c.M(&x)

exit (if (x=11) && (u=11) then 0 else 1)
