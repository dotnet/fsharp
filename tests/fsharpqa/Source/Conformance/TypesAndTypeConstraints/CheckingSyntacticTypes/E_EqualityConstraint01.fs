// #Conformance #TypeConstraints 
//<Expects status="error" span="(14,11-14,15)" id="FS0001">The type 'S' does not support the 'equality' constraint because it has the 'NoEquality' attribute</Expects>

let testFunc<'a, 'b when 'a:equality> (x : 'a) =
    let o = obj()
    x.Equals(o)

testFunc 1 |> ignore

[<NoEquality>]
type S(x : int) =
    member this.Test = x + 1

testFunc (S(2)) |> ignore

exit 1