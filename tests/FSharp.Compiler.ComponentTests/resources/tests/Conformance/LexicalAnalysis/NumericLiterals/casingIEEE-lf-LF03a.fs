// #Regression #Conformance #LexicalAnalysis #Constants 
// Number type specifier
// IEEE32/IEEE64 - lF is illegal
//<Expects status="error" span="(6,9-6,14)" id="FS1156">This is not a valid numeric literal. Valid numeric literals include</Expects>

let x = 0X5lF

exit 1
