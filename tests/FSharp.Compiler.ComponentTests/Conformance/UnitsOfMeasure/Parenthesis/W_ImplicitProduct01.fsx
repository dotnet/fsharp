// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2662,2745
// Make sure we can use ( and ) in Units of Measure
//<Expects id="FS0632" span="(11,25-11,32)" status="warning">Implicit product of measures following /</Expects>
#light

[<Measure>] type m
[<Measure>] type s

module M =
    let velocity1 = 1.0<m / s s>    // Warning: Implicit product of measures following /
