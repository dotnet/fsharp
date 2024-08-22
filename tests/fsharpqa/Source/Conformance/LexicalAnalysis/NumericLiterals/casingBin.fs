// #Conformance #LexicalAnalysis #Constants 
// Number type specifier are not case-sensitive
// Bin
//<Expects status="success"></Expects>
#light

let x1 = 0B1
let y1 = 0b1

let x2 = 0B0
let y2 = 0b0

let x3 = 0B10
let y3 = 0b10

let x4 = 0B10
let y4 = 0b10

exit (if (x1=y1 && x2=y2 && x3=y3 && x4=y4) then 0 else 1)
