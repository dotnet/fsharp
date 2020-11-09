// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2732
//<Expects status="success"></Expects>
// Note: you need the 'step' in the range in order to see the warning!

[<Measure>] type Kg

let v1 = [1.0<Kg> .. 1.0<Kg> .. 3.0<Kg>] |> Seq.sum

(if v1 = 6.0<Kg> then 0 else 1) |> exit
