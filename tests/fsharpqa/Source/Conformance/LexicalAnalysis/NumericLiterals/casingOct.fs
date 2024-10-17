// #Conformance #LexicalAnalysis #Constants 
// Number type specifier are not case-sensitive
// Oct
//<Expects status="success"></Expects>
#light

let x = 0o7
let y = 0O6

exit (if x=y then 1 else 0)
