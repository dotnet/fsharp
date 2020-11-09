// #Conformance #LexicalAnalysis 
#light

// test1 --------------------------------------
let test1 = 
    "a"
    #if NOTDEFINED
    + "b
    #else 
    + "c
    #else
    d"
    #endif

if test1.Contains("b") then exit 1

exit 0
