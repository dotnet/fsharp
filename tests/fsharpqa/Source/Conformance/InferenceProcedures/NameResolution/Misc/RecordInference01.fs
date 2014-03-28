// #Conformance #TypeInference 
// Verify the compiler can correctly infer record types
// in the face of (partial) ambiguity.

type R1 = { a : int; b : int }

type R2 = { a : int; c : int }

let r1 = { a = 1; b = 2 }

let r2 = { a = 1; c = 2 }

exit 0
