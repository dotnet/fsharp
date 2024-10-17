// #Regression #Conformance #TypeInference #ByRef 
// Verify you cannot set an object field to store a byref value



module ModuleFoo =
    let mutable x = 0
    let byrefVal = &x

    
