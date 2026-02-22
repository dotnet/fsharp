// #Regression #Conformance #ObjectOrientedTypes #Classes 
// Test warnings associated with static upcasts to sealed types
//<Expects id="FS0064" status="warning">This construct causes code to be less generic than indicated by its type annotations.</Expects>

type BaseClass() = 
    override this.ToString() = "Base Class"
    
[<Sealed>]
type SealedClass() =
    inherit BaseClass()
    override this.ToString() = "Sealed Class"

let f (x : 'a when 'a :> SealedClass) = ()    
    

// Should have compile warnings
exit 0
