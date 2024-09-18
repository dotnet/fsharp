#light
#r "WindowsBase"
#r "PresentationFramework"
#r "System.Xaml"
open System.Threading
open System.Windows.Threading
open System.Net
open Microsoft.FSharp.Control.WebExtensions

let dsc = new DispatcherSynchronizationContext()
SynchronizationContext.SetSynchronizationContext(dsc)
let thread = System.Threading.Thread.CurrentThread

let app = new System.Windows.Application()

async {
    let req = WebRequest.Create("http://foo.bar.baz")
    do!
        async {
            try
                let! _ =  req.AsyncGetResponse()
                return ()
            with
            |   e -> return ()
        }
    if not (System.Threading.Thread.CurrentThread.Equals(thread)) then
        printfn "Test Failed"
        app.Shutdown(128)
    else
        printfn "Test Passed"
        System.IO.File.WriteAllText("test.ok","ok")
        app.Shutdown(0)
} |> Async.StartImmediate

[<System.STAThread>]
do ()


app.Run()
