// <testmetadata>
// { "optimization": { "reported_in": "#6499", "reported_by": "@atlemann,@cartermp", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>
module Foo =

    let bar (f : 'a -> 'a -> 'a) (a:'a array) (b:'a array) =
        Array.map2 f a b

    let barInPlace (f : 'a -> 'a -> 'a) (a:'a array) (b:'a array) =
        for i in 0..b.Length - 1 do
            b.[i] <- f a.[i] b.[i]

    let inline barInPlaceInline (f : 'a -> 'a -> 'a) (a:'a array) (b:'a array) =
        for i in 0..b.Length - 1 do
            b.[i] <- f a.[i] b.[i]

    let barInPlaceOptClosure (f : 'a -> 'a -> 'a) (a:'a array) (b:'a array) =
        let func = OptimizedClosures.FSharpFunc<'a, 'a, 'a>.Adapt(f)
        for i in 0..b.Length - 1 do
            b.[i] <- func.Invoke(a.[i], b.[i])

    let inline barInPlaceOptClosureInline (f : 'a -> 'a -> 'a) (a:'a array) (b:'a array) =
        let func = OptimizedClosures.FSharpFunc<'a, 'a, 'a>.Adapt(f)
        for i in 0..b.Length - 1 do
            b.[i] <- func.Invoke(a.[i], b.[i])

    let barFunInside (a:single array) (b:single array) =
        let f = (fun a b -> a * 0.7f + b * 0.3f)
        for i in 0..b.Length - 1 do
            b.[i] <- f a.[i] b.[i]
            
System.Console.WriteLine()