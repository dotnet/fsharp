module EqualsMicroPerfAndCodeGenerationTests = 

    [<Struct>]
    // exact Equals with comparer should be generated internal
    type internal SomeRecord = { V: int; U: int }

    { V = 1; U = 2 } = { V = 2; U = 3 } |> ignore