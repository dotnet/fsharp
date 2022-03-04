// #Regression #NoMT #Printing #RequiresENU  
// Regression test for FSharp1.0:3981 - Lazy<unit> gets NullReferenceException when displayed
// <Expects status="success">val a: Lazy<unit> = Value is not created</Expects>
// <Expects status="success">val b: unit list = \[\(\)\]</Expects>
// <Expects status="success">val c: unit\[\] = \[\|\(\); \(\); \(\)\|]</Expects>
// <Expects status="success">val d: unit = \(\)</Expects>

let a = lazy()
let b = [ () ]
let c = [| (); (); () |]
let d = ()

;;

#q;;
