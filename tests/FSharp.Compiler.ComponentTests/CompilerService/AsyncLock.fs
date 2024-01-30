module CompilerService.AsyncLock

open Internal.Utilities.Collections

open Xunit
open System.Threading.Tasks


[<Fact>]
let ``Async lock works`` () =
    task {
        use lock = new AsyncLock()

        let mutable x = 0

        let job () = task {
            let y = x
            do! Task.Delay(10)
            x <- y + 1
        }

        let jobs = [ for _ in 1..100 -> lock.Do job ]
        let! _ = Task.WhenAll(jobs)

        Assert.Equal(100, x)
    }