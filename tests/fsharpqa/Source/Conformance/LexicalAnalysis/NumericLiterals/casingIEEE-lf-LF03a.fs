// #Regression #Conformance #LexicalAnalysis #Constants 
// Number type specifier
// IEEE32/IEEE64 - lF is illegal
//<Expects status="error" span="(6,9-6,14)" id="FS1156">This is not a valid numeric literal\. Sample formats include 4, 0x4, 0b0100, 4L, 4UL, 4u, 4s, 4us, 4y, 4uy, 4\.0, 4\.0f, 4I\.$</Expects>

let x = 0X5lF

exit 1
