// #Regression #Conformance #TypeInference #ByRef 
// Verify appropriate error if attempting to assign a ByRef value to an
// object field. (Disallowed by the CLR.)

//<Expects id="FS0412" span="(14,28-14,37)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>
//<Expects id="FS0421" span="(14,29-14,30)" status="error">The address of the variable 'x' cannot be used at this point$</Expects>
//<Expects id="FS0412" span="(19,20-19,53)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>
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
