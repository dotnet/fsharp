namespace Microsoft.FSharp.Control

    open System
    open System.Threading
    open System.IO
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections

#if FX_NO_SYNC_CONTEXT
#else
    type AsyncWorker<'T>(p : Async<'T>,?cancellationToken) = 

        let cts =
            match cancellationToken with
            | None -> new CancellationTokenSource()
            | Some token ->
                  let cts = new CancellationTokenSource()
                  CancellationTokenSource.CreateLinkedTokenSource(token,cts.Token)

        let mutable syncContext : SynchronizationContext = null

        // A standard helper to raise an event on the GUI thread

        let raiseEventOnGuiThread (event:Event<_>) args =
            syncContext.Post((fun _ -> event.Trigger args),state=null)

        // Trigger one of the following events when the iteration completes.
        let completed = new Event<'T>()
        let error     = new Event<_>()
        let canceled   = new Event<_>()
        let progress  = new Event<int>()

        let doWork() = 
            Async.StartWithContinuations
                ( p, 
                  (fun res -> raiseEventOnGuiThread completed res),
                  (fun exn -> raiseEventOnGuiThread error exn),
                  (fun exn -> raiseEventOnGuiThread canceled exn ),cts.Token)
                                
        member x.ReportProgress(progressPercentage) = 
            raiseEventOnGuiThread progress progressPercentage
        
        member x.RunAsync()    = 
            match syncContext with 
            | null -> ()
            | _ -> invalidOp "The operation is already in progress. RunAsync can't be called twice"

            syncContext <- 
                match SynchronizationContext.Current with 
                | null -> new SynchronizationContext()
                | ctxt -> ctxt

            ThreadPool.QueueUserWorkItem(fun args -> doWork())

        member x.CancelAsync(?message:string) = 
            cts.Cancel()

        member x.ProgressChanged     = progress.Publish
        member x.Completed  = completed.Publish
        member x.Canceled   = canceled.Publish
        member x.Error      = error.Publish


#endif
