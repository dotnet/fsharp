module Conditions 

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Diagnosers
open System.Runtime.CompilerServices

[<MethodImpl(MethodImplOptions.NoInlining)>]
let condition_2 x =
    if  (x = 1 || x = 2)   then 1
    elif(x = 3 || x = 4)   then 2
    else 8

[<DisassemblyDiagnoser>]
[<HardwareCounters(HardwareCounter.BranchMispredictions, HardwareCounter.BranchInstructions)>]
type Benchmarks() =

    [<Benchmark>]
    [<Arguments(1)>]
    [<Arguments(2)>]
    [<Arguments(3)>]
    [<Arguments(4)>]
    member _.CSharp(x: int) =
        MicroPerfCSharp.Cond(x)

    [<Benchmark>]
    [<Arguments(1)>]
    [<Arguments(2)>]
    [<Arguments(3)>]
    [<Arguments(4)>]
    member _.FSharp(x: int) =
        condition_2(x)