open System

type C() =
    member _.M(a: Action<int>) = printfn "action"
    member _.M(f: Func<int, unit>) = printfn "func"
    
let c = C()

// https://github.com/dotnet/fsharp/issues/11534
// we may want a warning here, or maybe an error proper at some point
c.M(fun _ -> ())