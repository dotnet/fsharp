module EqualsMicroPerfAndCodeGenerationTests = 

    [<Struct>]
    type SomeGenericStruct<'T>(v: 'T, u: int) =
        member _.V = v
        member _.U = u

    SomeGenericStruct(1, 2) = SomeGenericStruct(2, 3) |> ignore