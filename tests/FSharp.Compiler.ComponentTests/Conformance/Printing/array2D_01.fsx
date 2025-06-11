// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:5231
// 
//<Expects status="success">val s: int list = \[1; 2\]</Expects>
//<Expects status="success">val q: int list list = \[\[1; 2\]; \[1; 2\]; \[1; 2\]\]</Expects>
//<Expects status="success">val a: int\[,\] = \[\[1; 2\]</Expects>
//<Expects status="success">                 \[1; 2\]</Expects>
//<Expects status="success">                 \[1; 2\]\]</Expects>
//<Expects status="success">val v: bool = false</Expects>

let s = [1 .. 2]
let q = [s; s; s]
let a = array2D q;;

let v = a.[1,1] = 1;;
if v then raise (new System.Exception($"Found '{a[1,1]}' instead of '2'"))
