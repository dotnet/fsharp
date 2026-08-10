module NetTfmResolution.Program

// Runtime smoke: the `use` on an IAsyncDisposable resource (guarded `#if NETSTANDARD2_1 || NET`) is
// absent from the netstandard2.0 asset, so executing it proves the widened net asset was resolved.

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
            use _res = new AsyncResource(recorder)
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
