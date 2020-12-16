// #Regression #Conformance #LexicalAnalysis #Constants 
// Regression test for FSHARP1.0:1284
// Enum type definitions do not support negative literals
// Negative literal with no space 
// As per FSHARP1.0:3714, enums can't be based on floats (pos/neg)
//<Expects id="FS0951" span="(10,23-10,34)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
//<Expects id="FS0951" span="(11,23-11,35)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
#light

type EnumDouble     = | A1 = -1.2   // err
type EnumSingle     = | A1 = -1.2f  // err
