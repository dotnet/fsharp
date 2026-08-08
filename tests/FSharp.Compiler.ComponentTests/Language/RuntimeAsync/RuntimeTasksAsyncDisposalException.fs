// Minimal repro: suspending with AsyncHelpers.Await inside the *handler* of an
// exception-handling region of a __runtimeAsync method. This is what `use` on an
// IAsyncDisposable lowers to (the DisposeAsync await sits in the finally).
//
// Today this compiles cleanly but terminates the process at execution
// (0xC0000409), so the component test compiles this file without running it.
// Awaiting in the try *body* with a plain finally works; awaiting inside the
// finally itself does not.
module RuntimeAsyncAwaitInExceptionRegion

open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let run () : Task<int> =
    StateMachineHelpers.__runtimeAsync (
        try
            1
        finally
            AsyncHelpers.Await(Task.Delay(1))
    )

[<EntryPoint>]
let main _ =
    if (run ()).GetAwaiter().GetResult() = 1 then 0 else 1
