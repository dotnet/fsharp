// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2922
//<Expects id="FS0064" span="(5,30-5,31)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The unit-of-measure variable 'a has been constrained to be measure '1'</Expects>
module M
let f_2922 (x:float<'a>) = x + 1.0

