// #Conformance #LexicalAnalysis 
#light

// Test more valid identifiers

let f   x = 3.5 * x ** 2.0 + 1.0 * x ** 1.0 + 6.0 * x + 8.0
let f'  x = 7.0 * x ** 1.0 + 1.0
let f'' x = 7.0

if f  10.0 < f'  10.0 then exit 1
if f' 10.0 < f'' 10.0 then exit 1

exit 0
