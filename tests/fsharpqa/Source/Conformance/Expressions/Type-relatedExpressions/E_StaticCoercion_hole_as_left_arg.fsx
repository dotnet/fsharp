// #Regression #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Negative tests on :>
// Using _ as left argument

//<Expects status="error" span="(13,20-13,21)" id="FS0010">Unexpected symbol ')' in expression. Expected '.' or other token.$</Expects>
//<Expects status="error" span="(14,13-14,14)" id="FS0010">Unexpected symbol ':>' in expression. Expected '.' or other token.$</Expects>
//<Expects status="error" span="(14,9-14,10)" id="FS0583">Unmatched '\('$</Expects>
//<Expects status="error" span="(15,13-15,14)" id="FS0010">Unexpected symbol ':>' in expression. Expected '.' or other token.$</Expects>
//<Expects status="error" span="(15,9-15,10)" id="FS0583">Unmatched '\('$</Expects>


let a = ( upcast _ ) : obj
let b = ( _ :> _ ) : obj
let c = ( _ :> obj)
