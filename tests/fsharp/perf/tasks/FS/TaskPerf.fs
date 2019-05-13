(*
csc /optimize /target:library tests\fsharp\perf\tasks\csbenchmark.cs
artifacts\bin\fsc\Debug\net472\fsc.exe tests\fsharp\perf\tasks\TaskBuilder.fs tests\fsharp\perf\tasks\benchmark.fs --optimize -g -r:csbenchmark.dll
*)

namespace TaskPerf

//open FSharp.Control.Tasks
open System.Diagnostics
open System.Threading.Tasks
open System.IO
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open TaskBuilderTasks.ContextSensitive // TaskBuilder.fs extension members
open FSharp.Control.ContextSensitiveTasks // the default

//[<ShortRunJob>]
type TaskPerfTests() =
    let bufferSize = 128
    let manyIterations = 10000

    let syncTask() = Task.FromResult 100
    let syncTask_FSharpAsync() = async.Return 100
    let asyncTask() = Task.Yield()

    let taskBuilder = TaskBuilderTasks.ContextSensitive.task

    let tenBindSync_Task() =
        task {
            let! res1 = syncTask()
            let! res2 = syncTask()
            let! res3 = syncTask()
            let! res4 = syncTask()
            let! res5 = syncTask()
            let! res6 = syncTask()
            let! res7 = syncTask()
            let! res8 = syncTask()
            let! res9 = syncTask()
            let! res10 = syncTask()
            return res1 + res2 + res3 + res4 + res5 + res6 + res7 + res8 + res9 + res10
         }

    let tenBindSync_TaskBuilder() =
        taskBuilder {
            let! res1 = syncTask()
            let! res2 = syncTask()
            let! res3 = syncTask()
            let! res4 = syncTask()
            let! res5 = syncTask()
            let! res6 = syncTask()
            let! res7 = syncTask()
            let! res8 = syncTask()
            let! res9 = syncTask()
            let! res10 = syncTask()
            return res1 + res2 + res3 + res4 + res5 + res6 + res7 + res8 + res9 + res10
         }

    let tenBindSync_FSharpAsync() =
        async {
            let! res1 = syncTask_FSharpAsync()
            let! res2 = syncTask_FSharpAsync()
            let! res3 = syncTask_FSharpAsync()
            let! res4 = syncTask_FSharpAsync()
            let! res5 = syncTask_FSharpAsync()
            let! res6 = syncTask_FSharpAsync()
            let! res7 = syncTask_FSharpAsync()
            let! res8 = syncTask_FSharpAsync()
            let! res9 = syncTask_FSharpAsync()
            let! res10 = syncTask_FSharpAsync()
            return res1 + res2 + res3 + res4 + res5 + res6 + res7 + res8 + res9 + res10
         }

    let tenBindAsync_Task() =
        task {
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
         }

    let tenBindAsync_TaskBuilder() =
        taskBuilder {
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
            do! asyncTask()
         }

(*
     let tenBindAsync_FSharpAsync() =
        taskBuilder {
            do! Async.Sleep(0)
            do! Async.Sleep(0)
            do! Async.Sleep(0)
            do! Async.Sleep(0)
            do! Async.Sleep(0)
            do! Async.Sleep(0)
            do! Async.Sleep(0)
            do! Async.Sleep(0)
            do! Async.Sleep(0)
            do! Async.Sleep(0)
         }
*)

    let singleTask_Task() =
        task { return 1 }

    let singleTask_TaskBuilder() =
        taskBuilder { return 1 }

    let singleTask_FSharpAsync() =
        async { return 1 }

    [<Benchmark>]
    member __.ManyWriteFile_CSharpAsync () =
        TaskPerfCSharp.ManyWriteFileAsync().Wait();

    [<Benchmark>]
    member __.ManyWriteFile_Task () =
        let path = Path.GetTempFileName()
        task {
            let junk = Array.zeroCreate bufferSize
            use file = File.Create(path)
            for i = 1 to manyIterations do
                do! file.WriteAsync(junk, 0, junk.Length)
        }
        |> fun t -> t.Wait()
        File.Delete(path)

    [<Benchmark>]
    member __.ManyWriteFile_TaskBuilder () =
        let path = Path.GetTempFileName()
        TaskBuilderTasks.ContextSensitive.task {
            let junk = Array.zeroCreate bufferSize
            use file = File.Create(path)
            for i = 1 to manyIterations do
                do! file.WriteAsync(junk, 0, junk.Length)
        }
        |> fun t -> t.Wait()
        File.Delete(path)

    [<Benchmark>]
    member __.ManyWriteFile_FSharpAsync () =
        let path = Path.GetTempFileName()
        async {
            let junk = Array.zeroCreate bufferSize
            use file = File.Create(path)
            for i = 1 to manyIterations do
                do! Async.AwaitTask(file.WriteAsync(junk, 0, junk.Length))
        }
        |> Async.RunSynchronously
        File.Delete(path)

    [<Benchmark>]
    member __.SyncBinds_CSharpAsync() = 
         for i in 1 .. manyIterations*100 do 
             TaskPerfCSharp.TenBindsSync_CSharp().Wait() 

    [<Benchmark>]
    member __.SyncBinds_Task() = 
        for i in 1 .. manyIterations*100 do 
             tenBindSync_Task().Wait() 

    [<Benchmark>]
    member __.SyncBinds_TaskBuilder() = 
        for i in 1 .. manyIterations*100 do 
             tenBindSync_TaskBuilder().Wait() 

    [<Benchmark>]
    member __.SyncBinds_FSharpAsync() = 
        for i in 1 .. manyIterations*100 do 
             tenBindSync_FSharpAsync() |> Async.RunSynchronously |> ignore

    [<Benchmark>]
    member __.AsyncBinds_CSharpAsync() = 
         for i in 1 .. manyIterations do 
             TaskPerfCSharp.TenBindsAsync_CSharp().Wait() 

    [<Benchmark>]
    member __.AsyncBinds_Task() = 
         for i in 1 .. manyIterations do 
             tenBindAsync_Task().Wait() 

    [<Benchmark>]
    member __.AsyncBinds_TaskBuilder() = 
         for i in 1 .. manyIterations do 
             tenBindAsync_TaskBuilder().Wait() 

    //[<Benchmark>]
    //member __.AsyncBinds_FSharpAsync() = 
    //     for i in 1 .. manyIterations do 
    //         tenBindAsync_FSharpAsync() |> Async.RunSynchronously 

    [<Benchmark>]
    member __.SingleSyncTask_CSharpAsync() = 
         for i in 1 .. manyIterations*500 do 
             TaskPerfCSharp.SingleSyncTask_CSharp().Wait() 

    [<Benchmark>]
    member __.SingleSyncTask_Task() = 
         for i in 1 .. manyIterations*500 do 
             singleTask_Task().Wait() 

    [<Benchmark>]
    member __.SingleSyncTask_TaskBuilder() = 
         for i in 1 .. manyIterations*500 do 
             singleTask_TaskBuilder().Wait() 

    [<Benchmark>]
    member __.SingleSyncTask_FSharpAsync() = 
         for i in 1 .. manyIterations*500 do 
             singleTask_FSharpAsync() |> Async.RunSynchronously |> ignore

module Main = 

    [<EntryPoint>]
    let main argv = 
        let summary = BenchmarkRunner.Run<TaskPerfTests>();
        0  