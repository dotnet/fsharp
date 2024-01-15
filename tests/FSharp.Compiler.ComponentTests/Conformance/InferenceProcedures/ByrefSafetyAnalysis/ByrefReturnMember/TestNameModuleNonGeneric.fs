open Prelude

module TestNameModuleNonGeneric =

    let rec testValue id (data: byref<byte>) : unit =
        if id = 10 then 
            data <- 3uy 
        else 
            testValue (id + 1) &data

    let Test() = 
        let mutable x = 0uy
        testValue  0 &x
        check "vruoer3r" x 3uy
    Test()