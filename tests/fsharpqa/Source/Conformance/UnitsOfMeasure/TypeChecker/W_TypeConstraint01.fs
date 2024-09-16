// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
//Regression test for FSharp1.0#3553 - 1.0<_> being bound to a unit-of-measure variable.
// See also FSHARP1.0:5554 (which is the actual fix for #3553)
//<Expects status="warning" span="(11,7-11,14)" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The unit-of-measure variable 'u has been constrained to be measure 'kg'\.$</Expects>
//<Expects status="warning" span="(11,19-11,26)" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The unit-of-measure variable 'v has been constrained to be measure 'kg'\.$</Expects>

[<Measure>] type kg

let f ( x:float<'u> ) : float<'v> = x * 1.0<_>

if (f 3.5<kg>) <> 3.5<kg> then exit 1

exit 0
