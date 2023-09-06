// #Regression #Conformance #ObjectOrientedTypes #Enums 
#light
// Test errors related to enums of invalid primitive/built-in types


//<Expects id="FS0951" span="(19,5-20,16)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
//<Expects id="FS0886" span="(23,11-23,13)" status="error">This is not a valid value for an enumeration literal</Expects>
//<Expects id="FS0886" span="(27,11-27,13)" status="error">This is not a valid value for an enumeration literal</Expects>
//<Expects id="FS0951" span="(32,5-33,13)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
//<Expects id="FS0951" span="(37,5-38,14)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
//<Expects id="FS0951" span="(41,5-42,15)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
//<Expects id="FS0951" span="(45,5-46,15)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
//<Expects id="FS0951" span="(49,5-50,24)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
//<Expects id="FS0951" span="(53,5-54,31)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
//<Expects id="FS0951" span="(57,5-58,15)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>


type EnumOfString =
    | A = "foo"
    | B = "bar"
    
type EnumOfBigInt =
    | A = 0I              // err
    | B = 1I

type EnumOfBigNum =
    | A = 0N              // err
    | B = 1N

// This is by spec.
type EnumOfNativeInt =
    | A = 0n
    | B = 0n

// This is by spec.
type EnumOfUNativeInt =
    | A = 0un
    | B = 0un

type EnumOfFloat =
    | A = 0.0f
    | B = 0.0f

type EnumOfDecimal =
    | A = 0.0m
    | B = 0.0m

type EnumOfIEEE32 =
    | A = 0x000000000lf
    | B = 0x0000000FFlf
    
type EnumOfIEEE64 =
    | A = 0x0000000000000000LF
    | B = 0x0000000000000001LF

type EnumOfFloat32 =
    | A = -1.0f
    | B = 2.3f

exit 1




