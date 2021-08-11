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

    [<Params( (* 0, 1, 100, *) 1000, 10000)>]
    member val Length = 0 with get, set
    /// List
    [<Benchmark>]
    member x.ListRemoveAtBeginning() =
        async {
            List.init x.Length (fun _ -> 0)
            |> List.removeAt 0 
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.ListRemoveAtEnd() =
        async {
            List.init x.Length (fun _ -> 0)
            |> List.removeAt (x.Length - 1) 
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.ListRemoveManyAtBeginning() =
        async {
            List.init x.Length (fun _ -> 0)
            |> List.removeManyAt 0 10 
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.ListRemoveManyAtEnd() =
        async {
            List.init x.Length (fun _ -> 0)
            |> List.removeManyAt (x.Length - 11) 10 
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.ListInsertAtBeginning() =
        async {
            List.init x.Length (fun _ -> 0)
            |> List.insertAt 0 1 
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.ListInsertAtEnd() =
        async {
            List.init x.Length (fun _ -> 0)
            |> List.insertAt (x.Length - 1) 1 
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.ListInsertManyAtBeginning() =
        async {
            List.init x.Length (fun _ -> 0)
            |> List.insertManyAt 0 [1..10] 
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.ListInsertManyAtEnd() =
        async {
            List.init x.Length (fun _ -> 0)
            |> List.insertManyAt (x.Length - 11) [1..10]
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.ListUpdateAtBeginning() =
        async {
            List.init x.Length (fun _ -> 0)
            |> List.updateAt 0 1 
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.ListUpdateEnd() =
        async {
            List.init x.Length (fun _ -> 0)
            |> List.updateAt (x.Length - 1) 1
            |> ignore
        }
        |> Async.StartAsTask
        
    /// Array
    [<Benchmark>]
    member x.ArrayRemoveAtBeginning() =
        async {
            Array.init x.Length (fun _ -> 0)
            |> Array.removeAt 0 
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.ArrayRemoveAtEnd() =
        async {
            Array.init x.Length (fun _ -> 0)
            |> Array.removeAt (x.Length - 1) 
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.ArrayRemoveManyAtBeginning() =
        async {
            Array.init x.Length (fun _ -> 0)
            |> Array.removeManyAt 0 10 
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.ArrayRemoveManyAtEnd() =
        async {
            Array.init x.Length (fun _ -> 0)
            |> Array.removeManyAt (x.Length - 11) 10 
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.ArrayInsertAtBeginning() =
        async {
            Array.init x.Length (fun _ -> 0)
            |> Array.insertAt 0 1 
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.ArrayInsertAtEnd() =
        async {
            Array.init x.Length (fun _ -> 0)
            |> Array.insertAt (x.Length - 1) 1 
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.ArrayInsertManyAtBeginning() =
        async {
            Array.init x.Length (fun _ -> 0)
            |> Array.insertManyAt 0 [1..10] 
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.ArrayInsertManyAtEnd() =
        async {
            Array.init x.Length (fun _ -> 0)
            |> Array.insertManyAt (x.Length - 11) [1..10]
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.ArrayUpdateAtBeginning() =
        async {
            Array.init x.Length (fun _ -> 0)
            |> Array.updateAt 0 1 
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.ArrayUpdateEnd() =
        async {
            Array.init x.Length (fun _ -> 0)
            |> Array.updateAt (x.Length - 1) 1
            |> ignore
        }
        |> Async.StartAsTask
        
    /// Seq
    [<Benchmark>]
    member x.SeqRemoveAtBeginning() =
        async {
            Seq.init x.Length (fun _ -> 0)
            |> Seq.removeAt 0 
            |> Seq.toList
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.SeqRemoveAtEnd() =
        async {
            Seq.init x.Length (fun _ -> 0)
            |> Seq.removeAt (x.Length - 1) 
            |> Seq.toList
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.SeqRemoveManyAtBeginning() =
        async {
            Seq.init x.Length (fun _ -> 0)
            |> Seq.removeManyAt 0 10 
            |> Seq.toList
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.SeqRemoveManyAtEnd() =
        async {
            Seq.init x.Length (fun _ -> 0)
            |> Seq.removeManyAt (x.Length - 11) 10 
            |> Seq.toList
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.SeqInsertAtBeginning() =
        async {
            Seq.init x.Length (fun _ -> 0)
            |> Seq.insertAt 0 1
            |> Seq.toList
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.SeqInsertAtEnd() =
        async {
            Seq.init x.Length (fun _ -> 0)
            |> Seq.insertAt (x.Length - 1) 1 
            |> Seq.toList
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.SeqInsertManyAtBeginning() =
        async {
            Seq.init x.Length (fun _ -> 0)
            |> Seq.insertManyAt 0 [1..10] 
            |> Seq.toList
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.SeqInsertManyAtEnd() =
        async {
            Seq.init x.Length (fun _ -> 0)
            |> Seq.insertManyAt (x.Length - 11) [1..10]
            |> Seq.toList
            |> ignore
        }
        |> Async.StartAsTask
        
    [<Benchmark>]
    member x.SeqUpdateAtBeginning() =
        async {
            Seq.init x.Length (fun _ -> 0)
            |> Seq.updateAt 0 1 
            |> Seq.toList
            |> ignore
        }
        |> Async.StartAsTask

    [<Benchmark>]
    member x.SeqUpdateEnd() =
        async {
            Seq.init x.Length (fun _ -> 0)
            |> Seq.updateAt (x.Length - 1) 1
            |> Seq.toList
            |> ignore
        }
        |> Async.StartAsTask