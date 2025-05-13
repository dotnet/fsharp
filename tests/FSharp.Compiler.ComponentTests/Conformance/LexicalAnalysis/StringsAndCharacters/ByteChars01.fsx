// #Regression #Conformance #LexicalAnalysis 
#light

// Verify byte chars return the expected value

// FSB 2501, Getting byte representation of characters returns wrong value

if 'A'B <> 65uy then ignore 1
if 'B'B <> 66uy then ignore 1


if 'a'B <> 97uy then ignore 1
if 'b'B <> 98uy then ignore 1


ignore 0
