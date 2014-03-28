// #Conformance #LexicalAnalysis 
#light

// test1 --------------------------------------
let test1 = 
    "a"
    #if DEFINED
    + "b
    #else 
    c"
    #else
    + "d"
    #endif

if test1.Contains("d") then exit 1

exit 0
