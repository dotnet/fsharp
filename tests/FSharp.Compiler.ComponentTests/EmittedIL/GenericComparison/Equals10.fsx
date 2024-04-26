module EqualsMicroPerfAndCodeGenerationTests = 

    [<Struct>]
    type SomeStruct(v: int, u: int) =
        member _.V = v
        member _.U = u

    SomeStruct(1, 2) = SomeStruct(2, 3) |> ignore