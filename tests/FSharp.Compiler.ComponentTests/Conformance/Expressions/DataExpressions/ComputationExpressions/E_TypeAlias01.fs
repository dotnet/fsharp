// #Regression #Conformance #DataExpressions #ComputationExpressions #ReqRetail 
// Dev11:207112
//<Expects status="error" id="FS0001" span="(10,11-10,16)">This expression was expected to have type.    ''a -> 'b'    .but here has type.    'AsyncBuilder'</Expects>
//<Expects status="error" id="FS0740" span="(10,17-10,29)">Invalid record, sequence or computation expression\. Sequence expressions should be of the form 'seq { \.\.\. }'</Expects>
let f x = async
let g x = x

let x = f "test" { return 1 } // ok
let y = f async { return 1 }  // also ok, same result
let z = g async { return 1 } 

let a = (g async) { return 1 } // ok
