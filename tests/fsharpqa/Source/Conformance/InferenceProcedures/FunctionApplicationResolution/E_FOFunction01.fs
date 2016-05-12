// #Regression #Conformance #TypeInference #ApplicationExpressions 
// Verify error if applying a curried first order function where the parameters don't match
//<Expects id="FS0001" status="error" span="(8,32-8,37)">This expression was expected to have type.    'int'    .but here has type.    'string'</Expects>

let myFunction x (y : int) z = (x, y, z)

let curriedFunc  = myFunction 5
let curriedFunc2 = curriedFunc "foo"    // boom

