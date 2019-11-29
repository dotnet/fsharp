// #Regression #Libraries #Operators 
// Regression test for FSHARP1.0:1247
// Precedence of type annotations :> and :?> over preceding expression forms, e.g. if-then-else etc.
//<Expects status="success"></Expects>

open System
let x = 2 :> Object
let y = [(2 :> Object)]
let z = [2 :> Object]

exit 0
