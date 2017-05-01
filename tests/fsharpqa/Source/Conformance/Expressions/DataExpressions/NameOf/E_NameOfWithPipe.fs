// #Regression #Conformance #DataExpressions 
// Verify that nameof can't be used as a function.
//<Expects id="FS3216" span="(6,28)" status="error">First-class uses of the 'nameof' operator is not permitted.</Expects>

let curriedFunction x y = x * y
let b = curriedFunction |> nameof
exit 0
