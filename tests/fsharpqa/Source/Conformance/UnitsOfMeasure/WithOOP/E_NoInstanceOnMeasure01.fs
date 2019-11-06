// #Regression #Conformance #UnitsOfMeasure #ObjectOrientedTypes 


// Test error when adding instance methods to Measure types.
//<Expects id="FS0897" span="(9,5)" status="error">Measure declarations may have only static members$</Expects>

[<Measure>]
type kg =
    member this.Foo() = 5

exit 1
