// #Regression #Conformance #TypeInference #ByRef 
#light

// Verify error when trying to use a byref<_> as generic argument.
// (Disallowed by CLR.)

//<Expects id="FS0412" span="(9,30-9,32)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>

let test : byref<int> list = []
