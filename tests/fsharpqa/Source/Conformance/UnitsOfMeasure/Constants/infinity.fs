// #Regression #Conformance #UnitsOfMeasure #Constants #RequiresPowerPack 
// Regression test for FSHARP1.0:6082

[<Measure>] type m
[<Measure>] type s

if 1.<m>/0.<s> <> Microsoft.FSharp.Math.Measure.infinity<m/s> then exit 1

if 1.0f<m>/0.0f<s> <> Microsoft.FSharp.Math.Measure.infinityf<m/s> then exit 1

if -1.<m>/0.<s> <> -Microsoft.FSharp.Math.Measure.infinity<m/s> then exit 1

if -1.0f<m>/0.0f<s> <> -Microsoft.FSharp.Math.Measure.infinityf<m/s> then exit 1

exit 0
