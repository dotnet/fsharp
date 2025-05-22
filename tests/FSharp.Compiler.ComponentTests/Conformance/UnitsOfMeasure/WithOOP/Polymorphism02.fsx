// #Regression #Conformance #UnitsOfMeasure #ObjectOrientedTypes 
// Regression test for FSHARP1.0:3687
// ICE in Units of Measure + Polymorphism
//<Expects status="success"></Expects>

#light
module Test
type Unit<[<Measure>] 'a >() =
    abstract Factor : unit -> float
    default this.Factor () = 1.0
[<Measure>] type kg

let objUnit = new Unit<kg>()
if objUnit.Factor() <> 1.0 then exit 1

ignore 0
