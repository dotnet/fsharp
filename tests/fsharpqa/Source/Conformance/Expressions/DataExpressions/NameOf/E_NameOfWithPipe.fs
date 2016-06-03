// #Regression #Conformance #DataExpressions 
// Verify that nameof can't be used as a function.
//<Expects id="FS3200" span="(26,10)" status="error">The nameof operator is not allowed in this position.</Expects>

let curriedFunction x y = x * y
let b = curriedFunction |> nameof
exit 0
