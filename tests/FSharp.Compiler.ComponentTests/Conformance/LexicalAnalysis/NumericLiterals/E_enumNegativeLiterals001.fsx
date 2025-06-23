// #Regression #Conformance #LexicalAnalysis #Constants 
// Regression test for FSHARP1.0:1284
// Enum type definitions do not support negative literals
// Negative literal with space 
//<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>


type EnumInt8s      = | A1 = - 10y
