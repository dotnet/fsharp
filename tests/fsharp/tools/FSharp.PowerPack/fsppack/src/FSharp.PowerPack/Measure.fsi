namespace Microsoft.FSharp.Math

#nowarn "42"

module Measure =

    /// <summary>Version of <c>System.Double.PositiveInfinity</c> that is generic in its units-of-measure</summary>
    [<GeneralizableValue>]
    val infinity<[<Measure>] 'u> : float<'u>

    /// <summary>Version of <c>System.Double.NaN</c> that is generic in its units-of-measure</summary>
    [<GeneralizableValue>]
    val nan<[<Measure>] 'u> : float<'u> 

    /// <summary>Version of <c>System.Single.PositiveInfinity</c> that is generic in its units-of-measure</summary>
    [<GeneralizableValue>]
    val infinityf<[<Measure>] 'u> : float32<'u> 

    /// <summary>Version of <c>System.Single.NaN</c> that is generic in its units-of-measure</summary>
    [<GeneralizableValue>]
    val nanf<[<Measure>] 'u> : float32<'u>

