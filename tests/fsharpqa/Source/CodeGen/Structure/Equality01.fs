// #Regression #NoMT #CodeGen #Interop 
// Regression test for FSHARP1.0:4324 - Generated structural equality for structs does not take into consideration implicit fields captured as constructor parameters

type T (a: int, b: int) = 
    struct
        member x.a' = a
        member x.b' = b
    end

// Compare with implicit constructors
if (T(0,0) <> T(0,0) || T(1,0) = T(0,1)) then exit 1
if (T() <> T()) then exit 1
