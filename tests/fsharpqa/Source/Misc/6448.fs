// #Misc 


module M
open Microsoft.FSharp.Control

type Result<'T>() = 
    inherit System.MarshalByRefObject() 
    let cell = AsyncResultCell<'T>()
    member x.AsyncResult = cell.AsyncResult
    member x.RegisterResult res = cell.RegisterResult res

let inOtherAppDomain (a: Async<'T>) : Async<'T> = 
    let ad = System.AppDomain.CreateDomain "other"
    let self = System.AppDomain.CurrentDomain
    let result = Result<'T>()
    async { ad.DoCallBack(fun _ -> Async.StartWithContinuations(a, continuation=(fun v -> self.DoCallBack(fun _ -> result.RegisterResult (AsyncOk v)) ),
                                                                   exceptionContinuation=(fun v -> self.DoCallBack(fun _ -> result.RegisterResult (AsyncException v)) ),
                                                                   cancellationContinuation=(fun v -> self.DoCallBack(fun _ -> result.RegisterResult (AsyncCanceled v)) ) ) )
            return! result.AsyncResult }
    

let task = async { do printfn "app domain '%s'" System.AppDomain.CurrentDomain.FriendlyName }

task |> Async.RunSynchronously
inOtherAppDomain task |> Async.RunSynchronously
