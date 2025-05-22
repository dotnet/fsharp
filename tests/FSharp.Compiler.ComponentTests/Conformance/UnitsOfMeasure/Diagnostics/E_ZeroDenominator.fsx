// #Regression #Conformance #UnitsOfMeasure #Diagnostics #RatExp
//<Expects id="FS0625" span="(8,19-8,20)" status="error">Denominator must not be 0 in unit-of-measure exponent</Expects>
//<Expects id="FS0625" span="(9,21-9,22)" status="error">Denominator must not be 0 in unit-of-measure exponent</Expects>
#light
 
[<Measure>] type m

let e1 = 2.0<m^(3/0)>
let e2 :float<m^-(4/0)> = 2.0<_>


