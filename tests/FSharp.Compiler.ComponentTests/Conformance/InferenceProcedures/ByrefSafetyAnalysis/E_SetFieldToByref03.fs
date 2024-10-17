// #Regression #Conformance #TypeInference #ByRef 
// Verify appropriate error if attempting to assign a ByRef value to an
// object field. (Disallowed by the CLR.)




type RecordWithByref = {A : byref<int> }


