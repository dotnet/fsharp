// #Regression #Conformance #UnitsOfMeasure #ObjectOrientedTypes 
// Regression test for FSharp1.0:3280 - Constructors should be rejected in [<Measure>] types
//<Expects id="FS0904" span="(8,5-8,18)" status="error">Measure declarations may have only static members: constructors are not available</Expects>

#light
[<Measure>]
type kg =
    new () = kg()
    new (i : int) = kg()
    static member Foo() = 1

let objKg = 1.0f<kg>
ignore 1
