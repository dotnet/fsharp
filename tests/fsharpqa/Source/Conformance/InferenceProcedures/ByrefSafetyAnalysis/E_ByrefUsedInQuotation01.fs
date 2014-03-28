// #Regression #Conformance #TypeInference #ByRef 
// Attempt to use a byref in a quotation
//<Expects id="FS0412" span="(11,1-11,14)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>
//<Expects id="FS0421" span="(11,10-11,11)" status="error">The address of the variable 'i' cannot be used at this point$</Expects>
//<Expects id="FS0412" span="(11,6-11,14)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>

open Microsoft.FSharp.Quotations

let test a = ();
let mutable i = 5 in
test <@ &i @>
