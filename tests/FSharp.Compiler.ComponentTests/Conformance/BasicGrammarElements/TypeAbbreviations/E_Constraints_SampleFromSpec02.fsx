// #Regression #Conformance #TypesAndModules 
// Abbreviation: the constraints on the right-hand-side are not sufficient

#light

type IA = 
    abstract AbstractMember : int -> int

type IB = 
    abstract AbstractMember : int -> int

type C<'a when 'a :> IB>() = 
    static member StaticMember(x:'a) = x.AbstractMember(1)

// invalid: missing constraint
type E<'b> = C<'b>      // error

exit 1

