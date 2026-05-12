// #Conformance #TypeConstraints 
// Simple baselines for unmanaged constraint

let testFunc<'a when 'a:unmanaged> (x : 'a) =
    let z = x
    z

testFunc 1 |> ignore

[<Struct>]
type S(x : int) =
    member this.Test = x + 1

testFunc (S(2)) |> ignore

testFunc (System.IntPtr(5)) |> ignore

testFunc (nativeint(-1)) |> ignore

exit 0