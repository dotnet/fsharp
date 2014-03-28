namespace Microsoft.FSharp.Control

    open System
    open System.Threading
    open Microsoft.FSharp.Control

#if FX_NO_SYNC_CONTEXT
#else
    type AsyncWorker<'T> =
        new : Async<'T> * ?asyncGroup: CancellationToken -> AsyncWorker<'T>
        member ProgressChanged : IEvent<int> 
        member Error : IEvent<exn> 
        member Completed : IEvent<'T> 
        member Canceled : IEvent<OperationCanceledException> 
        member RunAsync : unit -> bool
        member ReportProgress : int -> unit
        member CancelAsync : ?message:string -> unit

#endif
