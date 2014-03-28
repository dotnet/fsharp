// #Conformance #LexicalAnalysis 
#light

// Test ability to create byte strings
// literal strings

let test= @"a\b\c

d\e\f"B;;

          //   a    \     b     \     c     \r   \n    \r    \n     d      \    e       \      f
if test <> [|97uy; 92uy; 98uy; 92uy; 99uy; 13uy; 10uy; 13uy; 10uy; 100uy; 92uy; 101uy; 92uy; 102uy|] then exit 1

exit 0
