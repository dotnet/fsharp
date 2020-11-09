// #Conformance #LexicalAnalysis 
#light

// Test two-byte Unicode Character literals
let spade, club, heart, diamond = '\u2660', '\u2663', '\u2665', '\u2666'
let str = sprintf "%c %c %c %c" spade club heart diamond

let expectedResult = "♠ ♣ ♥ ♦"

if str <> expectedResult then exit 1

exit 0
