open FSharp.Compiler.Benchmarks

let bench = new FileCascadeBenchmarks()
do bench.Setup()

[<EntryPoint>]
let main args =
    match args |> Array.toList with
    | ["no-change"] -> 
        for i=1 to 256 do
            printfn "***************************"
            printfn "ITERATION %i" i
            printfn "***************************"
            bench.ParseAndCheckLastFileProjectAsIs()  |> ignore         
    | ["mid-change"] ->
        for i=1 to 16 do
            printfn "***************************"
            printfn "ITERATION %i" i
            printfn "***************************"
            bench.ParseProjectWithChangingMiddleFile() |> ignore
    | _ -> failwith "Invalid args. Use cache-clear or mid-change"
    |> ignore
    0
