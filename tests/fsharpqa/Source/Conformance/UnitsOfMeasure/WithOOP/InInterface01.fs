// #Regression #Conformance #UnitsOfMeasure #ObjectOrientedTypes 
#light 

// Regression for FSB-3570
// ICE when type parameters and units-of-measure are mixed in the same hierarchy

type IA = 
  interface
    abstract Foo<[<Measure>]'a> : unit -> obj
  end

type A() =
  class
    interface IA with
      member obj.Foo<[<Measure>] 'a>() = failwith ""
    end
  end


[<Measure>]
type MeasureType


let testResult =
    try
        let instanceA = new A()
        let instanceIA = instanceA :> IA
        let _ = instanceIA.Foo<MeasureType>()
        false
    with 
    | _ -> true

if testResult = false then
    exit 1

exit 0
