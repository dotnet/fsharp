// #Conformance #LexicalAnalysis 
#light

// Test the ability to specify byte strings
let test = "ABC"B

let result = if test <> [|65uy; 66uy; 67uy|] then 1 else 0

exit result

