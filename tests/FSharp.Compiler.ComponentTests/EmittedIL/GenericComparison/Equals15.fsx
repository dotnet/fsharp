module EqualsMicroPerfAndCodeGenerationTests = 

    [<Struct>]
    type SomeGenericRecord<'T> = { V: 'T; U: int }

    { V = 1; U = 2 } = { V = 2; U = 3 } |> ignore