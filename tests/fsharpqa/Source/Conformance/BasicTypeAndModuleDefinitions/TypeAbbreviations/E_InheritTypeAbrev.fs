// #Regression #Conformance #TypesAndModules 
// Verify error when trying to inherit from type abbreviations.
//<Expects id="FS0945" status="error">Cannot inherit a sealed type</Expects>

type Type1 = A | B
type Type2 = Type1 * int
type Type3 = 
    class
        inherit Type2
    end

exit 1
