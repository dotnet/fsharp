// Erasure tests for units-of-measure: overloading
module Neg52

[<Measure>] type kg
[<Measure>] type s

open System.Collections.Generic

type C<[<Measure>] 'u>() = 
  member this.Meth() = ()

// Emit error if overloaded method signatures are not distinct wrt erasure
type T() =
  member this.Foo(x:float<kg>) = ()
  member this.Foo(x:float) = ()

  member this.Bazz(x:float32<kg>) = ()
  member this.Bazz(x:float32<s>) = ()

  member this.Bar(x:C<kg>) = ()
  member this.Bar(x:C<s>) = ()

  member this.Bing(x:C<'u>) = ()
  member this.Bing(x:C<1>) = ()
