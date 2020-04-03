(*
msbuild tests\fsharp\perf\tasks\FS\TaskPerf.fsproj /p:Configuration=Release
dotnet artifacts\bin\TaskPerf\Release\netcoreapp2.1\TaskPerf.dll
*)

namespace TaskPerf

//open FSharp.Control.Tasks
open System.Diagnostics
open System.Threading.Tasks
open System.IO
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open TaskBuilderTasks.ContextSensitive // TaskBuilder.fs extension members
//open FSharp.Control.ContextSensitiveTasks // the default
open FSharp.Control // AsyncSeq
open Tests.SyncBuilder
open Tests.TaskSeqBuilder
open BenchmarkDotNet.Configs

[<AutoOpen>]
module Helpers =
    let bufferSize = 128
    let manyIterations = 10000

    let syncTask() = Task.FromResult 100
    let syncTask_FSharpAsync() = async.Return 100
    let asyncTask() = Task.Yield()

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

    let singleTask_Task() =
        task { return 1 }

    let singleTask_TaskBuilder() =
        taskBuilder { return 1 }

    let singleTask_FSharpAsync() =
        async { return 1 }

[<MemoryDiagnoser>]
[<GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)>]
[<CategoriesColumn>]
type Benchmarks() =
    //[<BenchmarkCategory("ManyWriteFile"); Benchmark(Baseline=true)>]
    //member __.ManyWriteFile_CSharpAsync () =
    //    TaskPerfCSharp.ManyWriteFileAsync().Wait();

    //[<BenchmarkCategory("ManyWriteFile");Benchmark>]
    //member __.ManyWriteFile_Task () =
    //    let path = Path.GetTempFileName()
    //    task {
    //        let junk = Array.zeroCreate bufferSize
    //        use file = File.Create(path)
    //        for i = 1 to manyIterations do
    //            do! file.WriteAsync(junk, 0, junk.Length)
    //    }
    //    |> fun t -> t.Wait()
    //    File.Delete(path)

    //[<BenchmarkCategory("ManyWriteFile");Benchmark>]
    //member __.ManyWriteFile_TaskBuilder () =
    //    let path = Path.GetTempFileName()
    //    taskBuilder {
    //        let junk = Array.zeroCreate bufferSize
    //        use file = File.Create(path)
    //        for i = 1 to manyIterations do
    //            do! file.WriteAsync(junk, 0, junk.Length)
    //    }
    //    |> fun t -> t.Wait()
    //    File.Delete(path)

    //[<BenchmarkCategory("ManyWriteFile");Benchmark>]
    //member __.ManyWriteFile_FSharpAsync () =
    //    let path = Path.GetTempFileName()
    //    async {
    //        let junk = Array.zeroCreate bufferSize
    //        use file = File.Create(path)
    //        for i = 1 to manyIterations do
    //            do! Async.AwaitTask(file.WriteAsync(junk, 0, junk.Length))
    //    }
    //    |> Async.RunSynchronously
    //    File.Delete(path)

    //[<BenchmarkCategory("NonAsyncBinds"); Benchmark(Baseline=true)>]
    //member __.NonAsyncBinds_CSharpAsync() = 
    //     for i in 1 .. manyIterations*100 do 
    //         TaskPerfCSharp.TenBindsSync_CSharp().Wait() 

    //[<BenchmarkCategory("NonAsyncBinds"); Benchmark>]
    //member __.NonAsyncBinds_Task() = 
    //    for i in 1 .. manyIterations*100 do 
    //         tenBindSync_Task().Wait() 

    //[<BenchmarkCategory("NonAsyncBinds"); Benchmark>]
    //member __.NonAsyncBinds_TaskBuilder() = 
    //    for i in 1 .. manyIterations*100 do 
    //         tenBindSync_TaskBuilder().Wait() 

    //[<BenchmarkCategory("NonAsyncBinds"); Benchmark>]
    //member __.NonAsyncBinds_FSharpAsync() = 
    //    for i in 1 .. manyIterations*100 do 
    //         tenBindSync_FSharpAsync() |> Async.RunSynchronously |> ignore

    //[<BenchmarkCategory("AsyncBinds"); Benchmark(Baseline=true)>]
    //member __.AsyncBinds_CSharpAsync() = 
    //     for i in 1 .. manyIterations do 
    //         TaskPerfCSharp.TenBindsAsync_CSharp().Wait() 

    //[<BenchmarkCategory("AsyncBinds"); Benchmark>]
    //member __.AsyncBinds_Task() = 
    //     for i in 1 .. manyIterations do 
    //         tenBindAsync_Task().Wait() 

    //[<BenchmarkCategory("AsyncBinds"); Benchmark>]
    //member __.AsyncBinds_TaskBuilder() = 
    //     for i in 1 .. manyIterations do 
    //         tenBindAsync_TaskBuilder().Wait() 

    ////[<Benchmark>]
    ////member __.AsyncBinds_FSharpAsync() = 
    ////     for i in 1 .. manyIterations do 
    ////         tenBindAsync_FSharpAsync() |> Async.RunSynchronously 

    //[<BenchmarkCategory("SingleSyncTask"); Benchmark(Baseline=true)>]
    //member __.SingleSyncTask_CSharpAsync() = 
    //     for i in 1 .. manyIterations*500 do 
    //         TaskPerfCSharp.SingleSyncTask_CSharp().Wait() 

    //[<BenchmarkCategory("SingleSyncTask"); Benchmark>]
    //member __.SingleSyncTask_Task() = 
    //     for i in 1 .. manyIterations*500 do 
    //         singleTask_Task().Wait() 

    //[<BenchmarkCategory("SingleSyncTask"); Benchmark>]
    //member __.SingleSyncTask_TaskBuilder() = 
    //     for i in 1 .. manyIterations*500 do 
    //         singleTask_TaskBuilder().Wait() 

    //[<BenchmarkCategory("SingleSyncTask"); Benchmark>]
    //member __.SingleSyncTask_FSharpAsync() = 
    //     for i in 1 .. manyIterations*500 do 
    //         singleTask_FSharpAsync() |> Async.RunSynchronously |> ignore

    //[<BenchmarkCategory("sync"); Benchmark(Baseline=true)>]
    //member __.SyncBuilderLoop_NormalCode() = 
    //    for i in 1 .. manyIterations do 
    //                let mutable res = 0
    //                for i in Seq.init 1000 id do
    //                   res <- i + res

    //[<BenchmarkCategory("sync"); Benchmark>]
    //member __.SyncBuilderLoop_WorkflowCode() = 
    //    for i in 1 .. manyIterations do 
    //         sync { let mutable res = 0
    //                for i in Seq.init 1000 id do
    //                   res <- i + res }

    //[<BenchmarkCategory("list"); Benchmark(Baseline=true)>]
    //member __.ListBuilder_ListExpression() = Tests.ListAndArrayBuilder.Examples.perf2()

    //[<BenchmarkCategory("list"); Benchmark>]
    //member __.ListBuilder_ListBuilder() = Tests.ListAndArrayBuilder.Examples.perf1()

    //[<BenchmarkCategory("array"); Benchmark(Baseline=true)>]
    //member __.ArrayBuilder_ArrayExpression() = Tests.ListAndArrayBuilder.Examples.perf2A()

    //[<BenchmarkCategory("array"); Benchmark>]
    //member __.ArrayBuilder_ArrayBuilder() = Tests.ListAndArrayBuilder.Examples.perf1A()

    [<BenchmarkCategory("taskSeq"); Benchmark(Baseline=true)>]
    member __.TaskSeq_Example() = 
        Tests.TaskSeqBuilder.Examples.perf2() |> TaskSeq.iter ignore

    [<BenchmarkCategory("taskSeq"); Benchmark(Baseline=true)>]
    member __.AsyncSeq_Example() = 
        Tests.TaskSeqBuilder.Examples.perf2_AsyncSeq() |> AsyncSeq.iter ignore |> Async.RunSynchronously

module Main = 

    [<EntryPoint>]
    let main argv = 
        printfn "Testing that the tests run..."
        //Benchmarks().ManyWriteFile_CSharpAsync()
        //Benchmarks().ManyWriteFile_Task ()
        //Benchmarks().ManyWriteFile_TaskBuilder ()
        //Benchmarks().ManyWriteFile_FSharpAsync ()
        //Benchmarks().NonAsyncBinds_CSharpAsync() 
        //Benchmarks().NonAsyncBinds_Task() 
        //Benchmarks().NonAsyncBinds_TaskBuilder() 
        //Benchmarks().NonAsyncBinds_FSharpAsync() 
        //Benchmarks().AsyncBinds_CSharpAsync() 
        //Benchmarks().AsyncBinds_Task() 
        //Benchmarks().AsyncBinds_TaskBuilder() 
        //Benchmarks().SingleSyncTask_CSharpAsync()
        //Benchmarks().SingleSyncTask_Task() 
        //Benchmarks().SingleSyncTask_TaskBuilder() 
        //Benchmarks().SingleSyncTask_FSharpAsync()
        Benchmarks().TaskSeq_Example()
        Benchmarks().AsyncSeq_Example()
        printfn "Running benchmarks..."

        let results = BenchmarkRunner.Run<Benchmarks>()
        0  