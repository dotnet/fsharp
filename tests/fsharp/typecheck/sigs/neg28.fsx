
module AdhocStructuralEqualityAndCOmparisonTests =

    [<StructuralEquality>]
    type X = X of (int -> int)


module PairWithPrimaryConstructorCantContainVal =

    [<Struct>]
    type Pair(x:int, y:int) =
        val z : int
        member p.X = x
        member p.Y = y

module StructPrimaryConstructorARgsMustByTypeAnnotated =

    [<Struct>]
    type U3(v) = 
        member x.P = v

module StructDefaultConstructorCantBeUsed =

    [<Struct>]
    type U1(v:int) = 
        member x.P = v

    [<Struct>]
    type U2(v:list<int>) = 
        member x.P = 1

    let v1 = U1()  // can be used - expect ok
    let v2 = U2() // can't be used - expect an error


module BasicExample1 =
    let f1 (x : list<int>) = (x = x) // expect ok
    let f2 (x : option<int>) = (x = x) // expect ok
    let f3 (x : Choice<int,int>) = (x = x) // expect ok
    let f4 (x : Choice<int,int,int>) = (x = x) // expect ok
    let f5 (x : Choice<int,int,int,int>) = (x = x) // expect ok
    let f6 (x : Choice<int,int,int,int,int>) = (x = x) // expect ok
    let f7 (x : Choice<int,int,int,int,int,int>) = (x = x) // expect ok
    let f8 (x : Choice<int,int,int,int,int,int,int>) = (x = x) // expect ok
    let f9 (x : ref<int>) = (x = x) // expect ok
    let fq (x : Set<int>) = (x = x) // expect ok
    let fw (x : Map<int,int>) = (x = x) // expect ok
    
    let fe (x : list<System.Type>) = (x = x) // expect ok
    let fr (x : option<System.Type>) = (x = x) // expect ok
    let ft (x : Choice<System.Type,int>) = (x = x) // expect ok
    let fy (x : Choice<System.Type,int,int>) = (x = x) // expect ok
    let fu (x : Choice<System.Type,int,int,int>) = (x = x) // expect ok
    let fi (x : Choice<System.Type,int,int,int,int>) = (x = x) // expect ok
    let fo (x : Choice<System.Type,int,int,int,int,int>) = (x = x) // expect ok
    let fp (x : Choice<System.Type,int,int,int,int,int,int>) = (x = x) // expect ok
    let fa (x : ref<System.Type>) = (x = x) // expect ok
    let fs (x : Set<System.Type>) = (x = x) // expect error
    let fd (x : Map<System.Type,int>) = (x = x) // expect error

    let ff (x : list<(int -> int)>) = (x = x) // expect error
    let fg (x : option<(int -> int)>) = (x = x) // expect error
    let fh (x : Choice<(int -> int),int>) = (x = x) // expect error
    let fj (x : Choice<(int -> int),int,int>) = (x = x) // expect error
    let fk (x : Choice<(int -> int),int,int,int>) = (x = x) // expect error
    let fl (x : Choice<(int -> int),int,int,int,int>) = (x = x) // expect error
    let fz (x : Choice<(int -> int),int,int,int,int,int>) = (x = x) // expect error
    let fx (x : Choice<(int -> int),int,int,int,int,int,int>) = (x = x) // expect error
    let fc (x : ref<(int -> int)>) = (x = x) // expect error
    let fv (x : Set<(int -> int)>) = (x = x) // expect error
    let fb (x : Map<(int -> int),int>) = (x = x) // expect error


    let fn (x : Set<list<int>>) = () // expect ok
    let fm (x : Set<option<int>>) = () // expect ok
    let fQ (x : Set<ref<int>>) = () // expect ok
    let fW (x : Set<Set<int>>) = () // expect ok
    let fE (x : Set<Map<int,int>>) = () // expect ok
    let fR (x : Set<list<System.Type>>) = () // expect error
    let fT (x : Set<option<System.Type>>) = () // expect error
    let fY (x : Set<ref<System.Type>>) = () // expect error
    let fU (x : Set<Set<System.Type>>) = () // expect error
    let fI (x : Set<Map<System.Type,int>>) = () // expect error
    let fO (x : Set<int * int>) = () // expect ok
    let fP (x : Set<int * int * int>) = () // expect ok
    let fA (x : Set<int * int * int * int>) = () // expect ok
    let fS (x : Set<int * int * int * int * int>) = () // expect ok
    let fD (x : Set<int * int * int * int * int * int>) = () // expect ok
    let fF (x : Set<int * int * int * int * int * int * int>) = () // expect ok
    let fG (x : Set<int * int * int * int * int * int * int * int>) = () // expect ok
    let fH (x : Set<int * int * int * int * int * int * int * int * int >) = () // expect ok
    let fJ (x : Set<int * int * int * int * int * int * int * int * int * int>) = () // expect ok
    let fK (x : Set<int * int * int * int * int * int * int * int * int * int * int>) = () // expect ok

    type R<'T> = R of 'T * R<'T>
    let r1 (x : Set<R<int>>) = () // expect ok
    let r2 (x : Set<R<System.Type>>) = () // expect error
    let r3 (x : R<int>) = (x = x) // expect ok
    let r4 (x : R<System.Type>) = (x = x) // expect ok
    let r5 (x : R<int -> int>) = (x = x) // expect error

    
module Example1 =
    type X<'T> = X of 'T

    let f0 (x : Set<X<int>>) = ()  // expect ok
    let f1 (x : Set<X<'T>>) = ()  // expect ok
    let f2 (x : Set<X<System.Type>>) = ()  // expect error
    let f3 (x : X<list<int>>) = (x = x) // expect ok
    let f4 (x : X<int -> int>) = (x = x) // expect error   
    let f5 (x : X<list<System.Type>>) = (x = x) // expect ok
    let f6 (x : X<list<int -> int>>) = (x = x) // expect error    

module Example1_Record =
    type X<'T> = { r : 'T }

    let f0 (x : Set<X<int>>) = ()  // expect ok
    let f1 (x : Set<X<'T>>) = ()  // expect ok
    let f2 (x : Set<X<System.Type>>) = ()  // expect error
    let f3 (x : X<list<int>>) = (x = x) // expect ok
    let f4 (x : X<int -> int>) = (x = x) // expect error   
    let f5 (x : X<list<System.Type>>) = (x = x) // expect ok
    let f6 (x : X<list<int -> int>>) = (x = x) // expect error    

module Example1_Struct =
    type X<'T> = struct val r : 'T  end

    let f0 (x : Set<X<int>>) = ()  // expect ok
    let f1 (x : Set<X<'T>>) = ()  // expect ok
    let f2 (x : Set<X<System.Type>>) = ()  // expect error
    let f3 (x : X<list<int>>) = (x = x) // expect ok
    let f4 (x : X<int -> int>) = (x = x) // expect error   
    let f5 (x : X<list<System.Type>>) = (x = x) // expect ok
    let f6 (x : X<list<int -> int>>) = (x = x) // expect error    

module Example1_StructImplicit =
    [<Struct; StructuralComparison; StructuralEquality>]
    type X<[<EqualityConditionalOn;ComparisonConditionalOn>] 'T>(r:'T) = struct member x.R = r  end

    let f0 (x : Set<X<int>>) = ()  // expect ok
    let f1 (x : Set<X<'T>>) = ()  // expect ok
    let f2 (x : Set<X<System.Type>>) = ()  // expect error 
    let f3 (x : X<list<int>>) = (x = x) // expect ok
    let f4 (x : X<int -> int>) = (x = x) // expect error   
    let f5 (x : X<list<System.Type>>) = (x = x) // expect ok
    let f6 (x : X<list<int -> int>>) = (x = x) // expect error    

module Example1_StructImplicit2 =
    [<Struct>]
    type X<[<EqualityConditionalOn;ComparisonConditionalOn>] 'T>(r:'T) = struct member x.R = r  end

    let f0 (x : Set<X<int>>) = ()  // expect ok
    let f1 (x : Set<X<'T>>) = ()  // expect ok
    let f2 (x : Set<X<System.Type>>) = ()  // expect error 
    let f3 (x : X<list<int>>) = (x = x) // expect ok
    let f4 (x : X<int -> int>) = (x = x) // expect error   
    let f5 (x : X<list<System.Type>>) = (x = x) // expect ok
    let f6 (x : X<list<int -> int>>) = (x = x) // expect error    

module Example2 = 
    type X<'T> = X of list<'T>

    let f0 (x : Set<X<int>>) = ()   // expect ok
    let f1 (x : Set<X<'T>>) = ()   // expect ok
    let f2 (x : Set<X<System.Type>>) = () // expect error
    let f3 (x : Set<X<list<System.Type>>>) = () // expect error

    let f4 (x : X<list<int>>) = (x = x) // expect ok
    let f5 (x : X<int -> int>) = (x = x) // expect error   
    let f6 (x : X<list<System.Type>>) = (x = x) // expect ok
    let f7 (x : X<list<int -> int>>) = (x = x) // expect error    


module Example3 = 
    type X<'T> = X of Y<'T>
    and Y<'T> = Y of 'T

    let f0 (x : Set<X<int>>) = ()   // expect ok
    let f1 (x : Set<X<'T>>) = ()   // expect ok
    let f2 (x : Set<X<System.Type>>) = () // expect error
    let f3 (x : Set<X<list<System.Type>>>) = () // expect error

    let f4 (x : X<list<int>>) = (x = x) // expect ok
    let f5 (x : X<int -> int>) = (x = x) // expect error   
    let f6 (x : X<list<System.Type>>) = (x = x) // expect ok
    let f7 (x : X<list<int -> int>>) = (x = x) // expect error    

module Example4 = 
    type X<'T> = X of Y<'T>
    and Y<'T> = Y of 'T * X<'T>

    let f0 (x : Set<X<int>>) = ()   // expect ok
    let f1 (x : Set<X<'T>>) = ()   // expect ok
    let f2 (x : Set<X<System.Type>>) = () // expect error
    let f3 (x : Set<X<list<System.Type>>>) = () // expect error

    let f4 (x : X<list<int>>) = (x = x) // expect ok
    let f5 (x : X<int -> int>) = (x = x) // expect error   
    let f6 (x : X<list<System.Type>>) = (x = x) // expect ok
    let f7 (x : X<list<int -> int>>) = (x = x) // expect error    

    let g0 (x : Set<Y<int>>) = ()   // expect ok
    let g1 (x : Set<Y<'T>>) = ()   // expect ok
    let g2 (x : Set<Y<System.Type>>) = () // expect error
    let g3 (x : Set<Y<list<System.Type>>>) = () // expect error

    let g4 (x : Y<list<int>>) = (x = x) // expect ok
    let g5 (x : Y<int -> int>) = (x = x) // expect error   
    let g6 (x : Y<list<System.Type>>) = (x = x) // expect ok
    let g7 (x : Y<list<int -> int>>) = (x = x) // expect error    

module Example5 = 
    type X<'T> = X of Y<'T>
    and Y<'T> = Y of int

    let f0 (x : Set<X<int>>) = ()   // expect ok
    let f1 (x : Set<X<'T>>) = ()   // expect ok
    let f2 (x : Set<X<System.Type>>) = () // expect ok
    let f3 (x : Set<X<list<System.Type>>>) = () // expect ok

    let f4 (x : X<list<int>>) = (x = x) // expect ok
    let f5 (x : X<int -> int>) = (x = x) // expect ok
    let f6 (x : X<list<System.Type>>) = (x = x) // expect ok
    let f7 (x : X<list<int -> int>>) = (x = x) // expect ok

    let g0 (x : Set<Y<int>>) = ()   // expect ok
    let g1 (x : Set<Y<'T>>) = ()   // expect ok
    let g2 (x : Set<Y<System.Type>>) = () // expect ok
    let g3 (x : Set<Y<list<System.Type>>>) = () // expect ok

    let g4 (x : Y<list<int>>) = (x = x) // expect ok
    let g5 (x : Y<int -> int>) = (x = x) // expect ok
    let g6 (x : Y<list<System.Type>>) = (x = x) // expect ok
    let g7 (x : Y<list<int -> int>>) = (x = x) // expect ok

module Example6 = 
    type X<'T> = X of Y<int,'T>
    and Y<'T,'U> = Y of 'T * X<'T>

    let f0 (x : Set<X<int>>) = ()   // expect ok
    let f1 (x : Set<X<'T>>) = ()   // expect ok
    let f2 (x : Set<X<System.Type>>) = () // expect ok
    let f3 (x : Set<X<list<System.Type>>>) = () // expect ok

    let f4 (x : X<list<int>>) = (x = x) // expect ok
    let f5 (x : X<int -> int>) = (x = x) // expect ok
    let f6 (x : X<list<System.Type>>) = (x = x) // expect ok
    let f7 (x : X<list<int -> int>>) = (x = x) // expect ok

    let g0 (x : Set<Y<int,int>>) = ()   // expect ok
    let g1 (x : Set<Y<'T,'T>>) = ()   // expect ok
    let g2 (x : Set<Y<System.Type,int>>) = () // expect error
    let g3 (x : Set<Y<list<System.Type>,int>>) = () // expect error

    let g4 (x : Y<list<int>,int>) = (x = x) // expect ok
    let g5 (x : Y<(int -> int), int>) = (x = x) // expect error   
    let g6 (x : Y<list<System.Type>, int>) = (x = x) // expect ok
    let g7 (x : Y<list<(int -> int)>, int>) = (x = x) // expect error    


    let g8 (x : Y<int,list<int>>) = (x = x) // expect ok
    let g9 (x : Y<int,(int -> int)>) = (x = x) // expect ok
    let g10 (x : Y<int,list<System.Type>>) = (x = x) // expect ok
    let g11 (x : Y<int,list<(int -> int)>>) = (x = x) // expect ok

module Example7 = 
    // a type inferred to be without equality or comparison
    type X = X of (int -> int)
    // a type transitively inferred to be without equality or comparison
    type  Y = Y of X

    let f0 (x : Set<X>) = ()   // expect error
    let f1 (x : Set<Y>) = ()   // expect error

    let f2 (x : X) = (x = x) // expect error
    let f3 (x : Y) = (x = x) // expect error

module Example8 = 
    // a type inferred to be without comparison
    type X = X of System.Type
    // a type transitively inferred to be without comparison
    type  Y = Y of X

    let f0 (x : Set<X>) = ()   // expect error
    let f1 (x : Set<Y>) = ()   // expect error

    let f2 (x : X) = (x = x) // expect ok
    let f3 (x : Y) = (x = x) // expect ok

