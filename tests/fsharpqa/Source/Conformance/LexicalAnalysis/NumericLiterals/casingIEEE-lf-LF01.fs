// #Conformance #LexicalAnalysis #Constants 
// Number type specifier LF/lf are case-sensitive
// IEEE32/IEEE64 - lf vs LF
//<Expects status="success"></Expects>
#light

let x = 0x5lf
let y = 0x5LF

let wx = x.GetType()
let wy = y.GetType()

// We expect the 2 types to be different!
let r = if wx=wy then 1 else 0

exit r



