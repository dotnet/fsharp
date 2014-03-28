// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
#light

// Verify error for invalid type extensions
//<Expects id="FS0912" status="error">This declaration element is not permitted in an augmentation</Expects>

type Foo =
    class
    end

// You can't add 'val' fields in type extension
type Foo with 
    val m_field : string


exit 1
