// #Regression #CodeGen #Optimizations #Assemblies 
// Regression for Dev10:850602
// Previously a premature optimization was causing the call to isContaining to access private data of the record type in an invalid way
// It caused illegal IL and FieldAccessExceptions at runtime

open A

let v1 = VectorFloat(0.5, 2.5, 3.5)
let v2 = VectorFloat(1.1, 2.2, 3.3)
let o = { Min = v1; Max = v2 }

if (isContaining(o) <> 0.5) then
    // Notify caller that test failed
    failwith "Error executing:"
