// #Conformance #LexicalAnalysis #Constants 
// Number type specifier are not case-sensitive
// Float
//<Expects status="success"></Expects>
#light

let x = 0x5
let y = 0X5

exit (if x=y then 0 else 1)



