open Prelude

module MutateInRef3 =
    [<Struct>]
    type TestMut(x: int ref) =

        member this.X = x.contents
        member this.XAddr = &x.contents

    let testIn (m: inref<TestMut>) =
        // If the struct API indirectly reveals a byref return of a field in a reference type then  
        // there is nothing stopping it being written to.
        m.XAddr <- 1

    let test() =
        let m = TestMut(ref 0)
        testIn (&m)
        check "vleoij" m.X 1

    test()