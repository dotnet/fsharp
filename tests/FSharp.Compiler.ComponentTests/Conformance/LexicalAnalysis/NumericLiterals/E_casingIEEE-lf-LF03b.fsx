// #Regression #Conformance #LexicalAnalysis #Constants 
// Number type specifier
// IEEE32/IEEE64 - Lf is illegal
//<Expects status="error" id="FS1156" span="(6,9-6,14)">This is not a valid numeric literal. Valid numeric literals include</Expects>

let y = 0X5Lf
