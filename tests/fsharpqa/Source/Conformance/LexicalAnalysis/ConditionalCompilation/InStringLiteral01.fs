// #Conformance #LexicalAnalysis 
#light

// test1 --------------------------------------
let test1 = "
    A
    #if DEFINED
    B
    #else 
    C
    #endif
    D
"

// Verify no conditional compilation occurred
if test1.IndexOf("A") = -1 then exit 1
if test1.IndexOf("B") = -1 then exit 1
if test1.IndexOf("C") = -1 then exit 1
if test1.IndexOf("D") = -1 then exit 1
if test1.IndexOf("#else") = -1 then exit 1

exit 0
