// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2708
// ICE when using a known type (int, decimal, etc...) as a unit of measure
//<Expects span="(12,9-12,14)" id="FS0705" status="error">Expected unit-of-measure, not type</Expects>
//<Expects span="(15,17-15,20)" id="FS0705" status="error">Expected unit-of-measure, not type</Expects>
//<Expects span="(18,24-18,27)" id="FS0705" status="error">Expected unit-of-measure, not type</Expects>
//<Expects span="(21,37-21,42)" id="FS0705" status="error">Expected unit-of-measure, not type</Expects>
//<Expects span="(24,38-24,41)" id="FS0705" status="error">Expected unit-of-measure, not type</Expects>
[<Measure>] type Kg

module M1 = 
    1.0<float>  // error

module M2 = 
    let x = 1.0<int>  // error

module M3 =
    let f2 ( x : float<int>) = x    // error

module M4 =    
    let f4 (x : 'a when 'a :> float<float>) = x     // error

module M5 = 
    type T<[<Measure>] 'a>(x : float<int>, y : float<_>) = class end    // error

