// #Regression #Libraries #Async 
// Regression for FSHARP1.0:5972
// Async.StartChild: fails to install trampolines properly
module M

let r =
    async {
        let! a = Async.StartChild(
                    async { 
                        do! Async.Sleep(500)
                        return 5 
                    }
                  )
        let! b = a
        for i in 1..10000 do // 10000 > bindHijackLimit
            ()
    } |> Async.RunSynchronously

exit 0
