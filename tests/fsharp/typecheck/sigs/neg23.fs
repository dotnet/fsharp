module Test

module DuplicateOverloadUpToErasure1 =  begin

    type SomeClass() =
        
        member this.Foo (x:int->int) = printfn "method 1"
        
        member this.Foo (x:FSharpFunc<int,int>) = printfn "method 2"
end

// This one also checks that duplicate generic methods are detected
module DuplicateOverloadUpToErasure2 =  begin

    type SomeClass() =
       
        member this.Foo (x:'a->int) = printfn "method 1"
        
        member this.Foo (x:FSharpFunc<'b,int>) = printfn "method 2"
end

module DuplicateOverloadUpToErasure3 =  begin

    type SomeClass() =
        
        member this.Foo (x:int*int) = printfn "method 1"
        
        member this.Foo (x:System.Tuple<int,int>) = printfn "method 2"
end

module DuplicateOverloadUpToErasure4 =  begin

    type SomeClass() =
        
        member this.Foo (x:int*int*int*int*int*int*int*int*int) = printfn "method 1"
        
        member this.Foo (x:System.Tuple<int,int,int,int,int,int,System.Tuple<System.Tuple<int,int,int>>>) = printfn "method 2"
end
                                
module DuplicateOverloadUpToErasure5 =  begin

    type SomeClass() =
        
        member this.Foo (x:int*int*int*int*int*int*int) = printfn "method 1"
        
        member this.Foo (x:System.Tuple<int,int,int,int,int,int,System.Tuple<int>>) = printfn "method 2"
end
                                
module DuplicateOverloadUpToErasure6 =  begin

    type SomeClass() =
        
        member this.Foo (x:nativeptr<int>) = printfn "method 1"
        
        member this.Foo (x:nativeint) =  printfn "method 2"
end

module DuplicateOverloadUpToErasure7 =  begin

    type SomeClass() =
        
        member this.Foo (x:(int*int)*int*(int*nativeptr<int>)*int*int) = printfn "method 1"
        
        member this.Foo (x:System.Tuple<System.Tuple<int,int>,int,System.Tuple<int,nativeint>,int,int>) =  printfn "method 2"
end

module TestPrivateInheritance1 = 
    type private IA = interface abstract M : int -> int end
    type C() = 
        interface IA with 
           member obj.M(x) = x + 1


module TestPrivateInheritance2 = 
   type private IA = interface abstract M : int -> int end
   type IB = 
      inherit IA 

module TestCurriedMemberRestrictions = 
    [<AbstractClass>]
    type C() = 
        member x.X0 () () = 1
        member x.X0 (a1,a2) (a3:int)  = 1

        member x.X01 a b = 1
        member x.X01 (a1,a2,a3)  = 1
        
        member x.X02 a ([<System.ParamArrayAttribute>] b) = 1

        member x.X03 a ([<OptionalArgumentAttribute>] b) = 1

        abstract X04 : int -> int -> int
        abstract X04 : int * int -> int -> int

        
        // positive test - no error expected here
        abstract X1 : int -> int -> int
        default x.X1 a b = a + b

        // positive test - no error expected here
        abstract X2 : int * int -> int
        default x.X2 (a1,a3) = 1//(fun b -> 1)
        
        // positive test - no error expected here
        abstract X4 : int -> int * int * int -> int
        default x.X4 a (b1,b2,b3) = 1

    type T1() = 
        member this.F(a, b) = ()
        member this.F a b = ()

    type T2() = 
        member this.F a b = ()
        member this.F(a, b) = ()

module PositiveTests = 
    module CurriedMember = 
        type C() =
            member x.P = 1
            member x.M1 a b = a + b    
            member x.M2 (a,b) c = a + b + c
            
        let c = C()

        // positive test - no error expected here
        let x1 : int = c.M1 3 4
        let x2 : int -> int = c.M1 3
        let x3 : int -> int -> int = c.M1

        // positive test - no error expected here
        let y1 : int = c.M2 (3,4) 4
        let y2 : int -> int = c.M2 (3,4)
        let y3 : int * int -> int -> int = c.M2

    module CurriedExtensionMember = 
        type C() =
            member x.P = 1
            
        module M = 
            type C with 
                member x.M1 a b = a + b    
                member x.M2 (a,b) c = a + b + c

        open M

        let c = C()

        // positive test - no error expected here
        let x1 : int = c.M1 3 4
        let x2 : int -> int = c.M1 3
        let x3 : int -> int -> int = c.M1 
        
        // positive test - no error expected here
        let y1 : int = c.M2 (3,4) 4
        let y2 : int -> int = c.M2 (3,4)
        let y3 : int * int -> int -> int = c.M2


type Frame = 
  class 
  end
module X =
  type Frame with
    member frame.GroupRowsBy(key) = ()
    member frame.GroupRowsBy(key) = ()
    
    // Up to erasure
    member this.Foo (x:int->int) = printfn "method 1"
    member this.Foo (x:FSharpFunc<int,int>) = printfn "method 2"
