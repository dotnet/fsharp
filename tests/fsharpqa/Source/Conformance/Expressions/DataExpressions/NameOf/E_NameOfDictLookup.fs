// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on dictionary lookup
//<Expects id="FS3197" span="(6,9)" status="error">This expression does not have a name.</Expects>

let dict = new System.Collections.Generic.Dictionary<int,string>()
let b = nameof(dict.[2])

exit 0
