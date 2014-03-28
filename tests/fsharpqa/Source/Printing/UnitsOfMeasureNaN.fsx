// #Regression #NoMT #Printing #RequiresPowerPack 
// Regression test for FSHARP1.0:6082
//<Expects status="success">val it : float<m/s> = nan</Expects>
//<Expects status="success">val it : float32<m/s> = nanf</Expects>
 
[<Measure>] type m
[<Measure>] type s;;

Microsoft.FSharp.Math.Measure.nan<m/s>;;

Microsoft.FSharp.Math.Measure.nanf<m/s>;;

exit 0;;

