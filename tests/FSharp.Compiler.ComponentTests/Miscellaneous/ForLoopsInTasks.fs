let mutable v = 0

let incr () = 
    task {
        System.Threading.Interlocked.Increment(&v) |> ignore
    }

let t () =
    task {
        for i in 1 .. 10 .. 1000 do
            do! incr ()
    }
t().Wait()

if v <> 100 then
    failwith $"Expected: 100; got: {v}"