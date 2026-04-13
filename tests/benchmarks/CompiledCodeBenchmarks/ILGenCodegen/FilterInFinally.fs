/// Phase 3: filter → catch inside finally handlers
/// Tests try/with inside finally blocks
/// Changed: when-guard uses catch instead of filter when inside finally/fault handler
module ILGenCodegen.FilterInFinally

open System
open BenchmarkDotNet.Attributes

[<MemoryDiagnoser>]
type FilterInFinallyBenchmark() =

    [<Benchmark>]
    member _.TryWithInFinally_NoException() =
        let mutable sum = 0
        for i = 0 to 999 do
            try
                sum <- sum + i
            finally
                try
                    sum <- sum + 1
                with
                | :? InvalidOperationException when sum > 0 ->
                    sum <- sum - 1
        sum

    [<Benchmark>]
    member _.TryWithInFinally_WithException() =
        let mutable count = 0
        for _ = 0 to 999 do
            try
                try
                    raise (InvalidOperationException())
                with
                | :? InvalidOperationException ->
                    count <- count + 1
            finally
                try
                    ()
                with
                | :? ArgumentException when count > 0 ->
                    count <- count - 1
        count

    [<Benchmark>]
    member _.TryWithInFinally_GuardHit() =
        let mutable count = 0
        for _ = 0 to 999 do
            try
                count <- count + 1
            finally
                try
                    raise (ArgumentException())
                with
                | :? ArgumentException when count > 0 ->
                    count <- count + 1
        count

    [<Benchmark>]
    member _.SimpleTryFinally() =
        let mutable sum = 0
        for i = 0 to 999 do
            try
                sum <- sum + i
            finally
                sum <- sum + 1
        sum
