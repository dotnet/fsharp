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

if test1.Contains("d") then ignore 1

ignore 0
