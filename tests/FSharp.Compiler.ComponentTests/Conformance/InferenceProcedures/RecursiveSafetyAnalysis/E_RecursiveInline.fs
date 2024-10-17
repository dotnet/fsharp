// #Regression #Conformance #TypeInference #Recursion 
// Regression test for FSharp1.0:3475 - ICE on rec inline function




let rec inline test x =
    if x then test false
    else 0
