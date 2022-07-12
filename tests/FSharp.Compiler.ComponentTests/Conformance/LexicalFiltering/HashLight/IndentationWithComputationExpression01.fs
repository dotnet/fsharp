// #Regression #Conformance #LexFilter 
// Regression test for FSHARP1.0:6085

module M.N.A

async {
    let! a = Async.StartChild(
                async { 
                    do! Async.Sleep(500)
                    return 5 
                }
              )
    let! b = a
    for i in 1..10000 do
        ()
} |> Async.RunSynchronously
