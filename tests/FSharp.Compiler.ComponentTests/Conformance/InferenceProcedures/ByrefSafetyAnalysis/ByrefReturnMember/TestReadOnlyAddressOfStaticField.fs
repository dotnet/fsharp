open Prelude

module TestReadOnlyAddressOfStaticField = 
    type C() =
        static let x = 1
        static member F() = &x

    let test() =
        let addr = &C.F()
        check2 "mepojcwem18a2dw" 1 addr

    test()