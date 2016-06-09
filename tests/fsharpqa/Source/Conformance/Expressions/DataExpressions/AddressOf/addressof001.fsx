// #Conformance #DataExpressions #ReqNOMT 
// AddressOf Operator
// Verify we can use the & operator to invoke .Net method (byref)
// In this case we call into a C# method (ref)
//<Expects status="success"></Expects>
#light

#r @"addressof001dll.dll"

let mutable x = 10
let c = new C()
let u = c.M(&x)

exit (if (x=11) && (u=11) then 0 else 1)
