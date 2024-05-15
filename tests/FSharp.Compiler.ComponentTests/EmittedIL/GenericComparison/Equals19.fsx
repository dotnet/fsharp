module EqualsMicroPerfAndCodeGenerationTests = 

    [<Struct>]
    type SomeStruct(v: int) =
        member _.V = v

    // the equality test should be a function, otherwise might get inlined
    let test x y = SomeStruct x = SomeStruct y