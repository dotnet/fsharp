// #Conformance #LexicalAnalysis 
#light

// Test string literals with short Unicode Literals

let unicodeString = "\u2660 \u2663 \u2665 \u2666"
let expectedResult = "♠ ♣ ♥ ♦"

if unicodeString <> expectedResult then exit 1

exit 0
