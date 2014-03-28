// #Conformance #TypeConstraints 
// Simple baselines for unmanaged constraint
//<Expects status="error" span="(17,11-17,15)" id="FS0001">A generic construct requires that the type ''a S' is an unmanaged type</Expects>
//<Expects status="error" span="(20,10-20,11)" id="FS0001">A generic construct requires that the type 'D' is an unmanaged type</Expects>
//<Expects status="error" span="(23,11-23,14)" id="FS0001">A generic construct requires that the type 'C' is an unmanaged type</Expects>

let testFunc<'a when 'a:unmanaged> (x : 'a) =
    let z = x
    z

testFunc 1 |> ignore

[<Struct>]
type 'a S(x : int) =
    member this.Test = x + 1

testFunc (S(2)) |> ignore

type D = A | B
testFunc A |> ignore

type C() = class end
testFunc (C()) |> ignore

exit 1