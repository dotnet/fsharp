// #Conformance #TypeInference #ByRef 
#light

// Test that byref values can be used in inner lambdas if they
// do not escape.

let testFunc = 
    fun () -> 
        let mutable x = 0
        let byrefVal = &x
        byrefVal

if testFunc () <> 0 then failwith "Failed"
