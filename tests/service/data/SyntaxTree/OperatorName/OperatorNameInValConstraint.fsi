
[<AutoOpen>]
    module Operators
    /// <summary>Overloaded unary negation.</summary>
    ///
    /// <param name="n">The value to negate.</param>
    ///
    /// <returns>The result of the operation.</returns>
    /// 
    /// <example-tbd></example-tbd>
    /// 
    val inline (~-): n: ^T -> ^T when ^T: (static member ( ~- ): ^T -> ^T) and default ^T: int
