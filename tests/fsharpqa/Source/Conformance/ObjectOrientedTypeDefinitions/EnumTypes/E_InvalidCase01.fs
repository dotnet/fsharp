// #Regression #Conformance #ObjectOrientedTypes #Enums 
#light

// FSB 1744, 'value__' is reserved and cannot be a name of enum element
//<Expects id="FS0745" status="error">This is not a valid name for an enumeration case</Expects>

type X =
    | value__ = 1

// Shouldn't even compile
exit 1
