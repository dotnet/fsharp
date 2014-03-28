// #Regression #TypeProvider #Methods
// Regression test for 705149
//<Expects status="success"></Expects>

open TPTest

let r = Test.StaticMethod5 ()
printfn "Expecting null.  Actual: %A" r

exit <| if r <> null then 1 else 0


