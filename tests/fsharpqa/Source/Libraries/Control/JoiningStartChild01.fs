// #Regression #Libraries #Async 
// Regression for FSHARP1.0:5970
// Async.StartChild: race in implementation of ResultCell in FSharp.Core

module M

let Join (a1: Async<'a>) (a2: Async<'b>) = async {
      let! task1 = a1 |> Async.StartChild
      let! task2 = a2 |> Async.StartChild
      
      let! res1 = task1
      let! res2 = task2 
      return (res1,res2) }

let r =
    try 
                     Async.RunSynchronously (Join (async { do! Async.Sleep(30) 
                                                           failwith "fail"
                                                           return 3+3 }) 
                                                  (async { do! Async.Sleep(30) 
                                                           return 2 + 2 } ))
    with _ -> 
                     (0,0)     

exit 0
