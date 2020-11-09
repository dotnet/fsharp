// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2708
// ICE when using a known type (int, decimal, etc...) as a unit of measure

//<Expects span="(14,9-14,14)"  status="error" id="FS0705">Expected unit-of-measure, not type</Expects>
//<Expects span="(17,17-17,20)" status="error" id="FS0705">Expected unit-of-measure, not type</Expects>
//<Expects span="(20,24-20,27)" status="error" id="FS0705">Expected unit-of-measure, not type</Expects>
//<Expects span="(23,37-23,42)" status="error" id="FS0705">Expected unit-of-measure, not type</Expects>
//<Expects span="(26,38-26,41)" status="error" id="FS0705">Expected unit-of-measure, not type</Expects>

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

