namespace Equality

open BenchmarkDotNet.Attributes

open System

[<Struct>]
type BigStruct = 
    val A : int64
    val B : int64
    new (a, b) = { A = a; B = b }

[<Struct>]
type Container<'a> =
    val Item : 'a
    new i = { Item = i }

type RandomRecord = { 
    Field1 : int
    Field2 : string
    Field3 : byte 
}

[<Struct>]
type RandomRecordStruct = { 
    Field1S : int
    Field2S : string
    Field3S : byte 
}

type RandomGeneric<'a> = RandomGeneric of 'a * 'a

[<MemoryDiagnoser>]
type Misc() =

    let createBigStruct() = 
        let n = Random().NextInt64()
        Container (BigStruct (n, n))

    [<Benchmark>]
    member _.BigStruct() =
        let set = Set.empty
        for _ = 0 to 200000 do
            set.Add (createBigStruct ()) |> ignore

    [<Benchmark>]
    member _.Record() =
        let array = Array.init 1000 id
        array |> Array.countBy (fun n -> { 
            Field1 = n
            Field2 = string n
            Field3 = byte n 
        })
    
    [<Benchmark>]
    member _.RecordStruct() =
        let array = Array.init 1000 id
        array |> Array.countBy (fun n -> { 
            Field1S = n
            Field2S = string n
            Field3S = byte n 
        })

    // BDN can't work with F# anon records yet
    // https://github.com/dotnet/BenchmarkDotNet/issues/2530
    // [<Benchmark>]
    member _.AnonymousRecord() =
        let array = Array.init 1000 id
        array |> Array.countBy (fun n -> {| 
            Field1A = n
            Field2A = string n
            Field3A = byte n 
        |})

    [<Benchmark>]
    member _.GenericUnion() =
        let array = Array.init 1000 id
        array |> Array.countBy (fun n -> RandomGeneric(n, n))
