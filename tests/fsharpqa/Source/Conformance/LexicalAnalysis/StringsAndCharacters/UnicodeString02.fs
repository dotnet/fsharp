// #Conformance #LexicalAnalysis 
#light

// Test string literals with long Unicode Literals

let unicodeString = "\U00002660 \U00002663 \U00002665 \U00002666"
let expectedResult = "♠ ♣ ♥ ♦"

if unicodeString <> expectedResult then exit 1

exit 0
