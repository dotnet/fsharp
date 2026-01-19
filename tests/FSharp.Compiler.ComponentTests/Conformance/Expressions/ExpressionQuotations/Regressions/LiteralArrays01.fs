// #Regression #Conformance #Quotations 
// DevDiv:188523

let a = <@ [| 2;3;4 |] @> 
let b = <@ [| 2u;3u;4u |] @> 
let c = <@ [| 2s;3s;4s |] @> 
let d = <@ [| 2UL;3UL;4UL |] @> 
let e = <@ [| 2us;3us;4us |] @> // previously threw internal error: unexpected expression shape
if e.ToString() <> "NewArray (UInt16, Value (2us), Value (3us), Value (4us))" then exit 1 else exit 0