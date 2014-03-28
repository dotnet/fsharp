// #Regression #Conformance #UnitsOfMeasure #Constants #RequiresPowerPack 
// Regression test for FSHARP1.0:6082

[<Measure>] type m
[<Measure>] type s

// Note: nan <> nan so there is not much we can do here
if 0.<m>/0.<s> = Microsoft.FSharp.Math.Measure.nan<m/s> then exit 1
if 0.f<m>/0.f<s> = Microsoft.FSharp.Math.Measure.nanf<m/s> then exit 1

exit 0
