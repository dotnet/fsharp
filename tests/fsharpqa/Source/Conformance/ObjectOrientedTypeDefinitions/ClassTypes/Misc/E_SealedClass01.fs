// #Regression #Conformance #ObjectOrientedTypes #Classes 
#light

// FSB 1502, Implement a "sealed" attribute
//<Expects id="FS0945" status="error">Cannot inherit a sealed type</Expects>

type BaseClass() = 
    override this.ToString() = "Base Class"
    
[<Sealed>]
type SealedClass() =
    inherit BaseClass()
    override this.ToString() = "Sealed Class"
    
type DerivedClass() =
    inherit SealedClass()
    override this.ToString() = "Derived Class"
    

// Should have compile errors
exit 1
