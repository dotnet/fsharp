// #Conformance #LexicalAnalysis 
#light

// Test conditional compilation flags. 
// To run this test please define "THIS_IS_DEFINED"

let runTest () =
    let mutable testsPassed = 0

    // If something is defined, use it
    #if THIS_IS_DEFINED
    testsPassed <- testsPassed + 1
    #endif

    // Make sure something that isn't defined, isn't
    #if THIS_IS_NOT_DEFINED
    testsPassed <- testsPassed + -1
    #endif

    testsPassed

let numTestsPassed = runTest()
if numTestsPassed <> 1 then exit 1

exit 0
