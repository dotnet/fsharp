// #Regression #Libraries #LanguagePrimitives 
// Regression for FSHARP1.0:5794
// New functions to cast dimensionless value to unitful value

module M

open Microsoft.FSharp.Core.LanguagePrimitives

[<Measure>]
type M

let r1 = SByteWithMeasure<M> 1y + 2y<M>
let r2 = Int16WithMeasure<M> 2s - 2s<M>
let r3 = Int32WithMeasure<M> 3 * 3<M>
let r4 = Int64WithMeasure<M> 5L / 5L<M>
let r5 = FloatWithMeasure<M> 11.11 + 1.1<M>
let r6 = Float32WithMeasure<M> 22.22f - 11.11f<M>
let r7 = DecimalWithMeasure<M> 33.33M * 44.44M<M>

exit 0
