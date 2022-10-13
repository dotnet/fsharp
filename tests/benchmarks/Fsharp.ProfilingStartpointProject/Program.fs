open FSharp.Compiler.Benchmarks

let bench = new FileCascadeBenchmarks()
do bench.Setup()

[<EntryPoint>]
let main args =
    match args |> Array.toList with
    | ["cache-clear"] -> 
        for i=1 to 1000 do
            bench.ParseProjectWithFullCacheClear()  |> ignore         
    | ["mid-change"] ->
        for id=1 to 1000 do
            bench.ParseProjectWithChangingMiddleFile() |> ignore
    | _ -> failwith "Invalid args. Use cache-clear or mid-change"
    |> ignore
    0
