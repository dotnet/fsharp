// #Regression #Misc 
// Error 
//This runtime coercion or type test from type  'a  to   System.Exception involves an indeterminate type. Runtime type tests are not allowed on some types. Further type annotations are needed.
//<Expects id="FS0008" status="error">This runtime coercion or type test from type</Expects>

//383 conjPatternElements -> headBindingPattern AMP headBindingPattern 

let :? System.Exception = box(3)

// Should be compiler error
exit 1
