// #Regression #NoMT #FSI
// Regression for FSHARP1.0:6425, the exception used to keep throwing after every statement
// Note: we are happy with partial match on the error string, so we can run fine on LOC builds.
// <Expect status="success" id="FS1222">--noframework</Expects>
// <Expect status="success">val x : int = 1</Expect>
// <Expect status="success">val y : int = 2</Expect>

#r @"..\..\common\FSharp.Core.dll";;
let x = 1;;

let y = 2;;

exit 0;;
