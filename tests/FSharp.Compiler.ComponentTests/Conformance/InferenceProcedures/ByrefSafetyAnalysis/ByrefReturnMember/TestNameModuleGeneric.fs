open Prelude

// check recursive functions
module TestNameModuleGeneric =

    let rec testValue (unused: 'T) id (data: byref<byte>) : unit =
        if id = 10 then 
            data <- 3uy 
        else
                testValue unused (id + 1) &data
    let Test() = 
        let mutable x = 0uy
        testValue "unused" 0 &x
        check "vruoer" x 3uy
    Test()