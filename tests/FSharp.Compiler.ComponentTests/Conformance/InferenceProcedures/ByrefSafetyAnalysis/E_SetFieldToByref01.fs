// #Regression #Conformance #TypeInference #ByRef 
#light

// Verify error when setting object field to a byref value. 
// (This is disallowed by the CLR.)




type Foo() =
    let mutable m_byrefOpt : byref<int> option = None
