// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:5231
// 
//<Expects status="success"></Expects>
let s = [1 .. 2]
let q = [s; s; s]
let a = array2D q;;

let v = a.[1,1] = 2;;

if not (v) then raise (new System.Exception($"Found '{a[1,1]}' instead of '2'"))
