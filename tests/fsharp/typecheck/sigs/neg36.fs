module Neg36

module TestAbstractOverrides_Bug4232_Case1 = 
    [<AbstractClass>]
    type D<'T,'U>() = 
        abstract M : 'T  -> int
        abstract M : 'U -> int
        
    type E() = 
        inherit D<string,string>()
        override x.M(a:string) = 1

module TestAbstractOverrides_Bug4232_Case2 = 
        
    [<AbstractClass>]
    type PA() =
        abstract M : int -> unit

    [<AbstractClass>]
    type PB<'a>() =
        inherit PA()
        abstract M : 'a -> unit

    [<AbstractClass>]
    type PC() =
        inherit PB<int>()
        // Here, PA.M amd PB<int>.M have the same signature, so PA.M is unimplementable.
        // EXPECT: friendly error at this point?

    type PD() = 
        inherit PC()
        override this.M(x:int) = ()

