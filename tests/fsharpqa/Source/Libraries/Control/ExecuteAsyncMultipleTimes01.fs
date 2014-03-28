// #Regression #Libraries #Async 
// Regression for FSHARP1.0:5969
// Async.StartChild: error when wait async is executed more than once

module M

let a = async {
                let! a = Async.StartChild(
                            async {
                                do! Async.Sleep(500)
                                return 27
                            })
                let! result  = Async.Parallel [ a; a; a; a ]
                return result
            } |> Async.RunSynchronously

exit 0
