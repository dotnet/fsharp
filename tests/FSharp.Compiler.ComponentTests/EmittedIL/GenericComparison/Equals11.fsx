module EqualsMicroPerfAndCodeGenerationTests = 

    [<Struct>]
    type SomeUnion = SomeUnion of int * int

    SomeUnion (1, 2) = SomeUnion (2, 3) |> ignore