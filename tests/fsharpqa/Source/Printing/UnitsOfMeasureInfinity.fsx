// #Regression #NoMT #Printing #RequiresPowerPack 
// Regression test for FSHARP1.0:6082
//<Expects status="success">val it : float<m/s> = infinity</Expects>
//<Expects status="success">val it : float32<m/s> = infinityf</Expects>
//<Expects status="success">val it : float<m/s> = -infinity</Expects>
//<Expects status="success">val it : float32<m/s> = -infinityf</Expects>

[<Measure>] type m
[<Measure>] type s;;

Microsoft.FSharp.Math.Measure.infinity<m/s>;;

Microsoft.FSharp.Math.Measure.infinityf<m/s>;;

-Microsoft.FSharp.Math.Measure.infinity<m/s>;;

-Microsoft.FSharp.Math.Measure.infinityf<m/s>;;

exit 0;;
