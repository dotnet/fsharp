// #Regression #Conformance #ObjectOrientedTypes #Enums 
#light
// Test errors related to enums of invalid primitive/built-in types


//<Expects id="FS0010" status="error">Unexpected keyword 'true' in union case</Expects>

type EnumOfBool = 
    | A = true
    | B = false

