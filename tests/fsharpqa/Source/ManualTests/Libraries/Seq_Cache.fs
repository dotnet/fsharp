// Regression test for FSHARP1.0:4997
// Note: this test is not harnessed with the other tests, since it should be executed for a long time (30min or so)
//       I'm just saving the code snippet, in case we want to run it once in a while

// How to run this test:
// - Compile
// - Just leave it running for half an hour or so
// - Verify no crashes 

while true do 
   let s = Seq.cache (seq { 0 .. 1000000 })
   Async.Parallel [ for i in 0 .. 100 -> async { return s |> Seq.truncate (i * 1000) |> Seq.toList } ] |> Async.RunSynchronously |> ignore
   printfn "one iteration done..."
