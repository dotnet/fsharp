// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2732
//<Expects status="notin">Floating point ranges are experimental</Expects>
//<Expects status="success"></Expects>

[<Measure>] type Kg

let v1 = [1.0<Kg> .. 2.0<Kg> .. 5.0<Kg>] |> Seq.item 1

(if v1 = 3.0<Kg> then 0 else 1) |> ignore


