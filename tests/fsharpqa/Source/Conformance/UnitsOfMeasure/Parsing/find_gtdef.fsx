// #Regression #Conformance #UnitsOfMeasure #ReqNOMT 
// Regression test for a bug reported by a user
// In 1.9.6.2, this code snipped passed to fsi used to throw an ICE
// This issue was automagically fixed in Beta1
// The text of the ICE was: FSC(0,0): error FS0192: internal error: find_gtdef: Continuous not found
//<Expects status="success"></Expects>

module Compounding =
    [<Measure>] type Continuous 
    [<Measure>] type Annual 
    // Simple equation Rm = m * (exp (r / m) - 1) 
    let private convert (r: float<'u>, m: float) : float<'u> = 
        m * exp ( r / (m * 1.0<_>) - 1.0) * 1.0<_>
    type Continuous with
        static member ToAnnual (r: float<Continuous>) = 
            convert(r, 1.) * 1.<Annual/Continuous>
