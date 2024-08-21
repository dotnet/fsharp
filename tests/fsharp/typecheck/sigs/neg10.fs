namespace N

type x = int


namespace N

//type x = A | B

type IM = interface end
type C1 = class inherit IM end
type D = delegate of int -> int
type M2 = class inherit D end
type IM2 = interface inherit IM end
type IM3 = interface inherit C1 end
type IM4 = interface interface C1 end
type IM5 = class interface C1 end

type S = struct new() = { }  end
module M = begin
  let i1 = new System.Nullable< System.Enum >()
  let i2 = new System.Nullable< System.ValueType >()
  let i3 = new System.Nullable< obj >()
  let i4 = new System.Nullable< string >()
  let i5 = new System.Nullable< System.Nullable<S> >()
end

type I = 
   interface
      abstract X : int 
      abstract Y : x:int -> int 
   end

module KeikoNakataBug = begin
  type c = class
   new () = {}
   abstract f : int -> int
  end

  type d = class
   inherit c
   new () = {}
  end

  let x = new d ()
  let y = x.f 1
end

module CheckNoImplementationsInAugmentations = begin
    type X = class
        abstract M : int
    end
    type X with 
        default a.M = 3
    end    
end
    
module CheckNoInterfacesInAugmentations = begin
    type IX = interface
        abstract M : int
    end
    type X = class
        interface IX 
    end
    type X with 
        interface IX with 
            member a.M = 3
        end
    end    
end

module CheckNoMembersOnEnumerations = begin

    // Check you can't have members in enumerations (see bug 1418, FSharp 1.0 database) *)
    type Season1 = Spring=0 | Summer=1 | Autumn=2 | Winter=3
        with
            static member M() = 1
        end


    // Check you can't have instance members in enumerations (see bug 1418, FSharp 1.0 database) *)
    type Season2 = Spring=0 | Summer=1 | Autumn=2 | Winter=3
        with
            member x.M() = 1
        end

    // Check you can't have instance members in enumerations (see bug 1418, FSharp 1.0 database) *)
    type Season3 = Spring=0 | Summer=1 | Autumn=2 | Winter=3
        with
            interface System.IComparable with 
                member x.CompareTo(yobj) = 0 
            end
        end

    type IEmpty = interface end
    // Check you can't have an implementation of an empty interface
    type Season4 = Spring=0 | Summer=1 | Autumn=2 | Winter=3
        with
            interface IEmpty
        end
end


module CheckNoMembersOnAbbreviations = begin

    // Check you can't have members in type abbreviation
    type Season1 = int
        with
            static member M() = 1
        end


    // Check you can't have members in type abbreviation
    type Season2 = int
        with
            member x.M() = 1
        end

    // Check you can't have members in type abbreviation
    type Season3 = int
        with
            interface System.IComparable with 
                member x.CompareTo(yobj) = 0 
            end
        end

    type IEmpty = interface end
    // Check you can't have an implementation of an empty interface
    type Season4 = int
        with
            interface IEmpty
        end
end

module CheckMissingMethod_FSharp_1_0_bug_1625 = begin
    type IA = 
        interface
            abstract M : int -> int 
        end

    type IB = 
        interface
            inherit IA
            abstract M : int -> int 
        end

    type C() = 
        //interface IA with 
         //   member x.M(y) = y+3
        class
            interface IB with 
                member x.M(y) = y+3
            end
        end
end

module EnumOfString_FSharp_1_0_bug_1743 = begin
    type IA<'a> =
        interface 
            abstract X : unit -> 'a
        end

    type IB<'a,'b> =
        interface 
            inherit IA<'a>
            inherit IA<'b>
        end

    let x = { new IB<_,_> with X() = failwith "" }
end

module EnumOfString_FSharp_1_0_bug_1730 = begin
    type EnumOfString =
        | A = "foo"
        | B = "bar"
end


module EnumOfString_FSharp_1_0_bug_1749 = begin
    type X() =
        interface
        end
end


module MiscBug_FSharp_1_0_bug_1683 = begin

    type Foo(x : int) =
       class
           member v.MyX() = x
       end

    let foo = {new Foo(3) with member v.MyX() = 4 end }

    do printf "%d" (foo.MyX())

end
module NoEnumValueWithSpecialName = begin
    type X =
        | value__ = 1
end

module CheckHonourObsoleteOnModules = begin
    [<System.Obsolete("Obsolete")>]
    module M = begin
       let x = 1
       type C = A | B
    end
    let _ = M.x
    let _ = M.A
    let _ = M.B
    let _ : M.C list = []
    let f = function M.A -> 1 | M.B -> 2
    open M

    module M2 = M
    let _ = M2.x
    let _ = M2.A
    let _ = M2.B
    let _ : M2.C list = []
    let f2 = function M2.A -> 1 | M2.B -> 2
    open M2

end

module CheckUnderConstrainedExplicitTyparDeclarationsGiveErrors = begin
    type Four = | Four

    // An unsealed type
    type C() = class
        member x.P = 1
    end
    
    type C4 =    class
        static member M<'a>(x:'a,y:C) = Four
       end

    module M0 = begin
        let gB4<'a,'b> (x:'a) (y:'b) = C4.M(x,y)  = Four    // expect: error, missing constraint
     end

    module M0Rec = begin

        // Expected: error
        let rec gB4<'a,'b>(x:'a) (y:'b) = C4.M(x,y)  = Four    // expect: error, missing constraint
      end

end

module CheckMisusedConstraints = begin
    let cantHashAFunctionValue = Operators.hash Operators.id
    let cantCompareFunctionValues = compare Operators.id Operators.id
    let cantEquateFunctionValues = Operators.id = Operators.id


    let uncheckHashAFunctionValue = Unchecked.hash Operators.id // no error expected
    let uncheckCompareFunctionValues = Unchecked.compare Operators.id Operators.id // no error expected
    let uncheckEquateFunctionValues = Unchecked.equals Operators.id Operators.id // no error expected

end


module CheckMisusedProperties = begin 
    module Test1 = begin

        type T() = 
          class

            member this.X1 = 1

            member this.X2 with get () = 2

            member this.X3 with get (idx1 : int) = 3
            member this.X4 with get (idx1 : int, idx2 : int) = 3

            //member this.X5 with get (idx1 : int) (idx2 : int) = 4 // should be rejected by parsing
            //member this.X6 with get (idx1 : int) (idx2 : int) (idx3 : int) = 4 // should be rejected by parsing


            member this.S2 with set () v = ()
            member this.S3 with set (idx1 : int) v = ()
            member this.S4 with set (idx1 : int, idx2 : int) v = ()

            static member SS2 with set () v = ()
            static member SS3 with set (idx1 : int) v = ()
            static member SS4 with set (idx1 : int, idx2 : int) v = ()

            //member this.S5 with set (idx1 : int) (idx2 : int) v = 4 // should be rejected by parsing

            
          end

        let tobj = T()
        
        let _ = tobj.X1
        let _ = tobj.X2
        let _ = tobj.X3 // should be rejected
        let _ = tobj.X4 // should be rejected
        let _ = tobj.S2 // should be rejected
        let _ = tobj.S3 // should be rejected
        let _ = tobj.S4 // should be rejected

        let _ = T.SS2 // should be rejected
        let _ = T.SS3 // should be rejected
        let _ = T.SS4 // should be rejected
    end
    
    module Test2 =  begin
        type T() =
          class
             member this.X
               with set (a:int, b:int) = ()
          end

        let _ = (new T()).X <- (1,2) // should be ok 
        let _ = (new T()).X(1) <- 2 // should be rejected

        type T2() =
           class
             member this.X
               with set (a:int) (b:int) = ()
           end

        let _ = (new T2()).X <- (1,2) // should be rejected, currently not rejected
        let _ = (new T2()).X(1) <- 2 // should be ok

        type T3() =
          class
             member this.X
               with set (a:int) (b:int, c:int) = ()
          end

        let _ = (new T3()).X <- (1,2,3) // should be rejected
        let _ = (new T3()).X(1) <- (2,3) // should be ok
        let _ = (new T3()).X(1,2) <- 2 // should be rejected
    end
end

module MissingStaticOperatorsTests = begin

    module ActualRepro = begin
        open System.Data.SqlTypes

        let d = SqlDecimal(12)
        let x = float d
    end

    module SystematicTests1 = begin

        type C() = class
            static member op_Explicit(c: C) : int = 1
            static member op_Explicit(c: C) : double = 2.0
            static member op_Explicit(c: C) : char = 'a'
            static member op_Explicit(c: C) : byte = byte 1 
        end

        let c = C()
        let x1 = float c // expect ok
        let x2 = char c // expect ok
        let x3 = byte c // expect ok
        let x4 = int c  // expect ok
        let x5 = single c // expect fail with decent error message
        let x6 = sbyte c // expect fail with decent error message
        let x7 = int16 c // expect fail with decent error message
        let x8 = uint16 c // expect fail with decent error message
        let x9 = uint32 c // expect fail with decent error message
        let xQ = uint64 c // expect fail with decent error message
        let xW = int64 c // expect fail with decent error message
        let xE = decimal c // expect fail with decent error message

    end

    module SystematicTests2 = begin

        // From the F# perspective it doesn't matter if overloads are op_Explicit or op_Implicit
        type C() = class
            static member op_Explicit(c: C) : int = 1
            static member op_Explicit(c: C) : double = 2.0
            static member op_Implicit(c: C) : char = 'a'
            static member op_Implicit(c: C) : byte = byte 1 
        end

        let c = C()
        let x1 = float c // expect ok
        let x2 = char c // expect ok
        let x3 = byte c // expect ok
        let x4 = int c  // expect ok
        let x5 = single c // expect fail with decent error message
        let x6 = sbyte c // expect fail with decent error message
        let x7 = int16 c // expect fail with decent error message
        let x8 = uint16 c // expect fail with decent error message
        let x9 = uint32 c // expect fail with decent error message
        let xQ = uint64 c // expect fail with decent error message
        let xW = int64 c // expect fail with decent error message
        let xE = decimal c // expect fail with decent error message
        
        let y1 : float = C.op_Explicit c // expect ok
        let y2 : char = C.op_Implicit c // expect ok
        let yA : char = C.op_Explicit c // expect error - no special treatment for direct calls
        let y3 : byte = C.op_Implicit c // expect ok
        let yS : byte = C.op_Explicit c // expect ok
        let y4 : int = C.op_Explicit c  // expect ok
        let y5 : single = C.op_Explicit c // expect fail with decent error message
        let y6 : sbyte = C.op_Explicit c // expect fail with decent error message
        let y7 : int16 = C.op_Explicit c // expect fail with decent error message
        let y8 : uint16 = C.op_Explicit c // expect fail with decent error message
        let y9 : uint32 = C.op_Explicit c // expect fail with decent error message
        let yQ : uint64 = C.op_Explicit c // expect fail with decent error message
        let yW : int64 = C.op_Explicit c // expect fail with decent error message
        let yE : decimal = C.op_Explicit c // expect fail with decent error message

    end

    // Also check the case where there is only one overload!
    module SystematicTests3 = begin

        type C() = class
            static member op_Explicit(c: C) : int = 1
        end

        let c = C()
        let x1 = float c // expect fail with decent error message
        let x2 = char c // expect fail with decent error message
        let x3 = byte c // expect fail with decent error message
        let x4 = int c  // expect ok
        let x5 = single c // expect fail with decent error message
        let x6 = sbyte c // expect fail with decent error message
        let x7 = int16 c // expect fail with decent error message
        let x8 = uint16 c // expect fail with decent error message
        let x9 = uint32 c // expect fail with decent error message
        let xQ = uint64 c // expect fail with decent error message
        let xW = int64 c // expect fail with decent error message
        let xE = decimal c // expect fail with decent error message

    end

    // Also check the case where there is only one overload!
    module SystematicTests4 = begin

        type C() = class
           // From the F# perspective it doesn't matter if overloads are op_Explicit or op_Implicit
            static member op_Implicit(c: C) : int = 1
        end

        let c = C()
        let x1 = float c // expect fail with decent error message
        let x2 = char c // expect fail with decent error message
        let x3 = byte c // expect fail with decent error message
        let x4 = int c  // expect ok
        let x5 = single c // expect fail with decent error message
        let x6 = sbyte c // expect fail with decent error message
        let x7 = int16 c // expect fail with decent error message
        let x8 = uint16 c // expect fail with decent error message
        let x9 = uint32 c // expect fail with decent error message
        let xQ = uint64 c // expect fail with decent error message
        let xW = int64 c // expect fail with decent error message
        let xE = decimal c // expect fail with decent error message

    end
    
end

