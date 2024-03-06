namespace Equality

open BenchmarkDotNet.Attributes

[<Struct>]
type SomeStruct =
    val A : int
    new a = { A = a }

type FSharpCoreFunctions() =

    let array = Array.init 1000 id
    let list = List.init 1000 id
    let seq = Seq.init 1000 id

    [<Benchmark>]
    member _.ArrayCountBy() =
        array
        |> Array.countBy (fun n -> SomeStruct(n % 7))

    [<Benchmark>]
    member _.ArrayGroupBy() =
        array
        |> Array.groupBy (fun n -> SomeStruct(n % 7))

    [<Benchmark>]
    member _.ArrayDistinct() =
        array
        |> Array.map (fun n -> SomeStruct(n % 7))
        |> Array.distinct

    [<Benchmark>]
    member _.ArrayDistinctBy() =
        array
        |> Array.distinctBy (fun n -> SomeStruct(n % 7))

    [<Benchmark>]
    member _.ArrayExcept() =
        array
        |> Array.map SomeStruct
        |> Array.except ([| SomeStruct 42 |])

    [<Benchmark>]
    member _.ListCountBy() =
        list
        |> List.countBy (fun n -> SomeStruct(n % 7))

    [<Benchmark>]
    member _.ListGroupBy() =
        list
        |> List.groupBy (fun n -> SomeStruct(n % 7))

    [<Benchmark>]
    member _.ListDistinct() =
        list
        |> List.map (fun n -> SomeStruct(n % 7))
        |> List.distinct

    [<Benchmark>]
    member _.ListDistinctBy() =
        list
        |> List.distinctBy (fun n -> SomeStruct(n % 7))

    [<Benchmark>]
    member _.ListExcept() =
        List.init 1000 id
        |> List.map SomeStruct
        |> List.except ([| SomeStruct 42 |])

    [<Benchmark>]
    member _.SeqCountBy() =
        seq
        |> Seq.countBy (fun n -> SomeStruct(n % 7))
        |> Seq.last

    [<Benchmark>]
    member _.SeqGroupBy() =
        seq
        |> Seq.groupBy (fun n -> SomeStruct(n % 7))
        |> Seq.last

    [<Benchmark>]
    member _.SeqDistinct() =
        seq
        |> Seq.map (fun n -> SomeStruct(n % 7))
        |> Seq.distinct
        |> Seq.last

    [<Benchmark>]
    member _.SeqDistinctBy() =
        seq
        |> Seq.distinctBy (fun n -> SomeStruct(n % 7))
        |> Seq.last

    [<Benchmark>]
    member _.SeqExcept() =
        seq
        |> Seq.map SomeStruct
        |> Seq.except ([| SomeStruct 42 |])
        |> Seq.last


