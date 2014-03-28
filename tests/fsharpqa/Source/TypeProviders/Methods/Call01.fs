// #Regression #TypeProvider #Methods #Inline
// This is regression test for DevDiv:358710 - [Type Providers] Calls of provided methods though member constrains and directly has inconsistent behavior
//<Expects status="success"></Expects>

open TPTest

let inline f< ^a when ^a : (static member StaticMethod4 : int -> int)> x = (^a : (static member StaticMethod4 : int -> int) x)
//let inline f< ^ty1 when ^ty1 : (static member StaticMethod1 : int -> string)> x = (^ty1 : (static member StaticMethod1 : int -> string) x)
// let inline f< ^ty1 when ^ty1 : (static member StaticMethod1 : int -> string)> x = (^ty1 : (static member StaticMethod1 : int -> string) x)
// internal error: unexpected call to F# value
//let e2 = f<Test> 5

//let m1 = Test.StaticMethod1 5
// let m2 = Test.StaticMethod2 5
//let m3 = Test.StaticMethod3()
//let m4 = Test.StaticMethod4 5

let r = Test.StaticMethod4 5
let r2 = f<Test> 55

exit <| if r <> 5 || r2 <> 55 then 1 else 0


