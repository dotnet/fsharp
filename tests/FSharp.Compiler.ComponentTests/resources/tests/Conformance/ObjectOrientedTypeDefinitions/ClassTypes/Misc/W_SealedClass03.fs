// #Regression #Conformance #ObjectOrientedTypes #Classes 
// Test warnings associated with static upcasts to sealed types
//<Expects id="FS0064" status="warning">This construct causes code to be less generic than indicated by its type annotations\. The type variable implied by the use of a '#', '_' or other type annotation at or near</Expects>

type BaseClass() = 
    override this.ToString() = "Base Class"
    
[<Sealed>]
type SealedClass() =
    inherit BaseClass()
    override this.ToString() = "Sealed Class"

let f (x : #SealedClass) = ()    
    

// Should have compile warnings
exit 0


