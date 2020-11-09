// This causes a warning 40
[<EntryPoint>]
let main argv = 
    let rec x = lazy(x.Value)
    0 // return an integer exit code
