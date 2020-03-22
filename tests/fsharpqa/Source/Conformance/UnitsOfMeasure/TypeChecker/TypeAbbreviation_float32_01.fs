// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
// Regression tests for FSHARP1.0:3427 and 3423
// 
//<Expects status="success"></Expects>
#light

module WithFloat32 =
    [<Measure>] type A
    type Ab = float32<A>        // Type abbreviation
    let a    = 100.0f<A>
    let a_na = a * 2.0f
    let b : Ab = 100.0f<A>
    (if (b*2.0f = 200.0f<A>) then 0 else 1) |> exit
    
