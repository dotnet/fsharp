// #Regression #TypeProvider #Methods #StaticMemberConstraints
// This is regression test for DevDiv:356043 - [FSharp] Internal error when using static member constraints on provided types
//<Expects status="success"></Expects>


open TPTest

let inline f< ^a when ^a : (static member StaticMethod : int -> string)> x = (^a : (static member StaticMethod : int -> string) x)

// This used to give "internal error: unexpected call to F# value"
// Now it should be fine.
let e1 = f<Test> 0

// This one used to work before and should still work now
let e2 = Test.StaticMethod 0

exit <| if e1 = "0" && e2 = "0" then 0 else 1


