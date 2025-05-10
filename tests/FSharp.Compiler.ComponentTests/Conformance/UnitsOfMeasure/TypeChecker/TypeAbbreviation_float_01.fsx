// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
// Regression tests for FSHARP1.0:3427 and 3423
// 
//<Expects status="success"></Expects>

#light

module WithFloat =
    [<Measure>] type A
    type Ab = float<A>          // Type abbreviation
    let a    = 100.0<A>
    let a_na = a * 2.0
    let b : Ab = 100.0<A>

    (if (b*2.0 = 200.0<A>) then 0 else 1) |> ignore
    
