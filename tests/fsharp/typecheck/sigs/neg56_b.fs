module Neg56

// Similar repro to above, but involving a property

module Devdiv2_Bug_10649_case0 = 
    type Foo<'T>() = 
        static member Inf = ref Unchecked.defaultof<_>
        member this.Bar() = Foo<_>.Inf.Value : 'T 


    // Similar repro to above, but involving a static property
    type Foo2<'T>() = 
        static let x = (); Unchecked.defaultof<_>
        static member Inf = ref x
        member this.Bar() = Foo2<_>.Inf.Value : 'T 

module Devdiv2_Bug_10649 = 
    type Foo<'T> = FooCase of 'T
        with
        //static member private Inf l = ignore l  // ok
        static member private Inf = function
            | h::t -> h :: Foo<_>.Inf t
            | [] -> []
        member this.Bar() =
            let (FooCase(x)) = this
            Foo<_>.Inf [x; x]
 

module Devdiv2_Bug_10649_repro2 = 
    // compiles ok
    type Qux<'T>(x:'T) =
        static member private Foo = function
            | h::t -> h :: Qux<_>.Foo t
            | [] -> []
        member this.Bar() =
            Qux<_>.Foo [x; x]
 
