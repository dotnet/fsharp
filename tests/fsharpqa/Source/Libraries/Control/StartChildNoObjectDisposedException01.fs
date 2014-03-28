// #Regression #Libraries #Async 
// Regression for FSHARP1.0:5971
// Async.StartChild: ObjectDisposedException

module M

let shortVersion(args: string []) =
    let b = async {return 5} |> Async.StartChild
    printfn "%A" (b |> Async.RunSynchronously |> Async.RunSynchronously)
    (0)

exit 0
