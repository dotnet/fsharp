// #Regression #Conformance #DataExpressions 
// Verify that nameof can't be used as a function.
//<Expects id="FS3251" span="(6,28)" status="error">Using the 'nameof' operator as a first-class function value is not permitted.</Expects>

let curriedFunction x y = x * y
let b = curriedFunction |> nameof
exit 0
