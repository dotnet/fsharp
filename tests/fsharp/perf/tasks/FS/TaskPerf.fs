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

    [<Literal>]
    let bufferSize = RepeatedAsyncWriteCSharp.BufferSize

    let writeIterations() = RepeatedAsyncWriteCSharp.WriteIterations

    [<Literal>]
    let executionIterations = RepeatedAsyncWriteCSharp.ExecutionIterations

    module TaskVersion =
        let writeFile path =
            task {
                let junk = Array.zeroCreate bufferSize
                use file = File.Create(path)
                for i = 1 to writeIterations() do
                    match RepeatedAsyncWriteCSharp.Operation with
                    |  Operation.WRITE_ASYNC ->
                        do! file.WriteAsync(junk, 0, junk.Length)
                    | Operation.FROM_RESULT -> 
                        let! v = Task.FromResult(100)
                        () // file.WriteAsync(junk, 0, junk.Length)
                    | _ -> ()
            }

        let readFile path =
            task {
                let buffer = Array.zeroCreate bufferSize
                use file = File.OpenRead(path)
                let mutable reading = true
                while reading do
                    let! countRead = file.ReadAsync(buffer, 0, buffer.Length)
                    reading <- countRead > 0
            }

        let bench() =
            let tmp = "tmp"
            task {
                let sw = Stopwatch()
                sw.Start()
                for i = 1 to executionIterations do
                    do! writeFile tmp
                    do! readFile tmp
                sw.Stop()
                printfn "task { .. } completed in %d ms" sw.ElapsedMilliseconds
                File.Delete(tmp)
            }

    module TaskBuilderVersion =
        open FSharp.Control.Tasks

        let writeFile path =
            task {
                let junk = Array.zeroCreate bufferSize
                use file = File.Create(path)
                for i = 1 to writeIterations() do
                    match RepeatedAsyncWriteCSharp.Operation with
                    |  Operation.WRITE_ASYNC ->
                        do! file.WriteAsync(junk, 0, junk.Length)
                    | Operation.FROM_RESULT -> 
                        let! v = Task.FromResult(100)
                        () // file.WriteAsync(junk, 0, junk.Length)
                    | _ -> ()
            }

        let readFile path =
            task {
                let buffer = Array.zeroCreate bufferSize
                use file = File.OpenRead(path)
                let mutable reading = true
                while reading do
                    let! countRead = file.ReadAsync(buffer, 0, buffer.Length)
                    reading <- countRead > 0
            }

        
        let bench() =
            let tmp = "tmp"
            task {
                let sw = Stopwatch()
                sw.Start()
                for i = 1 to executionIterations do
                    do! writeFile tmp
                    do! readFile tmp
                sw.Stop()
                printfn "TaskBuilder task { .. } completed in %d ms" sw.ElapsedMilliseconds
                File.Delete(tmp)
            }

    module FSharpAsyncVersion =
        let writeFile path =
            async {
                let junk = Array.zeroCreate bufferSize
                use file = File.Create(path)
                for i = 1 to writeIterations() do
                    match RepeatedAsyncWriteCSharp.Operation with
                    |  Operation.WRITE_ASYNC ->
                        do! file.AsyncWrite(junk, 0, junk.Length)
                    | Operation.FROM_RESULT -> 
                        let! v = async.Return 100
                        ()
                    | _ -> ()
            }

        let readFile path =
            async {
                let buffer = Array.zeroCreate bufferSize
                use file = File.OpenRead(path)
                let mutable reading = true
                while reading do
                    let! countRead = file.AsyncRead(buffer, 0, buffer.Length)
                    reading <- countRead > 0
            }

        let bench() =
            let tmp = "tmp"
            async {
                let sw = Stopwatch()
                sw.Start()
                for i = 1 to executionIterations do
                    do! writeFile tmp
                    do! readFile tmp
                sw.Stop()
                printfn "F# async completed in %d ms" sw.ElapsedMilliseconds
                File.Delete(tmp)
            }

    module FSharpAsyncAwaitTaskVersion =
        let writeFile path =
            async {
                let junk = Array.zeroCreate bufferSize
                use file = File.Create(path)
                for i = 1 to writeIterations() do
                    match RepeatedAsyncWriteCSharp.Operation with
                    |  Operation.WRITE_ASYNC ->
                        do! Async.AwaitTask(file.WriteAsync(junk, 0, junk.Length))
                    | Operation.FROM_RESULT -> 
                        let! v = async.Return 100
                        ()
                    | _ -> ()
            }

        let readFile path =
            async {
                let buffer = Array.zeroCreate bufferSize
                use file = File.OpenRead(path)
                let mutable reading = true
                while reading do
                    let! countRead = Async.AwaitTask(file.ReadAsync(buffer, 0, buffer.Length))
                    reading <- countRead > 0
            }

        let bench() =
            let tmp = "tmp"
            async {
                let sw = Stopwatch()
                sw.Start()
                for i = 1 to executionIterations do
                    do! writeFile tmp
                    do! readFile tmp
                sw.Stop()
                printfn "F# async (AwaitTask) completed in %d ms" sw.ElapsedMilliseconds
                File.Delete(tmp)
            }

module AllocTests = 

    let syncTask() = Task.FromResult 100
    let asyncTask() = Task.Yield()

    let tenBindSynchronous() =
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

    let tenBindAsynchronous() =
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

    let singleTask() = task { return 1 }

    let numIterations = 10000

    let allocTestSyncBinds() = 
         for i in 1 .. numIterations do 
             tenBindSynchronous().Wait() 

    let allocTestAsyncBinds() = 
         for i in 1 .. numIterations do 
             tenBindAsynchronous().Wait() 

    let allocTestSingleTask() = 
         for i in 1 .. numIterations*100 do 
             singleTask().Wait() 


[<EntryPoint>]
let main argv = 
    match argv.[0] with
    | "allocSingleTask" -> AllocTests.allocTestSingleTask()
    | "allocSyncBinds" -> AllocTests.allocTestSyncBinds()
    | "allocAsyncBinds" -> AllocTests.allocTestAsyncBinds()
    | _ ->
        for (op, n) in [(Operation.WRITE_ASYNC, 5000); (Operation.FROM_RESULT, 300000)] do 
            RepeatedAsyncWriteCSharp.Operation <- op
            RepeatedAsyncWriteCSharp.WriteIterations <- n
            printfn "-------- operation = %A ------" op
            RepeatedAsyncWriteCSharp.Bench().Wait()
            RepeatedAsyncWrite.TaskVersion.bench().Wait()
            RepeatedAsyncWrite.TaskBuilderVersion.bench().Wait()
            RepeatedAsyncWrite.FSharpAsyncVersion.bench() |> Async.RunSynchronously
            RepeatedAsyncWrite.FSharpAsyncAwaitTaskVersion.bench() |> Async.RunSynchronously
    0  