// #Regression #Async #Misc
// Regression for 6448 - closures used to not be serializable
// We could check with an Expects=success here but really all we're worried about is no SerializationException is thrown

open System
open Microsoft.FSharp.Control

let inOtherAppDomain (a: Async<unit>) : Async<'T> = 
    let ad = System.AppDomain.CreateDomain "other"
    let self = System.AppDomain.CurrentDomain

    async 
        { 
            ad.DoCallBack(fun _ -> 
                Async.StartWithContinuations(a, continuation=(fun v -> self.DoCallBack(fun _ -> printfn "yay") ),
                                                                       exceptionContinuation=(fun v -> self.DoCallBack(fun _ -> printfn "oh noes" ) ),
                                                                       cancellationContinuation=(fun v -> self.DoCallBack(fun _ -> printfn "cancelled" ) ) ) )
        }
        

let task = async { do printfn "app domain '%s'" System.AppDomain.CurrentDomain.FriendlyName }

exit <|
    try
        task |> Async.RunSynchronously
        inOtherAppDomain task |> Async.RunSynchronously
        0
    with
        | :? System.Runtime.Serialization.SerializationException as e -> 1