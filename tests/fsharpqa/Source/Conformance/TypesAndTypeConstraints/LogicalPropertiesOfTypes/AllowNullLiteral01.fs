// #Conformance #TypeConstraints 
// Verify [<AllowNullLiteral>] does what it should.

[<AllowNullLiteral>]
type Foo(x : int) =
    member this.Value = x


let fooArray = [| (null : Foo); new Foo(42) |]
let fooList  = [  (null : Foo); new Foo(52)  ]

fooArray |> Array.iter (function null -> printfn "null" | x -> printfn "Foo(%d)" x.Value)
fooList  |> List.iter  (function null -> printfn "null" | x -> printfn "Foo(%d)" x.Value)

exit 0
