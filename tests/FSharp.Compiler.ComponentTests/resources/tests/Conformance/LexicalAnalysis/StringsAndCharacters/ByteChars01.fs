// #Regression #Conformance #LexicalAnalysis 
#light

// Verify byte chars return the expected value

// FSB 2501, Getting byte representation of characters returns wrong value

if 'A'B <> 65uy then exit 1
if 'B'B <> 66uy then exit 1


if 'a'B <> 97uy then exit 1
if 'b'B <> 98uy then exit 1


exit 0
