module NetTfmResolution.Program

// Runtime smoke for the shipped net-TFM FSharp.Core asset (e2e-2, step 5).
//
// Exercises a genuinely net-path public member: TaskBuilderBase.Using for an IAsyncDisposable
// resource, guarded in FSharp.Core behind `#if NETSTANDARD2_1 || NET` (see tasks.fs / tasks.fsi).
// If the consumer had bound a netstandard2.0 asset, this `use!` overload would not exist. Loading
// and running it therefore proves the widened net asset is what got resolved AND that it works.

open System
open System.Threading.Tasks

type private AsyncResource(recorder: string list ref) =
    interface IAsyncDisposable with
        member _.DisposeAsync() =
            recorder.Value <- "disposed" :: recorder.Value
            ValueTask.CompletedTask

let private run () =
    let recorder = ref []
    let work =
        task {
            use _res = new AsyncResource(recorder) // net/ns2.1-path TaskBuilderBase.Using(IAsyncDisposable)
            return 42
        }
    let result = work.GetAwaiter().GetResult()
    result, recorder.Value

[<EntryPoint>]
let main _ =
    let result, disposals = run ()
    if result = 42 && disposals = [ "disposed" ] then
        printfn "NetTfmResolution OK: widened IAsyncDisposable task member executed."
        0
    else
        eprintfn "NetTfmResolution FAILED: result=%d disposals=%A" result disposals
        1
