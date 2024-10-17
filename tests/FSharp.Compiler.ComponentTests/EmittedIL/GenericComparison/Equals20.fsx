module EqualsMicroPerfAndCodeGenerationTests = 

    [<Struct>]
    type SomeUnion = SomeUnion of int

    // the equality test should be a function, otherwise might get inlined
    let test x y = SomeUnion x = SomeUnion y