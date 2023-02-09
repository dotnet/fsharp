(* SHOULD GIVE A NICE ERROR INCLUDING THE NAME "Ascii": *)
module Test
let testError b = System.Text.Encoding.Ascii.GetString b 

[<Sealed>]
type SealedType() = 
    class
        member x.P = 1
    end
    
[<Sealed>]
type UnnecessarilySealedStruct = 
    struct
        member x.P = 1
    end
    
[<Sealed>]
type BadSealedInterface = 
    interface
        abstract P : int
    end
    
[<Sealed>]
type BadSealedAbbreviatedType = System.Object 

[<Sealed>]
type UnnecessarilySealedDelegate = delegate of int -> int
        
type BadExtensionOfSealedType() = 
    class
        inherit SealedType()
        member x.P = 1
    end
//Moved to FSHARPQA suite     
//let WarningOnHashOfSealedType (x: #SealedType) = x

type BadImmediateInheritance() = 
    class
        inherit BadImmediateInheritance()
    end

type BadImmediateInheritance1() = 
    class
        inherit BadImmediateInheritance2()
    end
and BadImmediateInheritance2() = 
    class 
        inherit BadImmediateInheritance1()
    end
    
 
module BadStructTest1 = begin
    type BadStruct = 
        struct
            val x : BadStruct
        end    
end

module BadStructTest2 =  begin
    type BadStruct1 = 
        struct
            val x : BadStruct2
        end    
    and BadStruct2 =
        struct
            val x : BadStruct1
        end    
end

module BadStructTest3 =  begin
    type One<'a> = 
        struct
            val x : 'a
        end    
    type BadStruct =
        struct
            val x : One<BadStruct>
        end    

    type BadAbbreviation = One<BadAbbreviation>

    type GoodStruct =
        struct
            val x : One<int>
        end    
end


module NegativeTest1 = begin
    type X = option<X>
end

module NegativeTest2 = begin
    type X = A<X> 
    and A<'a> = A of int
end

module NegativeTest3 = begin
    type X = A<X> 
    and A<'a> = int
end

module Neg4 = begin
    type Y = Y * Y    
end

module Neg5 = begin
    type Y = Y -> Y    
end

module Neg6 = begin
    type Y = F<Y>
    and F<'a> = 'a -> 'a
end

module Neg7 = begin
    type Y = F<Y>
    and F<'a> = Y -> Y
end

module Neg8 = begin
    type A = F<int>
    and F<'a> = G<'a>
    and G<'a> = F<'a>
end

module BadConstraint = begin
    let f<'a when 'a : (new : string -> unit)> () = new 'a()
end

module BadRecursiveTypeDefinitionSingleRecursion = begin

    type BadType0 =
      struct
        [<DefaultValueAttribute>]
        static val mutable private x : int  // structs require at least one field
      end

    let _ = BadType0()

    type BadType1<'a> =
      struct
        [<DefaultValueAttribute>]
        static val mutable private x : BadType1<'a[]>
        val Y : int
      end

    let _ = BadType1<int32>()
     
    type BadType2<'a> =
      struct
        [<DefaultValueAttribute>]
        static val mutable private X : BadType2<int>
        val Y : int
      end
     
    let _ = BadType2<int32>()

    type BadType3<'a> =
      struct
        [<DefaultValueAttribute>]
        val mutable X : BadType3<int>
        val Y : int
      end
     
    let _ = BadType3<int32>()

    

    type GoodType1<'a> =
      struct
        [<DefaultValueAttribute>]
        static val mutable private X : GoodType1<'a>
        val Y : int
      end
     
    let _ = GoodType1<int32>()
 
    type GoodType2 =
      struct
        [<DefaultValue(false)>]
        val mutable X : GoodBox2<GoodType2>
      end
    and GoodBox2<'T> = 
      class
        val v : 'T
      end
     
    let _ = GoodType2()
end

module BadRecursiveTypeDefinitionsWithAbbrev = begin


    type BadType1<'a> =
      struct
        [<DefaultValueAttribute>]
        static val mutable private x : Naught1<BadType1<'a[]>>
        val Y : int
      end
    and Naught1<'a> = 'a

    let _ = BadType1<int32>()
     
    type BadType2<'a> =
      struct
        [<DefaultValueAttribute>]
        static val mutable private X : Naught2<BadType2<int>>
        val Y : int
      end
    and Naught2<'a> = 'a
     
    let _ = BadType2<int32>()

    type BadType3<'a> =
      struct
        [<DefaultValueAttribute>]
        val mutable X : Naught3<BadType3<int>>
        val Y : int
      end
    and Naught3<'a> = 'a
     
    let _ = BadType3<int32>()

    
    type GoodType1<'a> =
      struct
        [<DefaultValueAttribute>]
        static val mutable private X : Naught5<int>
        val Y : int
      end
    and Naught5<'a> = 'a
      
    let _ = GoodType1<int32>()
 
    type GoodType2 =
      struct
        [<DefaultValue(false)>]
        val mutable X : GoodBox2<GoodType2>
      end
    and GoodBox2<'T> = 
      class
        val v : 'T
      end
     
    let _ = GoodType2()

end








































module PositiveTests = 

    [<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>]
    type X3 =  { r : int }

    [<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>]
    [<AbstractClass>]
    type X2 = 
        abstract M : unit -> 'a

    [<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)>]
    type X1 =  { r : int }

module NegativeTests = 
    [<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>]
    type X1 = 
        abstract M : unit -> 'a


    [<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>]
    [<AbstractClass>]
    type X2() = 
        abstract M : unit -> 'a

    [<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>]
    type X4 =  R1 | R2

    [<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>]
    type X5 =  R1 = 1 | R2 = 2

    [<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>]
    type X6 =  delegate of int -> int 


module Regression_3417a =
    // This type should be rejected due to a struct cycle (pre-fix, observed loop in fsi)
    // Prefix: it looped
    type BadType4 =
      struct
        [<DefaultValueAttribute>]
        val mutable X : BadBox4<BadType4>
      end
    and BadBox4<'T> = 
      struct
        val v : 'T
      end
    let _ = BadType4()

module Regression_3417b =
    // This type should be rejected due to a struct cycle (pre-fix, observed loop in fsi)     
    // Prefix: it looped
    type BadType4 =
      struct
        [<DefaultValueAttribute>]
        val mutable X : Naught4<BadBox4<BadType4>>
      end
    and BadBox4<'T> = 
      struct
        val v : 'T
      end
    and Naught4<'a> = 'a
    let _ = BadType4()





















module NonMonotonicByrefParameter =
  let f (x:byref<int>) = ()

  let g x = f x
  
module AmbiguousOverload = 
  type C = 
      static member M1 (x: int) = 1
      static member M1 (x: string) = 2

  let f x = C.M1 x  
  
module CheckErrorForNonUniformGeneric =
    type Foo<'a>(bar : Bar<'a>) =
        member this.Blah() = bar.Foo() 
    and Bar<'a>() =
        member this.Foo() = ()    // expect NO error here

module CheckNoErrorForNonUniformGenericWithExplicitSignature =
    type Foo<'a>(bar : Bar<'a>) =
        member this.Blah() = bar.Foo() 
    and Bar<'a>() =
        member this.Foo() : unit = ()    // expect NO error here 

module CHeckWarningForIncompletePatternMatchInForLoop = 
    let f () = 
        for 1 in [1;2;3] do ()

module CHeckErrorForNamedArgumentToDatatype = 
    type C = 
        | C1 of bool * bool
        | C2 of bool
    
    let x = 1
    let c1 = C1 (x=1, x = 1) // error no longer expected here
    let c2 = C2 (x=1) // no error expected

