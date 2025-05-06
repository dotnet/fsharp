// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2662
// Regression test for FSHARP1.0:2921
// Make sure we can use ( and ) in Units of Measure
//<Expects id="FS0464" span="(24,33-24,39)" status="warning">This code is less generic than indicated by its annotations\. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless\. Consider making the code generic, or removing the use of '_'</Expects>
module MM

[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s
module M =
    let velocity2 = 1.0<m / s * s>      + 1.0<m>
    let velocity3 = 1.0<m / (s  s)>     + 1.0<m/s^2>
    let velocity4 = 1.0<m / (s * s)>    + 1.0<m/s^2>
    let velocity5 = 1.0<m / (s / s)>    + 1.0<m>
    let velocity6 = 1.0<m / ( / s)>     + 1.0<m s>
    let velocity8 = 1.0<m / ( s / / / / / / / / / / / / / / / / / / s)>  + 1.0<m (s^-2)> + 1.0<m / s^2>
    let velocity10 = 1.0<m / ( (s) / s)>  + 1.0<m>
    let velocity11 = 1.0<m / ( (s)(s) )>  + 1.0<m/s^2>
    let v1 = 1.0< (s)/(m) >               + 1.0<(/ m) (/ / s)> + 1.0<s/m>
    let v4 = 1.0<(s (s))>                 + 1.0<(s)(s)> + 1.0<s^2>
    let N = 1.0<Kg * m / s^2> +  1.0<(Kg) * m / s^2> +  1.0<(Kg) * (m) / s^2> + 1.0<((Kg) * (m)) / s^2> + 1.0<(((((Kg) * (m)))) / (s^2))>
    let v2 = 1.0< (s)(m) > + 1.0< ((s)(m)) > + 1.0<m s>
    let N3 = 1.0<(/s) / (/s)> + 1.0<_> + 1.0        // expected warning: see bug #2921
    let N4 = 1.0<(/s) * (/s)> + 1.0< /(s^2)> + 1.0< (/s) (/s)> + 1.0< (/s) * (/s)>
    let velocity9 = 1.0<m / (s)^2>  + 1.0<m / s^2>
