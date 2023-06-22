open FSharp.Compiler.Benchmarks

open System
open System.IO
open System.Text
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open BenchmarkDotNet.Attributes
open FSharp.Compiler.Benchmarks
open Microsoft.CodeAnalysis.Text
open BenchmarkDotNet.Order
open BenchmarkDotNet.Mathematics

let bench = new FileCascadeBenchmarks()
bench.GenerateFSI <- true
do bench.Setup()

(*
This project was created as an easy entry point for low-level profiling of FCS operations.
The only purpose is the easy of setup (simply set as startup project and launch) so that a profiler can be connected.
There is definitely no harm in deleting it if it starts bothering anyone.
*)


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
