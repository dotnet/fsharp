open Prelude

type NonGenericTestNameRecursiveInClass() =

    let rec testValue id (data: byref<byte>) : unit =
        if id = 10 then 
            data <- 3uy 
        else 
            testValue (id + 1) &data

    static do NonGenericTestNameRecursiveInClass().Test()

    member __.Test() = 
        let mutable x = 0uy
        testValue  0 &x
        check "vruoer3rvvremtys" x 3uy