module Async 

open BenchmarkDotNet.Attributes

[<SimpleJob(launchCount = 2, warmupCount = 1, iterationCount = 2)>]
[<GcServer(true)>]
[<MemoryDiagnoser>]
[<MarkdownExporterAttribute.GitHub>]
type AsyncWhileMemoryBench() =

  [<Params(1000, 10000)>]
  member val Length = 0 with get, set

  [<Benchmark>]
  member x.Run() =
    async {
      let mutable i = 0
      while i < x.Length do
        i <- i + 1
      return i
    } |> Async.StartAsTask