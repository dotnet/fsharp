module EqualsMicroPerfAndCodeGenerationTests = 

    [<Struct>]
    // exact Equals with comparer should be generated internal
    type internal SomeStruct(v: int, u: int) =
        member _.V = v
        member _.U = u

    SomeStruct(1, 2) = SomeStruct(2, 3) |> ignore