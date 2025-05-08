// #Regression #Conformance #LexicalAnalysis #Constants 
// Regression test for FSHARP1.0:1284
// Enum type definitions do not support negative literals
// Negative literal with space 
//<Expects status="error" span="(8,23)" id="FS0951">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>

#light
type EnumNativeInt  = | A1 = -10n    // enum on this type are not supported, -ve or +ve
