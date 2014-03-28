namespace Microsoft.FSharp.Math

#nowarn "42"

module Measure =

    let infinity<[<Measure>] 'u> : float<'u> = LanguagePrimitives.FloatWithMeasure System.Double.PositiveInfinity
    let nan<[<Measure>] 'u> : float<'u> = LanguagePrimitives.FloatWithMeasure System.Double.NaN

    let infinityf<[<Measure>] 'u> : float32<'u> = LanguagePrimitives.Float32WithMeasure System.Single.PositiveInfinity
    let nanf<[<Measure>] 'u> : float32<'u> = LanguagePrimitives.Float32WithMeasure System.Single.NaN

