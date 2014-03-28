// #Conformance #ObjectOrientedTypes #Enums 
#light

// FSB 937, enums on not int32 types? e.g. int64.

type EnumOfLongs =
    | A = 0L
    | B = 0L
    
type EnumOfULongs =
    | A = 0UL
    | B = 0UL

type EnumOfUInt = 
    | A = 0u
    | B = 0u

type EnumOfInt = 
    | A = 0
    | B = 0

type EnumOfHex =
    | A = 0x00
    | B = 0xFF

type EnumOfOctal =
    | A = 0o00
    | B = 0o77
    
type EnumOfBit =
    | A = 0b0000
    | B = 0b1111
    
type EnumOfUShort = 
    | A = 0us
    | B = 0us

type EnumOfShort =
    | A = 0s
    | B = 0s

type EnumOfByte =
    | A = 0y
    | B = 0y

type EnumOfUByte =
    | A = 0uy
    | B = 0uy

type EnumOfChar =
    | A = 'a'
    | B = 'a'
    | C = 'A'
    

// This test just ensures we can define these non-int32 enums
exit 0
