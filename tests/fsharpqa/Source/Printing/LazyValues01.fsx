// #Regression #NoMT #Printing  
// Regression test for FSharp1.0:3981 - Lazy<unit> gets NullReferenceException when displayed
// <Expects status="success">val a: Lazy<unit> = <unevaluated></Expects>
// <Expects status="success">val b: unit list = \[null\]</Expects>
// <Expects status="success">val c: unit \[\] = \[\|null; null; null\|]</Expects>
// <Expects status="success">val d: unit = \(\)</Expects>

let a = lazy()
let b = [ () ]
let c = [| (); (); () |]
let d = ()

;;

#q;;
