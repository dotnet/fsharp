// #Regression #Conformance #TypeInference #ByRef 
// Verify appropriate error if attempting to assign a ByRef value to an
// object field. (Disallowed by the CLR.)




let mutable mutableObjectField : obj = null

// Set mutableObjectField to a byref (cast to Object)
let someFunction () =
    let mutable x = 0
    // Attempt to cast byref<int> to object, ERROR
    mutableObjectField <- (&x :> obj)
    ()

let someOtherFunction() =
    // Attempt to cast object to byref<int>, ERROR
    let byrefVar = mutableObjectField :?> byref<int>
    let valueInByrefSquared = byrefVar * byrefVar
    ()
    
// This file shouldn't compile
exit 1
