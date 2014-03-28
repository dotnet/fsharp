// #Regression #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Negative tests on :>
// Using _ as left argument

//<Expects status="error" span="(13,18-13,19)" id="FS0010">Unexpected symbol '_' in expression$</Expects>
//<Expects status="error" span="(13,9-13,10)" id="FS0583">Unmatched '\('$</Expects>
//<Expects status="error" span="(14,11-14,12)" id="FS0010">Unexpected symbol '_' in binding$</Expects>
//<Expects status="error" span="(14,9-14,10)" id="FS0583">Unmatched '\('$</Expects>
//<Expects status="error" span="(15,11-15,12)" id="FS0010">Unexpected symbol '_' in binding$</Expects>
//<Expects status="error" span="(15,9-15,10)" id="FS0583">Unmatched '\('$</Expects>


let a = ( upcast _ ) : obj
let b = ( _ :> _ ) : obj
let c = ( _ :> obj)
