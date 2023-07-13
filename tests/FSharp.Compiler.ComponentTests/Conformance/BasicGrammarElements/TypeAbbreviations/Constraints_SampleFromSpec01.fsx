// #Conformance #TypesAndModules 
// Abbreviation: the constraints on the right-hand-side are sufficient
//<Expects status="success"></Expects>
#light

type IA = 
    abstract AbstractMember : int -> int

type IB = 
    abstract AbstractMember : int -> int

type C<'a when 'a :> IB>() = 
    static member StaticMember(x:'a) = x.AbstractMember(1)


// Abbreviation: the constraints on the right-hand-side are sufficient
type D<'b when 'b :> IB> = C<'b>
