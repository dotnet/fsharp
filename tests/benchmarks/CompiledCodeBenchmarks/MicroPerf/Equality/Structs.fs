namespace Equality

open BenchmarkDotNet.Attributes

[<MemoryDiagnoser>]
type Structs() =

    let numbers = Array.init 1000 id

    let createStruct3 (x: int) = struct (x, x, x)
    let createStruct4 (x: int) = struct (x, x, x, x)
    let createStruct5 (x: int) = struct (x, x, x, x, x)
    let createStruct6 (x: int) = struct (x, x, x, x, x, x)
    let createStruct7 (x: int) = struct (x, x, x, x, x, x, x)
    let createStruct8 (x: int) = struct (x, x, x, x, x, x, x, x)

    [<Benchmark(Baseline = true)>]
    member _.Struct3() =
        numbers |> Array.countBy (fun n -> createStruct3 n)

    [<Benchmark>]
    member _.Struct4() =
        numbers |> Array.countBy (fun n -> createStruct4 n)

    [<Benchmark>]
    member _.Struct5() =
        numbers |> Array.countBy (fun n -> createStruct5 n)

    [<Benchmark>]
    member _.Struct6() =
        numbers |> Array.countBy (fun n -> createStruct6 n)

    [<Benchmark>]
    member _.Struct7() =
        numbers |> Array.countBy (fun n -> createStruct7 n)

    [<Benchmark>]
    member _.Struct8() =
        numbers |> Array.countBy (fun n -> createStruct8 n)