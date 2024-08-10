module EqualsMicroPerfAndCodeGenerationTests = 

    [<Struct>]
    type SomeGenericUnion<'T> = SomeGenericUnion of 'T * int

    SomeGenericUnion (1, 2) = SomeGenericUnion (2, 3) |> ignore