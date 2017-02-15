type AStruct<[<Measure>]'u> = 
    struct
    end

[<EntryPoint>]
let main argv = 
    let x = AStruct() // This doesn't work
    0