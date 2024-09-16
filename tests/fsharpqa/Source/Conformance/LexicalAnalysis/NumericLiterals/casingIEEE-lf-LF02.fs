// #Conformance #LexicalAnalysis #Constants 
// Number type specifier are not case-sensitive
// IEEE32/IEEE64 - lf vs LF
// verify that xint can be specified either with '0x' or '0X'
//<Expects status="success"></Expects>
#light

let x = 0X5LF
let y = 0X5lf

let wx = x.GetType()
let wy = y.GetType()

let r = if wx=wy then 1 else 0

exit r



