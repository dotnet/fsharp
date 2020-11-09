// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
// Regression test for FSHARP1.0:5554
//<Expects status="success"></Expects>

module Y

[<Measure>]
type Kg 

type M<[<Measure>] 'a> = class end

type T = static member star ( y:'b when 'b :> M<'a>) = 0

let p = 1<Kg>
let m = Unchecked.defaultof<M<Kg>>

T.star(m) |> exit
