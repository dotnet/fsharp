module Collections

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Configs
open BenchmarkDotNet.Diagnosers
open System.Runtime.CompilerServices

[<SimpleJob(launchCount = 2, warmupCount = 1, targetCount = 2)>]
[<GcServer(true)>]
[<MemoryDiagnoser>]
[<MarkdownExporterAttribute.GitHub>]
type CollectionsBenchmark() =
    let mutable length = 0
    let mutable list = []
    let mutable array = [||]
            
    [<Params( (* 0, 1, 100, *) 1000, 10000)>]
    member this.Length
        with get () = length
        and set (value) = 
            length <- value
            list <- List.init length (fun _ -> 0)
            array <- Array.init length (fun _ -> 0)  
          
    /// List
    [<Benchmark>]
    member x.ListRemoveAtBeginning() =
        list
        |> List.removeAt 0 
        |> ignore

    [<Benchmark>]
    member x.ListRemoveAtEnd() =
        list
        |> List.removeAt (x.Length - 1) 
        |> ignore
        
    [<Benchmark>]
    member x.ListRemoveManyAtBeginning() =
        list
        |> List.removeManyAt 0 10 
        |> ignore

    [<Benchmark>]
    member x.ListRemoveManyAtEnd() =
        list
        |> List.removeManyAt (x.Length - 11) 10 
        |> ignore
        
    [<Benchmark>]
    member x.ListInsertAtBeginning() =
        list
        |> List.insertAt 0 1 
        |> ignore

    [<Benchmark>]
    member x.ListInsertAtEnd() =
        list
        |> List.insertAt (x.Length - 1) 1 
        |> ignore
        
    [<Benchmark>]
    member x.ListInsertManyAtBeginning() =
        list
        |> List.insertManyAt 0 [1..10] 
        |> ignore

    [<Benchmark>]
    member x.ListInsertManyAtEnd() =
        list
        |> List.insertManyAt (x.Length - 11) [1..10]
        |> ignore
        
    [<Benchmark>]
    member x.ListUpdateAtBeginning() =
        list
        |> List.updateAt 0 1 
        |> ignore

    [<Benchmark>]
    member x.ListUpdateEnd() =
        list
        |> List.updateAt (x.Length - 1) 1
        |> ignore
        
    /// Array
    [<Benchmark>]
    member x.ArrayRemoveAtBeginning() =
        array
        |> Array.removeAt 0 
        |> ignore

    [<Benchmark>]
    member x.ArrayRemoveAtEnd() =
        array
        |> Array.removeAt (x.Length - 1) 
        |> ignore
        
    [<Benchmark>]
    member x.ArrayRemoveManyAtBeginning() =
        array
        |> Array.removeManyAt 0 10 
        |> ignore

    [<Benchmark>]
    member x.ArrayRemoveManyAtEnd() =
        array
        |> Array.removeManyAt (x.Length - 11) 10 
        |> ignore
        
    [<Benchmark>]
    member x.ArrayInsertAtBeginning() =
        array
        |> Array.insertAt 0 1 
        |> ignore

    [<Benchmark>]
    member x.ArrayInsertAtEnd() =
        array
        |> Array.insertAt (x.Length - 1) 1 
        |> ignore
        
    [<Benchmark>]
    member x.ArrayInsertManyAtBeginning() =
        array
        |> Array.insertManyAt 0 [1..10] 
        |> ignore

    [<Benchmark>]
    member x.ArrayInsertManyAtEnd() =
        array
        |> Array.insertManyAt (x.Length - 11) [1..10]
        |> ignore
        
    [<Benchmark>]
    member x.ArrayUpdateAtBeginning() =
        array
        |> Array.updateAt 0 1 
        |> ignore

    [<Benchmark>]
    member x.ArrayUpdateEnd() =
        array
        |> Array.updateAt (x.Length - 1) 1
        |> ignore
        
    /// Seq
    [<Benchmark>]
    member x.SeqBaseline() =
        array 
        |> Seq.toList
        |> ignore
        
    [<Benchmark>]
    member x.SeqRemoveAtBeginning() =
        list
        |> Seq.removeAt 0 
        |> Seq.toList
        |> ignore

    [<Benchmark>]
    member x.SeqRemoveAtEnd() =
        list
        |> Seq.removeAt (x.Length - 1) 
        |> Seq.toList
        |> ignore
        
    [<Benchmark>]
    member x.SeqRemoveManyAtBeginning() =
        list
        |> Seq.removeManyAt 0 10 
        |> Seq.toList
        |> ignore

    [<Benchmark>]
    member x.SeqRemoveManyAtEnd() =
        list
        |> Seq.removeManyAt (x.Length - 11) 10 
        |> Seq.iter ignore
        
    [<Benchmark>]
    member x.SeqInsertAtBeginning() =
        list
        |> Seq.insertAt 0 1
        |> Seq.iter ignore

    [<Benchmark>]
    member x.SeqInsertAtEnd() =
        list
        |> Seq.insertAt (x.Length - 1) 1 
        |> Seq.iter ignore
        
    [<Benchmark>]
    member x.SeqInsertManyAtBeginning() =
        list
        |> Seq.insertManyAt 0 [1..10] 
        |> Seq.iter ignore

    [<Benchmark>]
    member x.SeqInsertManyAtEnd() =
        list
        |> Seq.insertManyAt (x.Length - 11) [1..10]
        |> Seq.iter ignore
        
    [<Benchmark>]
    member x.SeqUpdateAtBeginning() =
        list
        |> Seq.updateAt 0 1 
        |> Seq.iter ignore

    [<Benchmark>]
    member x.SeqUpdateEnd() =
        list
        |> Seq.updateAt (x.Length - 1) 1
        |> Seq.iter ignore