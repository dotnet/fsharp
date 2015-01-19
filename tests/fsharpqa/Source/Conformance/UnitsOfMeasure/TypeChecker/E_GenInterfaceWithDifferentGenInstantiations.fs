// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
// Regression test for FSHARP1.0:4782
// It is illegal to implement or inherit the same interface at different generic instantiations
//<Expects status="error" id="FS0443" span="(13,6-13,8)">This type implements the same interface at different generic instantiations 'IA<kg>' and 'IA<'b>'\. This is not permitted in this version of F#</Expects>

[<Measure>] type kg

type IA<[<Measure>] 'b> = 
  interface
    abstract Foo : float <'b> -> int
  end
  
type IB<[<Measure>] 'b> = 
  interface
    inherit IA<'b>
    inherit IA<kg>
  end
 
type A() =
  class
    interface IB<kg> with
      member obj.Foo (x : float<kg>) = 5
    end
  end
