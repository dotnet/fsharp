module EqualsMicroPerfAndCodeGenerationTests = 

    [<Struct>]
    // exact Equals with comparer should be generated internal
    type internal SomeUnion = SomeUnion of int * int

    SomeUnion (1, 2) = SomeUnion (2, 3) |> ignore