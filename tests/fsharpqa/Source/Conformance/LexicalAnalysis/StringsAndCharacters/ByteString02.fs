// #Conformance #LexicalAnalysis 
#light

// Test ability to specify byte strings
// Multi line using backslash

let test ="000\
111\
222"B

if test <> [|48uy; 48uy; 48uy; 49uy; 49uy; 49uy; 50uy; 50uy; 50uy|] then exit 1

exit 0
