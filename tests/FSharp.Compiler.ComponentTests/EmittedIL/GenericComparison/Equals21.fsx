module EqualsMicroPerfAndCodeGenerationTests = 

    [<Struct>]
    type SomeRecord = { V: int }

    // the equality test should be a function, otherwise might get inlined
    let test x y = { V = x } = { V = y }