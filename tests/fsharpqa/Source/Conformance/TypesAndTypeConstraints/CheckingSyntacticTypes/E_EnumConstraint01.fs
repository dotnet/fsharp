// #Regression #Conformance #TypeConstraints 
#light

// Verify error if trying to use a failing enum type constraint
//<Expects id="FS0001" status="error">The type 'byte' does not match the type 'int16'</Expects>

let printByteEnum (e : 'a) : unit when 'a : enum<byte> = ()

type ShortEnum = 
    | A = 0s
    | B = 1s

printByteEnum (ShortEnum.A)

exit 1
    
