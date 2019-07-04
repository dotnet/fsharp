// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on dictionary lookup
//<Expects id="FS3250" span="(6,16)" status="error">Expression does not have a name.</Expects>

let dict = new System.Collections.Generic.Dictionary<int,string>()
let b = nameof(dict.[2])

exit 0
