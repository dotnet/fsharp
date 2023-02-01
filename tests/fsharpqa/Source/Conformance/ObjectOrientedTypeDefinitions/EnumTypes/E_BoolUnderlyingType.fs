// #Regression #Conformance #ObjectOrientedTypes #Enums 
#light
// Test errors related to enums of invalid primitive/built-in types


//<Expects id="FS0951" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>

type EnumOfBool = 
    | A = true
    | B = false

