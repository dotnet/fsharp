module Neg08
let l = ref []
exception StackEmpty

let push a x = l:=x:: !l

let topEnvTable1 = new System.Collections.Generic.List()
  
let topEnvTable2 = new System.Collections.Generic.List<int,string>()

#light 

// Check non-generalization of escaping implicit ctor tyvars
let x2 = ref []

type C<'b>() = 
    member a.Z() : 'b list = !x2
    
    member a.Z2() = a.Z()

exception A = System.Object

type Test = class
     val test: int
     new(test) = {test = test}
end

let t : Test = {test = 0}

let q = { } 

module TestMissingAndExtraOverrides = begin
    type B = class
      new () = { }
      
      abstract A : int
      abstract M : unit -> int
    end

    type C = class
      inherit B

      override x.A
        with get() = 3
        // Check we can't also provide a setter when the abstract only has a getter
        and set(v : int) = (invalidArg "v" "C.A.set" : unit) 

      override x.N() = 2
      override x.M() = 3
    end
end

module FSharp1_0_Bug1221 = begin
    type Foo = delegate of int * int -> int
    { new Foo with
      member x.Invoke (x,y) = x + y }
end
    
module FSharp1_0_Bug992= begin
   type Type = | P = 1
   let _ = Type.P.value__
end


module FSharp1_0_Bug1423 = begin

    type Variable() =
        class
           member x.Name with set(v:string) = ()
        end

    type Variable<'a>() =
        class
            inherit Variable()
            static member Random(y:Variable<'a>) = new Variable<'a>()
        end
        
    let x : Variable<int> = failwith ""
    Variable.Random<float> (x, Name = "m_")
end
