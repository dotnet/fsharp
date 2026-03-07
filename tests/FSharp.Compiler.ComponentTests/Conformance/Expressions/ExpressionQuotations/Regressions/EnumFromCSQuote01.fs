// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:6007 
// Enums were causing ArgumentExceptions when quoted
// Use an enum from C# (int based and non-int based)

module T

let q = <@ S.Test.Days.Sun @>
let q1 = <@ S.Test.Range.Max @>

exit 0
