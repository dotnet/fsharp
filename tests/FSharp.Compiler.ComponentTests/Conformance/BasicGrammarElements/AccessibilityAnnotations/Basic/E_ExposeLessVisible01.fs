// #Regression #Conformance #DeclarationElements #Accessibility 
//<Expects id="FS0410" status="error" span="(5,19)">The type 'A' is less accessible than the value, member or type 'x' it is used in</Expects>

type internal A = { x : int }
type public B = { x : A[] }
