// #Conformance #TypeConstraints 
//<Expects status="error" span="(20,11-20,16)" id="FS0001">The type 'S1' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface</Expects>
//<Expects status="error" span="(21,11-21,16)" id="FS0001">The type 'S2' does not support the 'comparison' constraint because it has the 'NoComparison' attribute</Expects>

open System

let testFunc<'a when 'a:comparison> (x : 'a) =
    let o = obj()
    x.Equals(o)

testFunc 1 |> ignore

type S1(x : int) =
    member this.Test = x + 1

[<NoComparison>]
type S2(x : int) =
    member this.Test = x + 1

testFunc (S1(2)) |> ignore
testFunc (S2(2)) |> ignore

exit 0