open Prelude

type StaticNonGenericTestNameRecursiveInClass() =

    static let rec testValue id (data: byref<byte>) : unit =
        if id = 10 then data <- 3uy else testValue (id + 1) &data

    static do StaticNonGenericTestNameRecursiveInClass.Test()

    static member Test() = 
        let mutable x = 0uy
        testValue  0 &x
        check "vruoer3rvvrebae" x 3uy