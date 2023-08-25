open Prelude

type StaticGenericTestNameRecursiveInClass() =

    static let rec testValue unused id (data: byref<byte>) : unit =
        if id = 10 then data <- 3uy else testValue unused (id + 1) &data

    static do StaticGenericTestNameRecursiveInClass.Test()

    static member Test() = 
        let mutable x = 0uy
        testValue "unused" 0 &x
        check "vruoer3rv" x 3uy
        let mutable z = 0uy
        testValue 6L 0 &z
        check "vruoer3rvwqfgw" z 3uy