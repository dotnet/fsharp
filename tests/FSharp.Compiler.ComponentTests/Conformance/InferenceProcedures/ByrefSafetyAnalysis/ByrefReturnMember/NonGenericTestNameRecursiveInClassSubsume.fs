open Prelude

type NonGenericTestNameRecursiveInClassSubsume() =

    let rec testValue id (data: byref<byte>) (y:System.IComparable) : unit =
        if id = 10 then 
            data <- 3uy 
        else 
            testValue (id + 1) &data y

    static do NonGenericTestNameRecursiveInClassSubsume().Test()

    member __.Test() = 
        let mutable x = 0uy
        testValue  0 &x Unchecked.defaultof<System.IComparable>
        check "vruoer3rvvremtys" x 3uy