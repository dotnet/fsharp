// #Conformance #UnitsOfMeasure #ObjectOrientedTypes 
#light

// Test adding static methods and properties to Measure types.

[<Measure>]
type kg =
    static member Prop = 5
    static member Func x = x + 1

if kg.Prop <> 5 then exit 1
if kg.Func 3 <> 4 then exit 1

ignore 0
