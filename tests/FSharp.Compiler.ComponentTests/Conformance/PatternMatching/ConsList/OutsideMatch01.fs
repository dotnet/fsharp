// #Conformance #PatternMatching
#light

// Verify pattern matching on lists outside of a patch statement

let a :: b :: c = [1; 2; 3]

if a <> 1   then exit 1
if b <> 2   then exit 1
if c <> [3] then exit 1

exit 0
