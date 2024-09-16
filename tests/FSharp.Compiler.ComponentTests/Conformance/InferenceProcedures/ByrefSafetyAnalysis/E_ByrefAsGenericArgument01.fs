// #Regression #Conformance #TypeInference #ByRef 
#light

// Verify error when trying to use a byref<_> as generic argument.
// (Disallowed by CLR.)



let test : byref<int> list = []
