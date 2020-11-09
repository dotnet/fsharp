// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
// Regression tests for FSHARP1.0:3427 and 3423
// 
//<Expects status="success"></Expects>
#light

module WithDecimal =
    [<Measure>] type A
    type Ab = decimal<A>        // Type abbreviation
    let a    = 100.0M<A>
    let a_na = a * 2.0M         // Bug #3427
    let b : Ab = 100.0M<A>
    (if (b*2.0M = 200.0M<A>) then 0 else 1) |> exit
