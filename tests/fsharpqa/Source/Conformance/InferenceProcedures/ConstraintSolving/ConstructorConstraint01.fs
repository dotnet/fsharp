// #Regression #Conformance #TypeInference #TypeConstraints 
// Regression test for FSHARP1.0:4850
// Title: incorrectly unsatisfied default constructor constraints

#light

type I<'self> when 'self :  (new : unit -> 'self) = interface
  abstract foo : int
end

type C = class
  val private f : int
  new() = {f= 0}
  interface I<C> with
    member x.foo = x.f + 1
  end
end

type D() =
  let f = 0
  interface I<D> with
    member x.foo = f + 1
    
let c = new C()
let d = new D()

exit 0
